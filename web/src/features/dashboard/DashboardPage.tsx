import { useEffect, useState } from "react";
import { dashboardApi } from "../../api/endpoints";
import { apiError } from "../../api/client";
import type { DashboardSummary } from "../../api/types";
import { PageHeader, StatCard, ErrorText } from "../../components/ui";
import { IconOnline, IconOffline, IconDevices, IconContent, IconPlaylist, IconProducts } from "../../components/icons";

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
        <div className="stat-grid">
          <StatCard label="Devices online" value={summary.devicesOnline} tone="good" icon={<IconOnline />} />
          <StatCard label="Devices offline" value={summary.devicesOffline} tone="danger" icon={<IconOffline />} />
          <StatCard label="Total devices" value={summary.totalDevices} icon={<IconDevices />} />
          <StatCard label="Content items" value={summary.totalContent} icon={<IconContent />} />
          <StatCard label="Active playlists" value={summary.activePlaylists} icon={<IconPlaylist />} />
          <StatCard label="Products" value={summary.totalProducts} icon={<IconProducts />} />
        </div>
      )}
      {summary?.lastImportAt && (
        <p className="hint">Last product import: {new Date(summary.lastImportAt).toLocaleString()}</p>
      )}
    </div>
  );
}
