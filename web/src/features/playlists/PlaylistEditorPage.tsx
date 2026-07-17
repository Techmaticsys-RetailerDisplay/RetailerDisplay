import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { playlistsApi, contentApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { ContentItem } from "../../api/types";
import { PageHeader, Card, Button, ErrorText, EmptyRow } from "../../components/ui";

const FIT_MODES = [
  { v: 1, label: "Contain" },
  { v: 2, label: "Cover" },
  { v: 3, label: "Stretch" },
];
const fitToNum = (s: string) => FIT_MODES.find((f) => f.label === s)?.v ?? 1;

interface Row {
  contentId: number;
  contentName: string;
  contentType: string;
  durationSeconds: number;
  fitMode: number;
  thumbnailUrl?: string | null;
  previewUrl?: string | null;
}

function RowThumb({ row }: { row: Row }) {
  const box: React.CSSProperties = {
    width: 64, height: 40, borderRadius: 6, border: "1px solid var(--border)",
    overflow: "hidden", display: "grid", placeItems: "center",
    background: "var(--surface-3)", color: "var(--faint)", fontSize: 16,
  };
  if (row.contentType === "Image" && row.thumbnailUrl) {
    return (
      <a href={row.previewUrl ?? row.thumbnailUrl} target="_blank" rel="noreferrer" title="View full size">
        <img src={row.thumbnailUrl} alt={row.contentName} style={{ ...box, objectFit: "cover", display: "block" }} />
      </a>
    );
  }
  if (row.contentType === "Video") return <div style={box}>▶</div>;
  return <div style={box}>▦</div>;
}

export default function PlaylistEditorPage() {
  const { id } = useParams();
  const playlistId = Number(id);
  const navigate = useNavigate();

  const [name, setName] = useState("");
  const [version, setVersion] = useState(0);
  const [rows, setRows] = useState<Row[]>([]);
  const [content, setContent] = useState<ContentItem[]>([]);
  const [addId, setAddId] = useState<string>("");
  const [error, setError] = useState<string | null>(null);
  const [note, setNote] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        const [detail, all] = await Promise.all([playlistsApi.get(playlistId), contentApi.list()]);
        setName(detail.name);
        setVersion(detail.version);
        const byId = new Map(all.map((c) => [c.contentId, c]));
        setRows(detail.items.map((i) => {
          const c = byId.get(i.contentId);
          return {
            contentId: i.contentId,
            contentName: i.contentName,
            contentType: i.contentType,
            durationSeconds: i.durationSeconds ?? 10,
            fitMode: fitToNum(i.fitMode),
            thumbnailUrl: c?.thumbnailUrl,
            previewUrl: c?.previewUrl,
          };
        }));
        setContent(all.filter((c) => c.isActive));
      } catch (e) { setError(apiError(e)); }
    })();
  }, [playlistId]);

  function addContent() {
    const cid = Number(addId);
    if (!cid) return;
    if (rows.some((r) => r.contentId === cid)) { setAddId(""); return; } // no duplicates
    const c = content.find((x) => x.contentId === cid);
    if (!c) return;
    setRows((r) => [...r, { contentId: c.contentId, contentName: c.name, contentType: c.contentType, durationSeconds: 10, fitMode: 1, thumbnailUrl: c.thumbnailUrl, previewUrl: c.previewUrl }]);
    setAddId("");
  }

  // Content not already in the playlist (a content item can appear at most once).
  const available = content.filter((c) => !rows.some((r) => r.contentId === c.contentId));

  function move(i: number, dir: -1 | 1) {
    setRows((r) => {
      const n = [...r];
      const j = i + dir;
      if (j < 0 || j >= n.length) return r;
      [n[i], n[j]] = [n[j], n[i]];
      return n;
    });
  }
  function removeRow(i: number) { setRows((r) => r.filter((_, idx) => idx !== i)); }
  function setRow(i: number, patch: Partial<Row>) { setRows((r) => r.map((row, idx) => idx === i ? { ...row, ...patch } : row)); }

  async function save() {
    setBusy(true); setError(null); setNote(null);
    try {
      const detail = await playlistsApi.setItems(playlistId, rows.map((r) => ({
        contentId: r.contentId,
        durationSeconds: r.durationSeconds,
        fitMode: r.fitMode,
      })));
      setVersion(detail.version);
      // Reflect the persisted (de-duped) state.
      setRows(detail.items.map((i) => {
        const c = content.find((x) => x.contentId === i.contentId);
        return {
          contentId: i.contentId,
          contentName: i.contentName,
          contentType: i.contentType,
          durationSeconds: i.durationSeconds ?? 10,
          fitMode: fitToNum(i.fitMode),
          thumbnailUrl: c?.thumbnailUrl,
          previewUrl: c?.previewUrl,
        };
      }));
      setNote(`Saved. Playlist is now version ${detail.version} — assigned devices will pick it up.`);
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  return (
    <div>
      <PageHeader
        title={name || "Playlist"}
        subtitle={`Version ${version} · ${rows.length} item${rows.length === 1 ? "" : "s"}`}
        action={<Button variant="ghost" onClick={() => navigate("/playlists")}>← Back</Button>}
      />
      <ErrorText>{error}</ErrorText>
      {note && <div className="ok-text" style={{ marginBottom: 12 }}>{note}</div>}

      <Card>
        <div className="toolbar">
          <select className="select grow" value={addId} onChange={(e) => setAddId(e.target.value)}>
            <option value="">Add content…</option>
            {available.map((c) => <option key={c.contentId} value={c.contentId}>{c.name} ({c.contentType})</option>)}
          </select>
          <Button onClick={addContent} disabled={!addId}>Add</Button>
        </div>
        {content.length === 0 && <div className="hint" style={{ marginTop: 8 }}>No content yet — create some on the Content page first.</div>}
        {content.length > 0 && available.length === 0 && <div className="hint" style={{ marginTop: 8 }}>All your content is already in this playlist.</div>}
      </Card>

      <div style={{ height: 16 }} />
      <Card pad={false}>
        <div className="table-wrap">
          <table className="data">
            <thead>
              <tr><th>#</th><th>Preview</th><th>Content</th><th>Type</th><th>Duration (s)</th><th>Fit</th><th>Order</th><th></th></tr>
            </thead>
            <tbody>
              {rows.map((r, i) => (
                <tr key={`${r.contentId}-${i}`}>
                  <td className="cell-muted">{i + 1}</td>
                  <td><RowThumb row={r} /></td>
                  <td className="cell-strong">{r.contentName}</td>
                  <td><span className="tag">{r.contentType}</span></td>
                  <td>
                    {r.contentType === "Video"
                      ? <span className="cell-muted">full length</span>
                      : <input className="input" style={{ width: 90 }} type="number" min={1} value={r.durationSeconds}
                          onChange={(e) => setRow(i, { durationSeconds: Number(e.target.value) })} />}
                  </td>
                  <td>
                    <select className="select" style={{ width: 120 }} value={r.fitMode} onChange={(e) => setRow(i, { fitMode: Number(e.target.value) })}>
                      {FIT_MODES.map((f) => <option key={f.v} value={f.v}>{f.label}</option>)}
                    </select>
                  </td>
                  <td>
                    <div className="toolbar">
                      <Button size="sm" variant="ghost" onClick={() => move(i, -1)}>↑</Button>
                      <Button size="sm" variant="ghost" onClick={() => move(i, 1)}>↓</Button>
                    </div>
                  </td>
                  <td><Button size="sm" variant="danger" onClick={() => removeRow(i)}>Remove</Button></td>
                </tr>
              ))}
              {rows.length === 0 && <EmptyRow colSpan={8}>No items yet. Add content above.</EmptyRow>}
            </tbody>
          </table>
        </div>
      </Card>

      <div style={{ marginTop: 16 }}>
        <Button onClick={save} disabled={busy}>{busy ? "Saving…" : "Save playlist"}</Button>
      </div>
    </div>
  );
}
