import { UnitOfMeasure, UnitsQueryParams } from '../../features/units/units.models';

export interface UnitsState {
  items: UnitOfMeasure[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  search: string;
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialUnitsState: UnitsState = {
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

export type UnitsLoadParams = UnitsQueryParams;
