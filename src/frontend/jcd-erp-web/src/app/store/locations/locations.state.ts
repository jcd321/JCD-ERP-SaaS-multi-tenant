import { LocationParentOption, StorageLocation, LocationsQueryParams } from '../../features/locations/locations.models';

export interface LocationsState {
  warehouseId: string | null;
  items: StorageLocation[];
  parentOptions: LocationParentOption[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialLocationsState: LocationsState = {
  warehouseId: null,
  items: [],
  parentOptions: [],
  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,
  search: '',
  loading: false,
  saving: false,
  error: null,
};

export type LocationsLoadParams = LocationsQueryParams;
