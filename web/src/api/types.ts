export interface RetailerProfile {
  retailerId: number;
  email: string;
  businessName: string;
  contactName?: string | null;
  phone?: string | null;
  addressLine1?: string | null;
  addressLine2?: string | null;
  city?: string | null;
  state?: string | null;
  postalCode?: string | null;
  country?: string | null;
  profileCompleted: boolean;
}

export interface UpdateProfileRequest {
  businessName: string;
  contactName?: string;
  phone?: string;
  addressLine1?: string;
  addressLine2?: string;
  city?: string;
  state?: string;
  postalCode?: string;
  country?: string;
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

export interface PlaylistItem {
  playlistItemId: number;
  contentId: number;
  contentName: string;
  contentType: string;
  sortOrder: number;
  durationSeconds?: number | null;
  fitMode: string;
}

export interface PlaylistDetail {
  playlistId: number;
  name: string;
  version: number;
  isActive: boolean;
  items: PlaylistItem[];
}

export interface PlaylistItemInput {
  contentId: number;
  durationSeconds?: number | null;
  fitMode: number; // 1 Contain, 2 Cover, 3 Stretch
}

export interface ContentItem {
  contentId: number;
  contentType: string;
  name: string;
  version: number;
  isActive: boolean;
  thumbnailUrl?: string | null;
  previewUrl?: string | null;
}

export interface Paged<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}
