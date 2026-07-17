import { useEffect, useRef, useState } from "react";
import { productsApi, storesApi, mediaApi, masterApi, type ProductInput, type MasterProduct } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Store, StoreProduct } from "../../api/types";
import { PageHeader, Card, Button, TextInput, ErrorText, EmptyRow, StatusPill } from "../../components/ui";

const EMPTY: ProductInput = {
  sku: "", productName: "", category: "", brand: "", productType: "", volume: "",
  abv: null, price: 0, salePrice: null, currency: "USD", imageUrl: null,
};

export default function ProductsPage() {
  const [stores, setStores] = useState<Store[]>([]);
  const [storeId, setStoreId] = useState<number | null>(null);
  const [products, setProducts] = useState<StoreProduct[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [note, setNote] = useState<string | null>(null);
  const [editing, setEditing] = useState<{ id: number | null; form: ProductInput } | null>(null);
  const [pulling, setPulling] = useState(false);
  const fileRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    storesApi.list().then((s) => { setStores(s); if (s.length) setStoreId(s[0].storeId); }).catch((e) => setError(apiError(e)));
  }, []);

  async function loadProducts(id = storeId) {
    if (id == null) return;
    try { setProducts((await productsApi.list(id)).items); } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void loadProducts(); /* eslint-disable-next-line */ }, [storeId]);

  async function onImport(file: File) {
    if (storeId == null) return;
    setError(null); setNote(null);
    try {
      const r = await productsApi.import(storeId, file) as { successCount: number; failCount: number };
      setNote(`Imported: ${r.successCount} ok, ${r.failCount} failed.`);
      await loadProducts();
    } catch (e) { setError(apiError(e)); }
  }

  async function act(fn: () => Promise<unknown>) {
    setError(null);
    try { await fn(); await loadProducts(); } catch (e) { setError(apiError(e)); }
  }

  return (
    <div>
      <PageHeader
        title="Products"
        subtitle="Store catalog — add manually, import a CSV, or pull from master"
        action={
          <div className="toolbar">
            <select className="select" style={{ width: 180 }} value={storeId ?? ""} onChange={(e) => setStoreId(Number(e.target.value))}>
              {stores.map((s) => <option key={s.storeId} value={s.storeId}>{s.storeName}</option>)}
            </select>
            <Button onClick={() => setEditing({ id: null, form: { ...EMPTY } })} disabled={storeId == null}>+ Add product</Button>
            <Button variant="ghost" onClick={() => setPulling(true)} disabled={storeId == null}>Pull from master</Button>
            <Button variant="ghost" onClick={() => fileRef.current?.click()} disabled={storeId == null}>Import CSV</Button>
            <input ref={fileRef} type="file" accept=".csv" style={{ display: "none" }}
              onChange={(e) => { const f = e.target.files?.[0]; if (f) void onImport(f); e.target.value = ""; }} />
          </div>
        }
      />
      <ErrorText>{error}</ErrorText>
      {note && <div className="ok-text" style={{ marginBottom: 12 }}>{note}</div>}

      {editing && storeId != null && (
        <div style={{ marginBottom: 18 }}>
          <ProductForm
            storeId={storeId}
            editingId={editing.id}
            initial={editing.form}
            onDone={() => { setEditing(null); void loadProducts(); }}
            onCancel={() => setEditing(null)}
          />
        </div>
      )}

      {pulling && storeId != null && (
        <div style={{ marginBottom: 18 }}>
          <PullFromMaster
            storeId={storeId}
            onDone={(msg) => { setPulling(false); setNote(msg); void loadProducts(); }}
            onCancel={() => setPulling(false)}
          />
        </div>
      )}

      {stores.length === 0 ? (
        <Card><span className="cell-muted">No store found for your account.</span></Card>
      ) : (
        <Card pad={false}>
          <div className="table-wrap">
            <table className="data">
              <thead>
                <tr><th>SKU</th><th>Name</th><th>Price</th><th>Source</th><th>Status</th><th>Actions</th></tr>
              </thead>
              <tbody>
                {products.map((p) => (
                  <tr key={p.storeProductId}>
                    <td className="cell-mono">{p.sku}</td>
                    <td className="cell-strong">{p.productName}</td>
                    <td>{p.currency} {p.price.toFixed(2)}{p.salePrice != null && <span className="cell-muted"> (sale {p.salePrice.toFixed(2)})</span>}</td>
                    <td><span className="tag">{p.source}</span></td>
                    <td><StatusPill status={p.isActive ? "Active" : "Inactive"} /></td>
                    <td>
                      <div className="toolbar">
                        <Button size="sm" variant="ghost" onClick={() => setEditing({ id: p.storeProductId, form: toForm(p) })}>Edit</Button>
                        <Button size="sm" variant="ghost" onClick={() => act(() => productsApi.setActive(p.storeProductId, !p.isActive))}>{p.isActive ? "Deactivate" : "Activate"}</Button>
                        <Button size="sm" variant="danger" onClick={() => act(() => productsApi.remove(p.storeProductId))}>Delete</Button>
                      </div>
                    </td>
                  </tr>
                ))}
                {products.length === 0 && <EmptyRow colSpan={6}>No products yet. Click “+ Add product” or import a CSV.</EmptyRow>}
              </tbody>
            </table>
          </div>
        </Card>
      )}
    </div>
  );
}

function toForm(p: StoreProduct): ProductInput {
  return {
    sku: p.sku, productName: p.productName, price: p.price, salePrice: p.salePrice ?? null,
    currency: p.currency, isActive: p.isActive,
  };
}

function ProductForm({ storeId, editingId, initial, onDone, onCancel }: {
  storeId: number; editingId: number | null; initial: ProductInput; onDone: () => void; onCancel: () => void;
}) {
  const [form, setForm] = useState<ProductInput>(initial);
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  function set<K extends keyof ProductInput>(k: K, v: ProductInput[K]) { setForm((f) => ({ ...f, [k]: v })); }

  async function save() {
    if (!form.productName.trim() || (!editingId && !form.sku?.trim())) {
      setError("SKU and Product name are required."); return;
    }
    setBusy(true); setError(null);
    try {
      let imageUrl = form.imageUrl ?? undefined;
      if (file) imageUrl = (await mediaApi.upload(file, "image")).key;
      const body: ProductInput = { ...form, imageUrl };
      if (editingId) await productsApi.update(editingId, body);
      else await productsApi.create(storeId, body);
      onDone();
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  const num = (v: string) => (v === "" ? null : Number(v));

  return (
    <Card>
      <div style={{ fontWeight: 700, marginBottom: 12 }}>{editingId ? "Edit product" : "New product"}</div>
      <div style={{ display: "grid", gridTemplateColumns: "repeat(3, 1fr)", gap: 12, maxWidth: 760 }}>
        <Field label="SKU *"><TextInput value={form.sku ?? ""} onChange={(v) => set("sku", v)} /></Field>
        <Field label="Product name *"><TextInput value={form.productName} onChange={(v) => set("productName", v)} /></Field>
        <Field label="Category"><TextInput value={form.category ?? ""} onChange={(v) => set("category", v)} /></Field>
        <Field label="Brand"><TextInput value={form.brand ?? ""} onChange={(v) => set("brand", v)} /></Field>
        <Field label="Type (Wine/Beer/…)"><TextInput value={form.productType ?? ""} onChange={(v) => set("productType", v)} /></Field>
        <Field label="Volume (e.g. 750ml)"><TextInput value={form.volume ?? ""} onChange={(v) => set("volume", v)} /></Field>
        <Field label="ABV %"><input className="input" type="number" step="0.01" value={form.abv ?? ""} onChange={(e) => set("abv", num(e.target.value))} /></Field>
        <Field label="Price *"><input className="input" type="number" step="0.01" value={form.price} onChange={(e) => set("price", Number(e.target.value))} /></Field>
        <Field label="Sale price"><input className="input" type="number" step="0.01" value={form.salePrice ?? ""} onChange={(e) => set("salePrice", num(e.target.value))} /></Field>
      </div>
      <div style={{ marginTop: 12 }}>
        <Field label="Product image (optional)"><input type="file" accept="image/png,image/jpeg,image/webp" onChange={(e) => setFile(e.target.files?.[0] ?? null)} /></Field>
      </div>
      <div className="toolbar" style={{ marginTop: 16 }}>
        <Button onClick={save} disabled={busy}>{busy ? "Saving…" : editingId ? "Save changes" : "Create product"}</Button>
        <Button variant="ghost" onClick={onCancel}>Cancel</Button>
      </div>
      <ErrorText>{error}</ErrorText>
    </Card>
  );
}

function Field({ label, children }: { label: string; children: React.ReactNode }) {
  return <div><label className="field-label">{label}</label>{children}</div>;
}

function PullFromMaster({ storeId, onDone, onCancel }: {
  storeId: number; onDone: (msg: string) => void; onCancel: () => void;
}) {
  const [items, setItems] = useState<MasterProduct[]>([]);
  const [selected, setSelected] = useState<Set<number>>(new Set());
  const [error, setError] = useState<string | null>(null);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    masterApi.list().then((r) => setItems(r.items)).catch((e) => setError(apiError(e)));
  }, []);

  function toggle(id: number) {
    setSelected((s) => { const n = new Set(s); n.has(id) ? n.delete(id) : n.add(id); return n; });
  }

  async function pull() {
    if (selected.size === 0) return;
    setBusy(true); setError(null);
    try {
      const r = await productsApi.pull(storeId, [...selected]) as { pulled: number };
      onDone(`Pulled ${r.pulled} product(s) from master.`);
    } catch (e) { setError(apiError(e)); } finally { setBusy(false); }
  }

  return (
    <Card>
      <div style={{ fontWeight: 700, marginBottom: 12 }}>Pull from master catalog</div>
      <div style={{ maxHeight: 260, overflowY: "auto", border: "1px solid var(--border)", borderRadius: 8, padding: 10 }}>
        {items.map((m) => (
          <label key={m.masterProductId} style={{ display: "flex", gap: 8, alignItems: "center", padding: "5px 4px", cursor: "pointer" }}>
            <input type="checkbox" checked={selected.has(m.masterProductId)} onChange={() => toggle(m.masterProductId)} />
            <span className="cell-strong">{m.productName}</span>
            <span className="cell-muted">{m.brand ? `— ${m.brand}` : ""} {m.volume ?? ""} {m.sku ? `(${m.sku})` : ""}</span>
          </label>
        ))}
        {items.length === 0 && <div className="cell-muted" style={{ padding: 6 }}>Master catalog is empty. An admin needs to import products first.</div>}
      </div>
      <div className="toolbar" style={{ marginTop: 12 }}>
        <Button onClick={pull} disabled={busy || selected.size === 0}>{busy ? "Pulling…" : `Pull selected (${selected.size})`}</Button>
        <Button variant="ghost" onClick={onCancel}>Cancel</Button>
      </div>
      <div className="hint" style={{ marginTop: 6 }}>Pulled products land in your store with price 0 — set the price after pulling.</div>
      <ErrorText>{error}</ErrorText>
    </Card>
  );
}
