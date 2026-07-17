import { BrowserRouter, Routes, Route, Navigate, useLocation } from "react-router-dom";
import { AuthProvider, useAuth } from "./auth/AuthContext";
import Layout from "./components/Layout";
import LoginPage from "./features/auth/LoginPage";
import AdminApp from "./features/admin/AdminApp";
import DashboardPage from "./features/dashboard/DashboardPage";
import ContentPage from "./features/content/ContentPage";
import PlaylistsPage from "./features/playlists/PlaylistsPage";
import PlaylistEditorPage from "./features/playlists/PlaylistEditorPage";
import DevicesPage from "./features/devices/DevicesPage";
import ProductsPage from "./features/products/ProductsPage";
import StoresPage from "./features/stores/StoresPage";
import ProfilePage from "./features/profile/ProfilePage";

function Protected({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, loading, retailer } = useAuth();
  const location = useLocation();
  if (loading) return <div style={{ padding: 40, fontFamily: "system-ui" }}>Loading…</div>;
  if (!isAuthenticated) return <Navigate to="/login" replace />;
  // Force profile completion before using the rest of the app.
  if (retailer && !retailer.profileCompleted && location.pathname !== "/profile") {
    return <Navigate to="/profile" replace />;
  }
  return <>{children}</>;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/admin" element={<AdminApp />} />
          <Route
            element={
              <Protected>
                <Layout />
              </Protected>
            }
          >
            <Route index element={<DashboardPage />} />
            <Route path="profile" element={<ProfilePage />} />
            <Route path="content" element={<ContentPage />} />
            <Route path="playlists" element={<PlaylistsPage />} />
            <Route path="playlists/:id" element={<PlaylistEditorPage />} />
            <Route path="devices" element={<DevicesPage />} />
            <Route path="products" element={<ProductsPage />} />
            <Route path="stores" element={<StoresPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
