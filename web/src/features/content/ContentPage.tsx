import { useEffect, useState } from "react";
import { contentApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { ContentItem } from "../../api/types";
import { PageHeader, Card, Button, ErrorText } from "../../components/ui";

export default function ContentPage() {
  const [items, setItems] = useState<ContentItem[]>([]);
  const [error, setError] = useState<string | null>(null);

  async function load() {
    try { setItems(await contentApi.list()); } catch (e) { setError(apiError(e)); }
  }
  useEffect(() => { void load(); }, []);

  return (
    <div>
      <PageHeader title="Content" subtitle="Images, videos and product lists" />
      <ErrorText>{error}</ErrorText>

      <Card style={{ padding: 0 }}>
        <table style={{ width: "100%", borderCollapse: "collapse", fontSize: 14 }}>
          <thead>
            <tr style={{ textAlign: "left", color: "#8a94a3", fontSize: 12 }}>
              <th style={{ padding: "12px 16px" }}>Name</th>
              <th style={{ padding: "12px 16px" }}>Type</th>
              <th style={{ padding: "12px 16px" }}>Version</th>
              <th style={{ padding: "12px 16px" }}></th>
            </tr>
          </thead>
          <tbody>
            {items.map((c) => (
              <tr key={c.contentId} style={{ borderTop: "1px solid #eef0f3" }}>
                <td style={{ padding: "12px 16px", fontWeight: 600 }}>{c.name}</td>
                <td style={{ padding: "12px 16px", color: "#616b7a" }}>{c.contentType}</td>
                <td style={{ padding: "12px 16px" }}>v{c.version}</td>
                <td style={{ padding: "12px 16px" }}>
                  <Button variant="danger" onClick={() => contentApi.remove(c.contentId).then(load).catch((e) => setError(apiError(e)))}>Delete</Button>
                </td>
              </tr>
            ))}
            {items.length === 0 && (
              <tr><td colSpan={4} style={{ padding: 16, color: "#8a94a3" }}>No content yet.</td></tr>
            )}
          </tbody>
        </table>
      </Card>
      <p style={{ color: "#8a94a3", fontSize: 13, marginTop: 20 }}>
        Upload UI (presigned <code>POST /media/upload-url</code> → <code>POST /content</code> with WebP
        rendition generation) and the product-list picker wire here — next increment.
      </p>
    </div>
  );
}
