import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateUnitRequest,
  PaginatedUnitsResponse,
  UnitsQueryParams,
  UpdateUnitRequest,
} from '../../features/units/units.models';

export const UnitsActions = createActionGroup({
  source: 'Units',
  events: {
    'Load Units': props<{ params?: UnitsQueryParams }>(),
    'Load Units Success': props<{ response: PaginatedUnitsResponse; search: string }>(),
    'Load Units Failure': props<{ error: string }>(),

    'Create Unit': props<{ request: CreateUnitRequest }>(),
    'Create Unit Success': emptyProps(),
    'Create Unit Failure': props<{ error: string }>(),

    'Update Unit': props<{ unitId: string; request: UpdateUnitRequest }>(),
    'Update Unit Success': emptyProps(),
    'Update Unit Failure': props<{ error: string }>(),

    'Delete Unit': props<{ unitId: string }>(),
    'Delete Unit Success': emptyProps(),
    'Delete Unit Failure': props<{ error: string }>(),
  },
});
