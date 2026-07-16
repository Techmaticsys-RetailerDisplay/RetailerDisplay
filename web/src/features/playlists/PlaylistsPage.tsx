import { useEffect, useState } from "react";
import { playlistsApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Playlist } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText } from "../../components/ui";

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
      <Card style={{ marginBottom: 20 }}>
        <div style={{ display: "flex", gap: 10 }}>
          <div style={{ flex: 1 }}><TextInput value={name} onChange={setName} placeholder="New playlist name" /></div>
          <Button onClick={create} disabled={busy}>Create playlist</Button>
        </div>
        <ErrorText>{error}</ErrorText>
      </Card>

      <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(240px, 1fr))", gap: 14 }}>
        {playlists.map((p) => (
          <Card key={p.playlistId}>
            <div style={{ fontWeight: 700, fontSize: 16 }}>{p.name}</div>
            <div style={{ color: "#616b7a", fontSize: 13, marginTop: 4 }}>
              {p.itemCount} item{p.itemCount === 1 ? "" : "s"} · v{p.version}
            </div>
            <div style={{ marginTop: 12 }}>
              <Button variant="danger" onClick={() => playlistsApi.remove(p.playlistId).then(load)}>Delete</Button>
            </div>
          </Card>
        ))}
        {playlists.length === 0 && <span style={{ color: "#8a94a3" }}>No playlists yet.</span>}
      </div>
      <p style={{ color: "#8a94a3", fontSize: 13, marginTop: 20 }}>
        Item ordering, per-item duration and fit-mode (drag-to-reorder builder) wire to
        <code style={{ margin: "0 4px" }}>PUT /playlists/&#123;id&#125;/items</code> — next increment.
      </p>
    </div>
  );
}
