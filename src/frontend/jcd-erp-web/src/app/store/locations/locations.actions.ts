import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  LocationParentOption,
  CreateLocationRequest,
  PaginatedLocationsResponse,
  LocationsQueryParams,
  UpdateLocationRequest,
} from '../../features/locations/locations.models';

export const LocationsActions = createActionGroup({
  source: 'Locations',
  events: {
    'Load Locations': props<{ params?: Partial<LocationsQueryParams> & { warehouseId?: string } }>(),
    'Load Locations Success': props<{ response: PaginatedLocationsResponse; search: string; warehouseId: string }>(),
    'Load Locations Failure': props<{ error: string }>(),

    'Load Parent Options': props<{ warehouseId: string; excludeId?: string }>(),
    'Load Parent Options Success': props<{ options: LocationParentOption[] }>(),
    'Load Parent Options Failure': props<{ error: string }>(),

    'Create Location': props<{ request: CreateLocationRequest }>(),
    'Create Location Success': emptyProps(),
    'Create Location Failure': props<{ error: string }>(),

    'Update Location': props<{ locationId: string; request: UpdateLocationRequest }>(),
    'Update Location Success': emptyProps(),
    'Update Location Failure': props<{ error: string }>(),

    'Delete Location': props<{ locationId: string }>(),
    'Delete Location Success': emptyProps(),
    'Delete Location Failure': props<{ error: string }>(),
  },
});
