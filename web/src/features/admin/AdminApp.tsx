import { useEffect, useRef, useState } from "react";
import { adminApi, adminToken, type PlatformOverview, type RetailerUsage, type MasterProduct } from "../../api/adminClient";
import { apiError } from "../../api/client";
import { StatCard, Button, TextInput, ErrorText, EmptyRow, StatusPill } from "../../components/ui";
import { IconStores, IconDevices, IconProducts, IconPlaylist, IconContent, IconOnline } from "../../components/icons";

export default function AdminApp() {
  const [authed, setAuthed] = useState(!!adminToken.get());
  return authed ? <AdminConsole onSignOut={() => setAuthed(false)} /> : <AdminLogin onDone={() => setAuthed(true)} />;
}

function AdminLogin({ onDone }: { onDone: () => void }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError(null); setBusy(true);
    try {
      const res = await adminApi.login(email, password);
      adminToken.set(res.accessToken);
      onDone();
    } catch (err) { setError(apiError(err)); } finally { setBusy(false); }
  }

  return (
    <div className="auth-wrap">
      <form className="auth-card" onSubmit={submit}>
        <div className="auth-brand">
          <span className="dot">▦</span>
          <span>Retailer<span className="accent">Display</span></span>
        </div>
        <p className="auth-sub">Admin console — platform oversight</p>
        <div className="stack">
          <TextInput value={email} onChange={setEmail} placeholder="Admin email" type="email" />
          <TextInput value={password} onChange={setPassword} placeholder="Password" type="password" />
          <Button type="submit" disabled={busy}>{busy ? "Signing in…" : "Sign in"}</Button>
          <ErrorText>{error}</ErrorText>
        </div>
      </form>
    </div>
  );
}

function AdminConsole({ onSignOut }: { onSignOut: () => void }) {
  const [overview, setOverview] = useState<PlatformOverview | null>(null);
  const [retailers, setRetailers] = useState<RetailerUsage[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [note, setNote] = useState<string | null>(null);
  const [biz, setBiz] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [busy, setBusy] = useState(false);
  const [master, setMaster] = useState<MasterProduct[]>([]);
  const masterFileRef = useRef<HTMLInputElement>(null);

  async function load() {
    try {
      setOverview(await adminApi.overview());
      setRetailers(await adminApi.retailers());
      setMaster((await adminApi.listMaster()).items);
    } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  async function importMaster(file: File) {
    setError(null); setNote(null);
    try {
      const r = await adminApi.importMaster(file);
      setNote(`Master import: ${r.successCount} ok, ${r.failCount} failed (of ${r.totalRows}).`);
      await load();
    } catch (e) { setError(apiError(e)); }
  }

  async function createRetailer() {
    if (!biz.trim() || !email.trim() || !password) return;
    setBusy(true); setError(null); setNote(null);
    try {
      await adminApi.createRetailer({ businessName: biz.trim(), email: email.trim(), password });
      setNote(`Retailer "${biz.trim()}" created. They can now sign in and complete their profile.`);
      setBiz(""); setEmail(""); setPassword("");
      await load();
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  function signOut() { adminToken.clear(); onSignOut(); }

  return (
    <div>
      <div style={{
        background: "var(--sidebar)", color: "#fff", padding: "14px 28px",
        display: "flex", alignItems: "center", justifyContent: "space-between",
      }}>
        <div className="auth-brand" style={{ fontSize: 17 }}>
          <span className="dot">▦</span>
          <span>Retailer<span className="accent">Display</span> <span style={{ color: "var(--sidebar-faint)", fontWeight: 600 }}>Admin</span></span>
        </div>
        <button className="btn-signout" style={{ width: "auto" }} onClick={signOut}>Sign out</button>
      </div>

      <div className="main-inner">
        <h1 className="page-title">Platform overview</h1>
        <div className="page-subtitle" style={{ marginBottom: 22 }}>Usage across all retailers on RetailerDisplay</div>
        <ErrorText>{error}</ErrorText>

        {overview && (
          <div className="stat-grid">
            <StatCard label="Retailers" value={overview.totalRetailers} icon={<IconStores />} />
            <StatCard label="Stores" value={overview.totalStores} icon={<IconStores />} />
            <StatCard label="Devices online" value={overview.devicesOnline} tone="good" icon={<IconOnline />} />
            <StatCard label="Total devices" value={overview.totalDevices} icon={<IconDevices />} />
            <StatCard label="Products" value={overview.totalProducts} icon={<IconProducts />} />
            <StatCard label="Playlists" value={overview.totalPlaylists} icon={<IconPlaylist />} />
            <StatCard label="Content" value={overview.totalContent} icon={<IconContent />} />
          </div>
        )}

        <h2 className="page-title" style={{ fontSize: 19, margin: "30px 0 14px" }}>Create retailer</h2>
        <div className="card card-pad">
          <div className="toolbar">
            <div className="grow"><TextInput value={biz} onChange={setBiz} placeholder="Business name" /></div>
            <div className="grow"><TextInput value={email} onChange={setEmail} placeholder="Email" type="email" /></div>
            <div style={{ width: 180 }}><TextInput value={password} onChange={setPassword} placeholder="Temp password" type="text" /></div>
            <Button onClick={createRetailer} disabled={busy}>Create</Button>
          </div>
          {note && <div className="ok-text">{note}</div>}
          <ErrorText>{error}</ErrorText>
        </div>

        <h2 className="page-title" style={{ fontSize: 19, margin: "30px 0 14px" }}>Retailers</h2>
        <div className="card" style={{ padding: 0 }}>
          <div className="table-wrap">
            <table className="data">
              <thead>
                <tr><th>Business</th><th>Email</th><th>Stores</th><th>Devices</th><th>Products</th><th>Status</th><th>Joined</th></tr>
              </thead>
              <tbody>
                {retailers.map((r) => (
                  <tr key={r.retailerId}>
                    <td className="cell-strong">{r.businessName}</td>
                    <td className="cell-muted">{r.email}</td>
                    <td>{r.storeCount}</td>
                    <td>{r.deviceCount}</td>
                    <td>{r.productCount}</td>
                    <td><StatusPill status={r.isActive ? "Active" : "Inactive"} /></td>
                    <td className="cell-muted">{new Date(r.createdAt).toLocaleDateString()}</td>
                  </tr>
                ))}
                {retailers.length === 0 && <EmptyRow colSpan={7}>No retailers yet.</EmptyRow>}
              </tbody>
            </table>
          </div>
        </div>

        <div style={{ display: "flex", alignItems: "center", justifyContent: "space-between", margin: "30px 0 14px" }}>
          <h2 className="page-title" style={{ fontSize: 19, margin: 0 }}>Master catalog</h2>
          <div>
            <Button onClick={() => masterFileRef.current?.click()}>Import CSV to master</Button>
            <input ref={masterFileRef} type="file" accept=".csv" style={{ display: "none" }}
              onChange={(e) => { const f = e.target.files?.[0]; if (f) void importMaster(f); e.target.value = ""; }} />
          </div>
        </div>
        <div style={{ color: "var(--faint)", fontSize: 13, marginBottom: 10 }}>
          CSV headers: <code>Sku, ProductName, Description, Category, Brand, ProductType, Abv, ContainerType, Volume, PackSize, Vintage</code> (Price not needed for master). Retailers pull these into their stores.
        </div>
        <div className="card" style={{ padding: 0 }}>
          <div className="table-wrap">
            <table className="data">
              <thead><tr><th>SKU</th><th>Product</th><th>Brand</th><th>Type</th><th>Volume</th></tr></thead>
              <tbody>
                {master.map((m) => (
                  <tr key={m.masterProductId}>
                    <td className="cell-mono">{m.sku ?? "—"}</td>
                    <td className="cell-strong">{m.productName}</td>
                    <td className="cell-muted">{m.brand ?? "—"}</td>
                    <td className="cell-muted">{m.productType ?? "—"}</td>
                    <td className="cell-muted">{m.volume ?? "—"}</td>
                  </tr>
                ))}
                {master.length === 0 && <EmptyRow colSpan={5}>Master catalog is empty. Import a CSV above.</EmptyRow>}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}
