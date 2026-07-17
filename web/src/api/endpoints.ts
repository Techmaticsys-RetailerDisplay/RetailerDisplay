import { api } from "./client";
import type {
  AuthResponse, RetailerProfile, UpdateProfileRequest, DashboardSummary, Store, Device, StoreProduct,
  Playlist, PlaylistDetail, PlaylistItemInput, ContentItem, Paged,
} from "./types";

export const authApi = {
  login: (email: string, password: string) =>
    api.post<AuthResponse>("/auth/login", { email, password }).then((r) => r.data),
  logout: (refreshToken: string) => api.post("/auth/logout", { refreshToken }),
  me: () => api.get<RetailerProfile>("/auth/me").then((r) => r.data),
  getProfile: () => api.get<RetailerProfile>("/auth/profile").then((r) => r.data),
  updateProfile: (body: UpdateProfileRequest) => api.put<RetailerProfile>("/auth/profile", body).then((r) => r.data),
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

export interface ProductInput {
  sku?: string;
  productName: string;
  description?: string;
  category?: string;
  brand?: string;
  productType?: string;
  abv?: number | null;
  containerType?: string;
  volume?: string;
  packSize?: number | null;
  vintage?: number | null;
  price: number;
  salePrice?: number | null;
  currency?: string;
  imageUrl?: string | null;
  isActive?: boolean;
}

export const productsApi = {
  list: (storeId: number, search = "", activeOnly = false) =>
    api.get<Paged<StoreProduct>>(`/stores/${storeId}/products`, { params: { search, activeOnly, pageSize: 100 } }).then((r) => r.data),
  create: (storeId: number, body: ProductInput) =>
    api.post<StoreProduct>(`/stores/${storeId}/products`, body).then((r) => r.data),
  update: (id: number, body: ProductInput) =>
    api.put<StoreProduct>(`/products/${id}`, body).then((r) => r.data),
  setActive: (id: number, value: boolean) =>
    api.patch(`/products/${id}/active`, null, { params: { value } }),
  remove: (id: number) => api.delete(`/products/${id}`),
  import: (storeId: number, file: File) => {
    const form = new FormData();
    form.append("file", file);
    return api.post(`/stores/${storeId}/products/import`, form).then((r) => r.data);
  },
  pull: (storeId: number, masterProductIds: number[]) =>
    api.post(`/stores/${storeId}/products/pull`, { masterProductIds }).then((r) => r.data),
};

export interface MasterProduct {
  masterProductId: number;
  sku?: string | null;
  productName: string;
  brand?: string | null;
  category?: string | null;
  volume?: string | null;
}

export const masterApi = {
  list: (search = "") =>
    api.get<Paged<MasterProduct>>("/master-products", { params: { search, pageSize: 200 } }).then((r) => r.data),
};

export const playlistsApi = {
  list: () => api.get<Playlist[]>("/playlists").then((r) => r.data),
  get: (id: number) => api.get<PlaylistDetail>(`/playlists/${id}`).then((r) => r.data),
  create: (name: string) => api.post<Playlist>("/playlists", { name }).then((r) => r.data),
  setItems: (id: number, items: PlaylistItemInput[]) =>
    api.put<PlaylistDetail>(`/playlists/${id}/items`, { items }).then((r) => r.data),
  remove: (id: number) => api.delete(`/playlists/${id}`),
};

export const mediaApi = {
  upload: (file: File, kind: "image" | "video") => {
    const form = new FormData();
    form.append("file", file);
    form.append("kind", kind);
    return api.post<{ key: string; contentType: string; sizeBytes: number }>("/media/upload", form).then((r) => r.data);
  },
};

export const contentApi = {
  list: (type?: string) => api.get<ContentItem[]>("/content", { params: type ? { type } : {} }).then((r) => r.data),
  createImage: (name: string, masterKey: string, fileSizeBytes: number) =>
    api.post<ContentItem>("/content", { contentType: 1, name, masterKey, fileSizeBytes }).then((r) => r.data),
  createVideo: (name: string, masterKey: string, fileSizeBytes: number, durationSeconds?: number) =>
    api.post<ContentItem>("/content", { contentType: 2, name, masterKey, fileSizeBytes, durationSeconds }).then((r) => r.data),
  createProductList: (name: string, storeProductIds: number[]) =>
    api.post<ContentItem>("/content", { contentType: 3, name, storeProductIds }).then((r) => r.data),
  remove: (id: number) => api.delete(`/content/${id}`),
};
