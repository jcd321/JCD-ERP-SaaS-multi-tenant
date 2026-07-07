import { Customer, CustomersQueryParams } from '../../features/customers/customers.models';

export interface CustomersState {
  items: Customer[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialCustomersState: CustomersState = {
  items: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  loading: false,
  saving: false,
  error: null,
};

export type CustomersLoadParams = CustomersQueryParams;
