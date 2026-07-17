import { NavLink, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";
import { IconDashboard, IconContent, IconPlaylist, IconDevices, IconProducts, IconStores } from "./icons";

const nav = [
  { to: "/", label: "Dashboard", end: true, icon: <IconDashboard /> },
  { to: "/content", label: "Content", icon: <IconContent /> },
  { to: "/playlists", label: "Playlists", icon: <IconPlaylist /> },
  { to: "/devices", label: "Devices", icon: <IconDevices /> },
  { to: "/products", label: "Products", icon: <IconProducts /> },
  { to: "/stores", label: "Stores", icon: <IconStores /> },
  { to: "/profile", label: "Profile", icon: <IconStores /> },
];

export default function Layout() {
  const { retailer, logout } = useAuth();
  const initial = retailer?.businessName?.trim().charAt(0).toUpperCase() || "R";

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <span className="dot">▦</span>
          <span>Retailer<span className="accent">Display</span></span>
        </div>
        <nav className="nav">
          {nav.map((n) => (
            <NavLink key={n.to} to={n.to} end={n.end} className={({ isActive }) => `nav-link ${isActive ? "active" : ""}`}>
              {n.icon}
              <span>{n.label}</span>
            </NavLink>
          ))}
        </nav>
        <div className="sidebar-footer">
          <div className="sidebar-account">
            {retailer?.businessName}
            <small>{retailer?.email}</small>
          </div>
          <button className="btn-signout" onClick={() => void logout()}>Sign out ({initial})</button>
        </div>
      </aside>
      <main className="main">
        <div className="main-inner">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
