import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from "react";
import { tokenStore } from "../api/client";
import { authApi } from "../api/endpoints";
import type { RetailerProfile } from "../api/types";

interface AuthState {
  retailer: RetailerProfile | null;
  isAuthenticated: boolean;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
}

const AuthContext = createContext<AuthState | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [retailer, setRetailer] = useState<RetailerProfile | null>(null);
  const [loading, setLoading] = useState(true);

  // Restore the session on load if a refresh token is present.
  useEffect(() => {
    let active = true;
    (async () => {
      if (tokenStore.refresh) {
        try {
          const me = await authApi.me();
          if (active) setRetailer(me);
        } catch {
          tokenStore.clear();
        }
      }
      if (active) setLoading(false);
    })();
    return () => {
      active = false;
    };
  }, []);

  const value = useMemo<AuthState>(
    () => ({
      retailer,
      isAuthenticated: retailer !== null,
      loading,
      login: async (email, password) => {
        const res = await authApi.login(email, password);
        tokenStore.set(res.accessToken, res.refreshToken);
        setRetailer(res.retailer);
      },
      logout: async () => {
        const refresh = tokenStore.refresh;
        if (refresh) {
          try {
            await authApi.logout(refresh);
          } catch {
            /* ignore */
          }
        }
        tokenStore.clear();
        setRetailer(null);
      },
    }),
    [retailer, loading]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

// eslint-disable-next-line react-refresh/only-export-components
export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider");
  return ctx;
}
