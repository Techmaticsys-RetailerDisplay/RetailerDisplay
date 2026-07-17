/* Minimal stroke icons (currentColor). */
type P = { size?: number };
const base = (size = 18) => ({
  width: size, height: size, viewBox: "0 0 24 24", fill: "none",
  stroke: "currentColor", strokeWidth: 1.8, strokeLinecap: "round" as const, strokeLinejoin: "round" as const,
});

export const IconDashboard = ({ size }: P) => (
  <svg {...base(size)}><rect x="3" y="3" width="7" height="9" rx="1.5" /><rect x="14" y="3" width="7" height="5" rx="1.5" /><rect x="14" y="12" width="7" height="9" rx="1.5" /><rect x="3" y="16" width="7" height="5" rx="1.5" /></svg>
);
export const IconContent = ({ size }: P) => (
  <svg {...base(size)}><rect x="3" y="4" width="18" height="14" rx="2" /><path d="m3 15 5-4 4 3 3-2 6 4" /><circle cx="8.5" cy="8.5" r="1.5" /></svg>
);
export const IconPlaylist = ({ size }: P) => (
  <svg {...base(size)}><path d="M4 6h11M4 12h11M4 18h7" /><path d="m17 14 5 3-5 3z" /></svg>
);
export const IconDevices = ({ size }: P) => (
  <svg {...base(size)}><rect x="2" y="4" width="14" height="10" rx="1.5" /><path d="M6 18h6" /><rect x="17" y="9" width="5" height="11" rx="1.5" /></svg>
);
export const IconProducts = ({ size }: P) => (
  <svg {...base(size)}><path d="M3 7 12 3l9 4-9 4-9-4Z" /><path d="M3 7v10l9 4 9-4V7" /><path d="M12 11v10" /></svg>
);
export const IconStores = ({ size }: P) => (
  <svg {...base(size)}><path d="M4 9h16l-1-4H5L4 9Z" /><path d="M5 9v11h14V9" /><path d="M9 20v-6h6v6" /></svg>
);
export const IconOnline = ({ size }: P) => (
  <svg {...base(size)}><path d="M5 12.5 10 17l9-10" /></svg>
);
export const IconOffline = ({ size }: P) => (
  <svg {...base(size)}><path d="M6 6l12 12M18 6 6 18" /></svg>
);
