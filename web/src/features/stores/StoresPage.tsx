import { useEffect, useState } from "react";
import { storesApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { Store } from "../../api/types";
import { PageHeader, Card, ErrorText, StatusPill, EmptyRow } from "../../components/ui";

export default function StoresPage() {
  const [stores, setStores] = useState<Store[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    storesApi.list().then(setStores).catch((e) => setError(apiError(e)));
  }, []);

  return (
    <div>
      <PageHeader title="Stores" subtitle="Your locations — provisioned for your account" />
      <ErrorText>{error}</ErrorText>
      <Card pad={false}>
        <div className="table-wrap">
          <table className="data">
            <thead>
              <tr><th>Name</th><th>Code</th><th>Time zone</th><th>Status</th></tr>
            </thead>
            <tbody>
              {stores.map((s) => (
                <tr key={s.storeId}>
                  <td className="cell-strong">{s.storeName}</td>
                  <td className="cell-muted">{s.storeCode ?? "—"}</td>
                  <td className="cell-muted">{s.timeZone}</td>
                  <td><StatusPill status={s.isActive ? "Active" : "Inactive"} /></td>
                </tr>
              ))}
              {stores.length === 0 && <EmptyRow colSpan={4}>No stores yet.</EmptyRow>}
            </tbody>
          </table>
        </div>
      </Card>
    </div>
  );
}
