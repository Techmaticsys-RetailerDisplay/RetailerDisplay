import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import { AuthProvider, useAuth } from "./auth/AuthContext";
import Layout from "./components/Layout";
import LoginPage from "./features/auth/LoginPage";
import DashboardPage from "./features/dashboard/DashboardPage";
import ContentPage from "./features/content/ContentPage";
import PlaylistsPage from "./features/playlists/PlaylistsPage";
import DevicesPage from "./features/devices/DevicesPage";
import ProductsPage from "./features/products/ProductsPage";
import StoresPage from "./features/stores/StoresPage";

function Protected({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, loading } = useAuth();
  if (loading) return <div style={{ padding: 40, fontFamily: "system-ui" }}>Loading…</div>;
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route
            element={
              <Protected>
                <Layout />
              </Protected>
            }
          >
            <Route index element={<DashboardPage />} />
            <Route path="content" element={<ContentPage />} />
            <Route path="playlists" element={<PlaylistsPage />} />
            <Route path="devices" element={<DevicesPage />} />
            <Route path="products" element={<ProductsPage />} />
            <Route path="stores" element={<StoresPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
