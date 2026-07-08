import { createReducer, on } from '@ngrx/store';

import { LocationsActions } from './locations.actions';
import { initialLocationsState } from './locations.state';

export const locationsReducer = createReducer(
  initialLocationsState,

  on(LocationsActions.loadLocations, (state, { params }) => ({
    ...state,
    loading: true,
    error: null,
    warehouseId: params?.warehouseId ?? state.warehouseId,
    search: params?.search ?? state.search,
    page: params?.page ?? state.page,
    pageSize: params?.pageSize ?? state.pageSize,
  })),

  on(LocationsActions.loadLocationsSuccess, (state, { response, search, warehouseId }) => ({
    ...state,
    warehouseId,
    items: response.items,
    page: response.page,
    pageSize: response.pageSize,
    totalCount: response.totalCount,
    totalPages: response.totalPages,
    search,
    loading: false,
    error: null,
  })),

  on(LocationsActions.loadLocationsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(LocationsActions.loadParentOptionsSuccess, (state, { options }) => ({
    ...state,
    parentOptions: options,
  })),

  on(
    LocationsActions.createLocation,
    LocationsActions.updateLocation,
    LocationsActions.deleteLocation,
    (state) => ({ ...state, saving: true, error: null }),
  ),

  on(
    LocationsActions.createLocationSuccess,
    LocationsActions.updateLocationSuccess,
    LocationsActions.deleteLocationSuccess,
    (state) => ({ ...state, saving: false, error: null }),
  ),

  on(
    LocationsActions.createLocationFailure,
    LocationsActions.updateLocationFailure,
    LocationsActions.deleteLocationFailure,
    (state, { error }) => ({ ...state, saving: false, error }),
  ),
);
