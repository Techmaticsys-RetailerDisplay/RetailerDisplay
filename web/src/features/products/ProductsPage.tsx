import { useEffect, useRef, useState } from "react";
import { productsApi, storesApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Store, StoreProduct } from "../../api/types";
import { PageHeader, Card, Button, ErrorText } from "../../components/ui";

export default function ProductsPage() {
  const [stores, setStores] = useState<Store[]>([]);
  const [storeId, setStoreId] = useState<number | null>(null);
  const [products, setProducts] = useState<StoreProduct[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [note, setNote] = useState<string | null>(null);
  const fileRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    storesApi.list().then((s) => {
      setStores(s);
      if (s.length) setStoreId(s[0].storeId);
    }).catch((e) => setError(apiError(e)));
  }, []);

  useEffect(() => {
    if (storeId == null) return;
    productsApi.list(storeId).then((p) => setProducts(p.items)).catch((e) => setError(apiError(e)));
  }, [storeId]);

  async function onImport(file: File) {
    if (storeId == null) return;
    setError(null); setNote(null);
    try {
      const result = await productsApi.import(storeId, file) as { successCount: number; failCount: number };
      setNote(`Imported: ${result.successCount} ok, ${result.failCount} failed.`);
      const p = await productsApi.list(storeId);
      setProducts(p.items);
    } catch (e) { setError(apiError(e)); }
  }

  return (
    <div>
      <PageHeader
        title="Products"
        subtitle="Store catalog — import via CSV or pull from master"
        action={
          <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
            <select value={storeId ?? ""} onChange={(e) => setStoreId(Number(e.target.value))}
              style={{ padding: "8px 10px", borderRadius: 8, border: "1px solid #c4cbd6" }}>
              {stores.map((s) => <option key={s.storeId} value={s.storeId}>{s.storeName}</option>)}
            </select>
            <Button onClick={() => fileRef.current?.click()} disabled={storeId == null}>Import CSV</Button>
            <input ref={fileRef} type="file" accept=".csv" style={{ display: "none" }}
              onChange={(e) => { const f = e.target.files?.[0]; if (f) void onImport(f); e.target.value = ""; }} />
          </div>
        }
      />
      <ErrorText>{error}</ErrorText>
      {note && <div style={{ color: "#1f9d57", fontSize: 13, marginBottom: 12 }}>{note}</div>}

      {stores.length === 0 ? (
        <Card><span style={{ color: "#8a94a3" }}>Create a store first, then import products into it.</span></Card>
      ) : (
        <Card style={{ padding: 0 }}>
          <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 14 }}>
            <thead>
              <tr style={{ textAlign: "left", color: "#8a94a3", fontSize: 12 }}>
                <th style={{ padding: "12px 16px" }}>SKU</th>
                <th style={{ padding: "12px 16px" }}>Name</th>
                <th style={{ padding: "12px 16px" }}>Price</th>
                <th style={{ padding: "12px 16px" }}>Source</th>
              </tr>
            </thead>
            <tbody>
              {products.map((p) => (
                <tr key={p.storeProductId} style={{ borderTop: "1px solid #eef0f3" }}>
                  <td style={{ padding: "12px 16px", fontFamily: "monospace" }}>{p.sku}</td>
                  <td style={{ padding: "12px 16px", fontWeight: 600 }}>{p.productName}</td>
                  <td style={{ padding: "12px 16px" }}>{p.currency} {p.price.toFixed(2)}</td>
                  <td style={{ padding: "12px 16px", color: "#616b7a" }}>{p.source}</td>
                </tr>
              ))}
              {products.length === 0 && (
                <tr><td colSpan={4} style={{ padding: 16, color: "#8a94a3" }}>No products in this store yet.</td></tr>
              )}
            </tbody>
          </table>
        </Card>
      )}
    </div>
  );
}
