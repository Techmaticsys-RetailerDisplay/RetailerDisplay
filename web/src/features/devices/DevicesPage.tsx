import { useEffect, useState } from "react";
import { devicesApi, playlistsApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Device, Playlist } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText, StatusPill, EmptyRow } from "../../components/ui";

export default function DevicesPage() {
  const [devices, setDevices] = useState<Device[]>([]);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load() {
    try {
      const [d, p] = await Promise.all([devicesApi.list(), playlistsApi.list()]);
      setDevices(d); setPlaylists(p);
    } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  async function register() {
    setBusy(true); setError(null);
    try { await devicesApi.register({ deviceName: name.trim() || undefined }); setName(""); await load(); }
    catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  async function act(fn: () => Promise<unknown>) {
    setError(null);
    try { await fn(); await load(); } catch (e) { setError(apiError(e)); }
  }

  return (
    <div>
      <PageHeader title="Devices" subtitle="Screens paired to your account" />
      <Card>
        <div className="toolbar">
          <div className="grow"><TextInput value={name} onChange={setName} placeholder="Device name (e.g. Front Window TV)" /></div>
          <Button onClick={register} disabled={busy}>Register device</Button>
        </div>
        <ErrorText>{error}</ErrorText>
      </Card>

      <div style={{ height: 18 }} />
      <Card pad={false}>
        <div className="table-wrap">
          <table className="data">
            <thead>
              <tr><th>Status</th><th>Name</th><th>Key</th><th>Playlist</th><th>Actions</th></tr>
            </thead>
            <tbody>
              {devices.map((d) => (
                <tr key={d.deviceId}>
                  <td><StatusPill status={d.status} /></td>
                  <td className="cell-strong">{d.deviceName ?? "—"}</td>
                  <td><span className="tag cell-mono">{d.deviceKey}</span></td>
                  <td>
                    <select className="select" style={{ width: 180 }} value={d.playlistId ?? ""}
                      onChange={(e) => act(() => devicesApi.assignPlaylist(d.deviceId, e.target.value ? Number(e.target.value) : null))}>
                      <option value="">— none —</option>
                      {playlists.map((p) => <option key={p.playlistId} value={p.playlistId}>{p.name}</option>)}
                    </select>
                  </td>
                  <td>
                    <div className="toolbar">
                      <Button size="sm" variant="ghost" onClick={() => act(() => devicesApi.refresh(d.deviceId))}>Refresh</Button>
                      <Button size="sm" variant="danger" onClick={() => act(() => devicesApi.revoke(d.deviceId))}>Revoke</Button>
                    </div>
                  </td>
                </tr>
              ))}
              {devices.length === 0 && <EmptyRow colSpan={5}>No devices yet. Register one, then enter its key in the TV app.</EmptyRow>}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
}
