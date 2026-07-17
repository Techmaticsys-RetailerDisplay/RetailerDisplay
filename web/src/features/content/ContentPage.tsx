import { useEffect, useState } from "react";
import { contentApi, mediaApi, storesApi, productsApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { ContentItem, Store, StoreProduct } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText, EmptyRow, StatusPill } from "../../components/ui";

type Draft = "image" | "video" | "productlist";

export default function ContentPage() {
  const [items, setItems] = useState<ContentItem[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [draft, setDraft] = useState<Draft | null>(null);

  async function load() {
    try { setItems(await contentApi.list()); } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  return (
    <div>
      <PageHeader
        title="Content"
        subtitle="Images, videos and product lists"
        action={
          <div className="toolbar">
            <Button variant={draft === "image" ? "primary" : "ghost"} onClick={() => setDraft("image")}>+ Image</Button>
            <Button variant={draft === "video" ? "primary" : "ghost"} onClick={() => setDraft("video")}>+ Video</Button>
            <Button variant={draft === "productlist" ? "primary" : "ghost"} onClick={() => setDraft("productlist")}>+ Product list</Button>
          </div>
        }
      />
      <ErrorText>{error}</ErrorText>

      {draft && (
        <div style={{ marginBottom: 18 }}>
          {draft === "productlist"
            ? <ProductListForm onDone={() => { setDraft(null); void load(); }} onCancel={() => setDraft(null)} />
            : <MediaForm kind={draft} onDone={() => { setDraft(null); void load(); }} onCancel={() => setDraft(null)} />}
        </div>
      )}

      <Card pad={false}>
        <div className="table-wrap">
          <table className="data">
            <thead><tr><th>Preview</th><th>Name</th><th>Type</th><th>Version</th><th>Status</th><th></th></tr></thead>
            <tbody>
              {items.map((c) => (
                <tr key={c.contentId}>
                  <td><ContentThumb item={c} /></td>
                  <td className="cell-strong">{c.name}</td>
                  <td><span className="tag">{c.contentType}</span></td>
                  <td className="cell-muted">v{c.version}</td>
                  <td><StatusPill status={c.isActive ? "Active" : "Inactive"} /></td>
                  <td><Button size="sm" variant="danger" onClick={() => contentApi.remove(c.contentId).then(load).catch((e) => setError(apiError(e)))}>Delete</Button></td>
                </tr>
              ))}
              {items.length === 0 && <EmptyRow colSpan={6}>No content yet. Use the buttons above to add some.</EmptyRow>}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
}

function ContentThumb({ item }: { item: ContentItem }) {
  const box: React.CSSProperties = {
    width: 72, height: 46, borderRadius: 6, border: "1px solid var(--border)",
    overflow: "hidden", display: "grid", placeItems: "center",
    background: "var(--surface-3)", color: "var(--faint)", fontSize: 18,
  };
  if (item.contentType === "Image" && item.thumbnailUrl) {
    return (
      <a href={item.previewUrl ?? item.thumbnailUrl} target="_blank" rel="noreferrer" title="View full size">
        <img src={item.thumbnailUrl} alt={item.name} style={{ ...box, objectFit: "cover", display: "block" }} />
      </a>
    );
  }
  if (item.contentType === "Video") {
    return item.previewUrl
      ? <a href={item.previewUrl} target="_blank" rel="noreferrer" title="Play video" style={{ ...box, textDecoration: "none" }}>▶</a>
      : <div style={box}>▶</div>;
  }
  return <div style={box}>▦</div>; // product list
}

function MediaForm({ kind, onDone, onCancel }: { kind: "image" | "video"; onDone: () => void; onCancel: () => void }) {
  const [name, setName] = useState("");
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  async function save() {
    if (!name.trim() || !file) { setError("Name and file are required."); return; }
    setBusy(true); setError(null);
    try {
      const uploaded = await mediaApi.upload(file, kind);
      if (kind === "image") await contentApi.createImage(name.trim(), uploaded.key, uploaded.sizeBytes);
      else await contentApi.createVideo(name.trim(), uploaded.key, uploaded.sizeBytes);
      onDone();
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  return (
    <Card>
      <div style={{ fontWeight: 700, marginBottom: 12 }}>New {kind}</div>
      <div className="toolbar" style={{ alignItems: "flex-end", flexWrap: "wrap" }}>
        <div className="grow" style={{ minWidth: 200 }}>
          <label className="field-label">Name</label>
          <TextInput value={name} onChange={setName} placeholder={`${kind} name`} />
        </div>
        <div className="grow" style={{ minWidth: 220 }}>
          <label className="field-label">File {kind === "image" ? "(JPEG/PNG/WebP, ≤2 MB)" : "(MP4, ≤20 MB)"}</label>
          <input type="file" accept={kind === "image" ? "image/png,image/jpeg,image/webp" : "video/mp4"}
            onChange={(e) => setFile(e.target.files?.[0] ?? null)} />
        </div>
        <Button onClick={save} disabled={busy}>{busy ? "Uploading…" : "Create"}</Button>
        <Button variant="ghost" onClick={onCancel}>Cancel</Button>
      </div>
      <ErrorText>{error}</ErrorText>
    </Card>
  );
}

function ProductListForm({ onDone, onCancel }: { onDone: () => void; onCancel: () => void }) {
  const [name, setName] = useState("");
  const [stores, setStores] = useState<Store[]>([]);
  const [storeId, setStoreId] = useState<number | null>(null);
  const [products, setProducts] = useState<StoreProduct[]>([]);
  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    storesApi.list().then((s) => { setStores(s); if (s.length) setStoreId(s[0].storeId); }).catch((e) => setError(apiError(e)));
  }, []);
  useEffect(() => {
    if (storeId == null) return;
    productsApi.list(storeId, "", true).then((p) => setProducts(p.items)).catch((e) => setError(apiError(e)));
  }, [storeId]);

  function toggle(id: number) {
    setSelected((s) => { const n = new Set(s); n.has(id) ? n.delete(id) : n.add(id); return n; });
  }

  async function save() {
    if (!name.trim() || selected.size === 0) { setError("Name and at least one product are required."); return; }
    setBusy(true); setError(null);
    try {
      await contentApi.createProductList(name.trim(), [...selected]);
      onDone();
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  return (
    <Card>
      <div style={{ fontWeight: 700, marginBottom: 12 }}>New product list</div>
      <div className="toolbar" style={{ marginBottom: 12 }}>
        <div className="grow"><TextInput value={name} onChange={setName} placeholder="Product list name" /></div>
        <select className="select" style={{ width: 200 }} value={storeId ?? ""} onChange={(e) => setStoreId(Number(e.target.value))}>
          {stores.map((s) => <option key={s.storeId} value={s.storeId}>{s.storeName}</option>)}
        </select>
      </div>
      <div style={{ maxHeight: 220, overflowY: "auto", border: "1px solid var(--border)", borderRadius: 8, padding: 10 }}>
        {products.map((p) => (
          <label key={p.storeProductId} style={{ display: "flex", gap: 8, alignItems: "center", padding: "5px 4px", cursor: "pointer" }}>
            <input type="checkbox" checked={selected.has(p.storeProductId)} onChange={() => toggle(p.storeProductId)} />
            <span className="cell-strong">{p.productName}</span>
            <span className="cell-muted">— {p.currency} {p.price.toFixed(2)}</span>
          </label>
        ))}
        {products.length === 0 && <div className="cell-muted" style={{ padding: 6 }}>No active products in this store. Add products first.</div>}
      </div>
      <div className="toolbar" style={{ marginTop: 12 }}>
        <Button onClick={save} disabled={busy}>{busy ? "Creating…" : `Create (${selected.size})`}</Button>
        <Button variant="ghost" onClick={onCancel}>Cancel</Button>
      </div>
      <ErrorText>{error}</ErrorText>
    </Card>
  );
}
