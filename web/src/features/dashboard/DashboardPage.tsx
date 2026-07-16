import { useEffect, useState } from "react";
import { dashboardApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { DashboardSummary } from "../../api/types";
import { PageHeader, StatCard, ErrorText } from "../../components/ui";

export default function DashboardPage() {
  const [summary, setSummary] = useState<DashboardSummary | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    dashboardApi.summary().then(setSummary).catch((e) => setError(apiError(e)));
  }, []);

  return (
    <div>
      <PageHeader title="Dashboard" subtitle="Fleet health and content overview" />
      <ErrorText>{error}</ErrorText>
      {summary && (
        <div style={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(160px, 1fr))", gap: 14 }}>
          <StatCard label="Devices online" value={summary.devicesOnline} accent="#1f9d57" />
          <StatCard label="Devices offline" value={summary.devicesOffline} accent="#d13a3a" />
          <StatCard label="Total devices" value={summary.totalDevices} />
          <StatCard label="Content items" value={summary.totalContent} />
          <StatCard label="Active playlists" value={summary.activePlaylists} />
          <StatCard label="Products" value={summary.totalProducts} />
        </div>
      )}
      {summary?.lastImportAt && (
        <p style={{ color: "#616b7a", fontSize: 13, marginTop: 16 }}>
          Last product import: {new Date(summary.lastImportAt).toLocaleString()}
        </p>
      )}
    </div>
  );
}
