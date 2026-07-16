import { api } from "./client";
import type {
  AuthResponse, DashboardSummary, Store, Device, StoreProduct, Playlist, ContentItem, Paged,
} from "./types";

export const authApi = {
  login: (email: string, password: string) =>
    api.post<AuthResponse>("/auth/login", { email, password }).then((r) => r.data),
  register: (body: { email: string; password: string; businessName: string; contactName?: string; phone?: string }) =>
    api.post<AuthResponse>("/auth/register", body).then((r) => r.data),
  logout: (refreshToken: string) => api.post("/auth/logout", { refreshToken }),
  me: () => api.get<AuthResponse["retailer"]>("/auth/me").then((r) => r.data),
};

export const dashboardApi = {
  summary: () => api.get<DashboardSummary>("/dashboard/summary").then((r) => r.data),
};

export const storesApi = {
  list: () => api.get<Store[]>("/stores").then((r) => r.data),
  create: (body: Partial<Store> & { storeName: string; timeZone: string }) =>
    api.post<Store>("/stores", body).then((r) => r.data),
  update: (id: number, body: Partial<Store>) => api.put<Store>(`/stores/${id}`, body).then((r) => r.data),
  deactivate: (id: number) => api.delete(`/stores/${id}`),
};

export const devicesApi = {
  list: () => api.get<Device[]>("/devices").then((r) => r.data),
  register: (body: { deviceName?: string; storeId?: number; playlistId?: number }) =>
    api.post<Device>("/devices", body).then((r) => r.data),
  assignPlaylist: (id: number, playlistId: number | null) =>
    api.put<Device>(`/devices/${id}/playlist`, { playlistId }).then((r) => r.data),
  refresh: (id: number) => api.post(`/devices/${id}/refresh`),
  revoke: (id: number) => api.post(`/devices/${id}/revoke`),
  remove: (id: number) => api.delete(`/devices/${id}`),
};

export const productsApi = {
  list: (storeId: number, search = "", activeOnly = false) =>
    api.get<Paged<StoreProduct>>(`/stores/${storeId}/products`, { params: { search, activeOnly, pageSize: 100 } }).then((r) => r.data),
  import: (storeId: number, file: File) => {
    const form = new FormData();
    form.append("file", file);
    return api.post(`/stores/${storeId}/products/import`, form).then((r) => r.data);
  },
  pull: (storeId: number, masterProductIds: number[]) =>
    api.post(`/stores/${storeId}/products/pull`, { masterProductIds }).then((r) => r.data),
};

export const playlistsApi = {
  list: () => api.get<Playlist[]>("/playlists").then((r) => r.data),
  create: (name: string) => api.post<Playlist>("/playlists", { name }).then((r) => r.data),
  remove: (id: number) => api.delete(`/playlists/${id}`),
};

export const contentApi = {
  list: (type?: string) => api.get<ContentItem[]>("/content", { params: type ? { type } : {} }).then((r) => r.data),
  createProductList: (name: string, storeProductIds: number[]) =>
    api.post<ContentItem>("/content", { contentType: 3, name, storeProductIds }).then((r) => r.data),
  remove: (id: number) => api.delete(`/content/${id}`),
};
