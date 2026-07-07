export interface Customer {
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

export interface PaginatedCustomersResponse {
  items: Customer[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface CustomerPayload {
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

export type CreateCustomerRequest = CustomerPayload;

export interface UpdateCustomerRequest extends CustomerPayload {
  isActive: boolean;
}

export type CustomerFormMode = 'create' | 'edit' | null;

export interface CustomersQueryParams {
  page?: number;
  pageSize?: number;
  search?: string;
  isActive?: boolean | null;
}
