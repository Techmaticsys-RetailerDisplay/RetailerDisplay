import axios, { AxiosError, type InternalAxiosRequestConfig } from "axios";
import type { AuthResponse } from "./types";

const baseURL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5080/api/v1";

export const api = axios.create({ baseURL });

const ACCESS_KEY = "rd_access";
const REFRESH_KEY = "rd_refresh";

export const tokenStore = {
  get access() {
    return localStorage.getItem(ACCESS_KEY);
  },
  get refresh() {
    return localStorage.getItem(REFRESH_KEY);
  },
  set(access: string, refresh: string) {
    localStorage.setItem(ACCESS_KEY, access);
    localStorage.setItem(REFRESH_KEY, refresh);
  },
  clear() {
    localStorage.removeItem(ACCESS_KEY);
    localStorage.removeItem(REFRESH_KEY);
  },
};

api.interceptors.request.use((config) => {
  const token = tokenStore.access;
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

let refreshing: Promise<string | null> | null = null;

async function doRefresh(): Promise<string | null> {
  const refresh = tokenStore.refresh;
  if (!refresh) return null;
  try {
    const res = await axios.post<AuthResponse>(`${baseURL}/auth/refresh`, { refreshToken: refresh });
    tokenStore.set(res.data.accessToken, res.data.refreshToken);
    return res.data.accessToken;
  } catch {
    tokenStore.clear();
    return null;
  }
}

api.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const original = error.config as (InternalAxiosRequestConfig & { _retry?: boolean }) | undefined;
    if (error.response?.status === 401 && original && !original._retry && tokenStore.refresh) {
      original._retry = true;
      refreshing ??= doRefresh();
      const newToken = await refreshing;
      refreshing = null;
      if (newToken) {
        original.headers.Authorization = `Bearer ${newToken}`;
        return api(original);
      }
    }
    return Promise.reject(error);
  }
);

/** Extracts a human-readable message from an API error (RFC 7807 or fallback). */
export function apiError(err: unknown): string {
  const ax = err as AxiosError<{ detail?: string; title?: string }>;
  return ax.response?.data?.detail ?? ax.response?.data?.title ?? ax.message ?? "Something went wrong.";
}
