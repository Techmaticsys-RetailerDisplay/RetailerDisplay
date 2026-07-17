import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { playlistsApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Playlist } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText } from "../../components/ui";
import { IconPlaylist } from "../../components/icons";

export default function PlaylistsPage() {
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load() {
    try { setPlaylists(await playlistsApi.list()); } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  async function create() {
    if (!name.trim()) return;
    setBusy(true); setError(null);
    try { await playlistsApi.create(name.trim()); setName(""); await load(); }
    catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  return (
    <div>
      <PageHeader title="Playlists" subtitle="Collections of content assigned to devices" />
      <Card>
        <div className="toolbar">
          <div className="grow"><TextInput value={name} onChange={setName} placeholder="New playlist name" /></div>
          <Button onClick={create} disabled={busy}>Create playlist</Button>
        </div>
        <ErrorText>{error}</ErrorText>
      </Card>

      <div style={{ height: 18 }} />
      <div className="card-grid">
        {playlists.map((p) => (
          <Card key={p.playlistId}>
            <div className="stat-top">
              <div style={{ fontWeight: 700, fontSize: 16 }}>{p.name}</div>
              <div className="stat-icon"><IconPlaylist size={17} /></div>
            </div>
            <div className="cell-muted" style={{ fontSize: 13, marginTop: 4 }}>
              {p.itemCount} item{p.itemCount === 1 ? "" : "s"} · v{p.version}
            </div>
            <div className="toolbar" style={{ marginTop: 14 }}>
              <Link to={`/playlists/${p.playlistId}`} className="btn btn-primary btn-sm" style={{ textDecoration: "none" }}>Manage content</Link>
              <Button size="sm" variant="danger" onClick={() => playlistsApi.remove(p.playlistId).then(load)}>Delete</Button>
            </div>
          </Card>
        ))}
        {playlists.length === 0 && <span className="cell-muted">No playlists yet.</span>}
      </div>
    </div>
  );
}
