import { createReducer, on } from '@ngrx/store';

import { UnitsActions } from './units.actions';
import { initialUnitsState } from './units.state';

export const unitsReducer = createReducer(
  initialUnitsState,

  on(UnitsActions.loadUnits, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(UnitsActions.loadUnitsSuccess, (state, { response, search }) => ({
    ...state,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search,
    loading: false,
    error: null,
  })),

  on(UnitsActions.loadUnitsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(UnitsActions.createUnit, UnitsActions.updateUnit, UnitsActions.deleteUnit, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    UnitsActions.createUnitSuccess,
    UnitsActions.updateUnitSuccess,
    UnitsActions.deleteUnitSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    UnitsActions.createUnitFailure,
    UnitsActions.updateUnitFailure,
    UnitsActions.deleteUnitFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
