export interface Supplier {
  id: string;
  code: string;
  legalName: string;
  tradeName: string | null;
  taxId: string | null;
  email: string | null;
  phone: string | null;
  mobilePhone: string | null;
  addressLine1: string | null;
  city: string | null;
  stateOrProvince: string | null;
  countryCode: string | null;
  notes: string | null;
  isActive: boolean;
}

export interface PaginatedSuppliersResponse {
  items: Supplier[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface SupplierPayload {
  code: string;
  legalName: string;
  tradeName?: string | null;
  taxId?: string | null;
  email?: string | null;
  phone?: string | null;
  mobilePhone?: string | null;
  addressLine1?: string | null;
  city?: string | null;
  stateOrProvince?: string | null;
  countryCode?: string | null;
  notes?: string | null;
}

export type CreateSupplierRequest = SupplierPayload;

export interface UpdateSupplierRequest extends SupplierPayload {
  isActive: boolean;
}

export type SupplierFormMode = 'create' | 'edit' | null;

export interface SuppliersQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
