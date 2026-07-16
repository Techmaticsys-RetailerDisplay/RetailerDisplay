import { useEffect, useState } from "react";
import { devicesApi, playlistsApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Device, Playlist } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText, StatusPill } from "../../components/ui";

export default function DevicesPage() {
  const [devices, setDevices] = useState<Device[]>([]);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function load() {
    try {
      const [d, p] = await Promise.all([devicesApi.list(), playlistsApi.list()]);
      setDevices(d);
      setPlaylists(p);
    } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  async function register() {
    setBusy(true); setError(null);
    try {
      await devicesApi.register({ deviceName: name.trim() || undefined });
      setName("");
      await load();
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  async function act(fn: () => Promise<unknown>) {
    setError(null);
    try { await fn(); await load(); } catch (e) { setError(apiError(e)); }
  }

  return (
    <div>
      <PageHeader title="Devices" subtitle="Screens paired to your account" />
      <Card style={{ marginBottom: 20 }}>
        <div style={{ display: "flex", gap: 10 }}>
          <div style={{ flex: 1 }}><TextInput value={name} onChange={setName} placeholder="Device name (e.g. Front Window TV)" /></div>
          <Button onClick={register} disabled={busy}>Register device</Button>
        </div>
        <ErrorText>{error}</ErrorText>
      </Card>

      <Card style={{ padding: 0 }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 14 }}>
          <thead>
            <tr style={{ textAlign: "left", color: "#8a94a3", fontSize: 12 }}>
              <th style={{ padding: "12px 16px" }}>Status</th>
              <th style={{ padding: "12px 16px" }}>Name</th>
              <th style={{ padding: "12px 16px" }}>Key</th>
              <th style={{ padding: "12px 16px" }}>Playlist</th>
              <th style={{ padding: "12px 16px" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {devices.map((d) => (
              <tr key={d.deviceId} style={{ borderTop: "1px solid #eef0f3" }}>
                <td style={{ padding: "12px 16px" }}><StatusPill status={d.status} /></td>
                <td style={{ padding: "12px 16px", fontWeight: 600 }}>{d.deviceName ?? "—"}</td>
                <td style={{ padding: "12px 16px", fontFamily: "monospace" }}>{d.deviceKey}</td>
                <td style={{ padding: "12px 16px" }}>
                  <select
                    value={d.playlistId ?? ""}
                    onChange={(e) => act(() => devicesApi.assignPlaylist(d.deviceId, e.target.value ? Number(e.target.value) : null))}
                    style={{ padding: "6px 8px", borderRadius: 6, border: "1px solid #c4cbd6" }}
                  >
                    <option value="">— none —</option>
                    {playlists.map((p) => <option key={p.playlistId} value={p.playlistId}>{p.name}</option>)}
                  </select>
                </td>
                <td style={{ padding: "12px 16px", display: "flex", gap: 8 }}>
                  <Button variant="ghost" onClick={() => act(() => devicesApi.refresh(d.deviceId))}>Refresh</Button>
                  <Button variant="danger" onClick={() => act(() => devicesApi.revoke(d.deviceId))}>Revoke</Button>
                </td>
              </tr>
            ))}
            {devices.length === 0 && (
              <tr><td colSpan={5} style={{ padding: 16, color: "#8a94a3" }}>No devices yet. Register one, then enter its key in the TV app.</td></tr>
            )}
          </tbody>
        </table>
      </Card>
    </div>
  );
}
