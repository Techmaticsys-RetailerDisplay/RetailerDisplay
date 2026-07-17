import axios from "axios";

export interface PlatformOverview {
  totalRetailers: number;
  totalStores: number;
  totalDevices: number;
  devicesOnline: number;
  devicesOffline: number;
  totalProducts: number;
  totalPlaylists: number;
  totalContent: number;
}

export interface RetailerUsage {
  retailerId: number;
  businessName: string;
  email: string;
  storeCount: number;
  deviceCount: number;
  productCount: number;
  isActive: boolean;
  createdAt: string;
}

const baseURL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5080/api/v1";
const KEY = "rd_admin";

export const adminToken = {
  get: () => localStorage.getItem(KEY),
  set: (t: string) => localStorage.setItem(KEY, t),
  clear: () => localStorage.removeItem(KEY),
};

const client = axios.create({ baseURL });
client.interceptors.request.use((config) => {
  const t = adminToken.get();
  if (t) config.headers.Authorization = `Bearer ${t}`;
  return config;
});

export interface MasterProduct {
  masterProductId: number;
  sku?: string | null;
  productName: string;
  brand?: string | null;
  category?: string | null;
  productType?: string | null;
  volume?: string | null;
}

export const adminApi = {
  login: (email: string, password: string) =>
    client.post<{ accessToken: string; admin: { adminUserId: number; email: string; name?: string } }>(
      "/admin/auth/login", { email, password }
    ).then((r) => r.data),
  overview: () => client.get<PlatformOverview>("/admin/overview").then((r) => r.data),
  retailers: () => client.get<RetailerUsage[]>("/admin/retailers").then((r) => r.data),
  createRetailer: (body: { businessName: string; email: string; password: string }) =>
    client.post<RetailerUsage>("/admin/retailers", body).then((r) => r.data),
  listMaster: () =>
    client.get<{ items: MasterProduct[]; total: number }>("/admin/master-products", { params: { pageSize: 200 } }).then((r) => r.data),
  importMaster: (file: File) => {
    const form = new FormData();
    form.append("file", file);
    return client.post<{ totalRows: number; successCount: number; failCount: number }>("/admin/master-products/import", form).then((r) => r.data);
  },
};
