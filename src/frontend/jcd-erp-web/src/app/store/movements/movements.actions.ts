import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateInventoryMovementRequest,
  MovementLookupsResponse,
  MovementsQueryParams,
  PaginatedMovementsResponse,
} from '../../features/movements/movements.models';

export const MovementsActions = createActionGroup({
  source: 'Movements',
  events: {
    'Load Movements': props<{ params?: MovementsQueryParams }>(),
    'Load Movements Success': props<{ response: PaginatedMovementsResponse; filters: MovementsQueryParams }>(),
    'Load Movements Failure': props<{ error: string }>(),

    'Load Movement Lookups': emptyProps(),
    'Load Movement Lookups Success': props<{ lookups: MovementLookupsResponse }>(),
    'Load Movement Lookups Failure': props<{ error: string }>(),

    'Create Movement': props<{ request: CreateInventoryMovementRequest }>(),
    'Create Movement Success': emptyProps(),
    'Create Movement Failure': props<{ error: string }>(),
  },
});
