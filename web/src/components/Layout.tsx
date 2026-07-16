import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

const nav = [
  { to: "/", label: "Dashboard", end: true },
  { to: "/content", label: "Content" },
  { to: "/playlists", label: "Playlists" },
  { to: "/devices", label: "Devices" },
  { to: "/products", label: "Products" },
  { to: "/stores", label: "Stores" },
];

export default function Layout() {
  const { retailer, logout } = useAuth();
  return (
    <div style={{ display: "flex", minHeight: "100vh", fontFamily: "system-ui, sans-serif" }}>
      <aside
        style={{
          width: 220,
          background: "#141a24",
          color: "#eef1f6",
          padding: "24px 16px",
          flexShrink: 0,
          display: "flex",
          flexDirection: "column",
        }}
      >
        <div style={{ fontWeight: 800, fontSize: 18, marginBottom: 24, letterSpacing: "-0.01em" }}>
          Retailer<span style={{ color: "#f2a13d" }}>Display</span>
        </div>
        <nav style={{ display: "flex", flexDirection: "column", gap: 4 }}>
          {nav.map((n) => (
            <NavLink
              key={n.to}
              to={n.to}
              end={n.end}
              style={({ isActive }) => ({
                padding: "9px 12px",
                borderRadius: 8,
                textDecoration: "none",
                color: isActive ? "#141a24" : "#c7cedb",
                background: isActive ? "#f2a13d" : "transparent",
                fontWeight: isActive ? 600 : 500,
                fontSize: 14,
              })}
            >
              {n.label}
            </NavLink>
          ))}
        </nav>
        <div style={{ marginTop: "auto", fontSize: 13, color: "#c7cedb" }}>
          <div style={{ marginBottom: 8, opacity: 0.8 }}>{retailer?.businessName}</div>
          <button
            onClick={() => void logout()}
            style={{ background: "transparent", color: "#c7cedb", border: "1px solid #35404f", borderRadius: 8, padding: "7px 12px", fontSize: 13, cursor: "pointer", width: "100%" }}
          >
            Sign out
          </button>
        </div>
      </aside>
      <main style={{ flex: 1, padding: "32px 40px", background: "#eceff3", color: "#10151f" }}>
        <Outlet />
      </main>
    </div>
  );
}
