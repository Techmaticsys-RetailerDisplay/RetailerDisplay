import { useEffect, useState } from "react";
import { storesApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Store } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText, StatusPill } from "../../components/ui";

export default function StoresPage() {
  const [stores, setStores] = useState<Store[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [name, setName] = useState("");
  const [tz, setTz] = useState("America/New_York");
  const [creating, setCreating] = useState(false);

  async function load() {
    try { setStores(await storesApi.list()); } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  async function create() {
    if (!name.trim()) return;
    setCreating(true);
    setError(null);
    try {
      await storesApi.create({ storeName: name.trim(), timeZone: tz });
      setName("");
      await load();
    } catch (e) { setError(apiError(e)); } finally { setCreating(false); }
  }

  return (
    <div>
      <PageHeader title="Stores" subtitle="Locations under your account" />
      <Card style={{ marginBottom: 20 }}>
        <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
          <div style={{ flex: 1 }}><TextInput value={name} onChange={setName} placeholder="New store name" /></div>
          <div style={{ width: 200 }}><TextInput value={tz} onChange={setTz} placeholder="Time zone" /></div>
          <Button onClick={create} disabled={creating}>Add store</Button>
        </div>
        <ErrorText>{error}</ErrorText>
      </Card>

      <Card style={{ padding: 0 }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 14 }}>
          <thead>
            <tr style={{ textAlign: "left", color: "#8a94a3", fontSize: 12 }}>
              <th style={{ padding: "12px 16px" }}>Name</th>
              <th style={{ padding: "12px 16px" }}>Code</th>
              <th style={{ padding: "12px 16px" }}>Time zone</th>
              <th style={{ padding: "12px 16px" }}>Status</th>
            </tr>
          </thead>
          <tbody>
            {stores.map((s) => (
              <tr key={s.storeId} style={{ borderTop: "1px solid #eef0f3" }}>
                <td style={{ padding: "12px 16px", fontWeight: 600 }}>{s.storeName}</td>
                <td style={{ padding: "12px 16px" }}>{s.storeCode ?? "—"}</td>
                <td style={{ padding: "12px 16px" }}>{s.timeZone}</td>
                <td style={{ padding: "12px 16px" }}><StatusPill status={s.isActive ? "Online" : "Offline"} /></td>
              </tr>
            ))}
            {stores.length === 0 && (
              <tr><td colSpan={4} style={{ padding: 16, color: "#8a94a3" }}>No stores yet.</td></tr>
            )}
          </tbody>
        </table>
      </Card>
    </div>
  );
}
