export interface RetailerProfile {
  retailerId: number;
  email: string;
  businessName: string;
  contactName?: string | null;
  phone?: string | null;
}

export interface AuthResponse {
  accessToken: string;
  accessTokenExpiresAt: string;
  refreshToken: string;
  retailer: RetailerProfile;
}

export interface DashboardSummary {
  devicesOnline: number;
  devicesOffline: number;
  totalDevices: number;
  totalContent: number;
  activePlaylists: number;
  totalProducts: number;
  lastImportAt?: string | null;
}

export interface Store {
  storeId: number;
  storeName: string;
  storeCode?: string | null;
  city?: string | null;
  state?: string | null;
  timeZone: string;
  isActive: boolean;
}

export interface Device {
  deviceId: number;
  deviceKey: string;
  deviceName?: string | null;
  storeId?: number | null;
  storeName?: string | null;
  playlistId?: number | null;
  playlistName?: string | null;
  status: string;
  lastSeenAt?: string | null;
  appVersion?: string | null;
  isRevoked: boolean;
  isActive: boolean;
}

export interface StoreProduct {
  storeProductId: number;
  storeId: number;
  source: string;
  sku: string;
  productName: string;
  price: number;
  salePrice?: number | null;
  currency: string;
  isActive: boolean;
}

export interface Playlist {
  playlistId: number;
  name: string;
  version: number;
  isActive: boolean;
  itemCount: number;
}

export interface ContentItem {
  contentId: number;
  contentType: string;
  name: string;
  version: number;
  isActive: boolean;
}

export interface Paged<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
