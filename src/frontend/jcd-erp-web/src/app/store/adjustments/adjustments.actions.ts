import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  AdjustmentLookupsResponse,
  AdjustmentsQueryParams,
  CreateInventoryAdjustmentRequest,
  PaginatedAdjustmentsResponse,
} from '../../features/adjustments/adjustments.models';

export const AdjustmentsActions = createActionGroup({
  source: 'Adjustments',
  events: {
    'Load Adjustments': props<{ params?: AdjustmentsQueryParams }>(),
    'Load Adjustments Success': props<{ response: PaginatedAdjustmentsResponse; filters: AdjustmentsQueryParams }>(),
    'Load Adjustments Failure': props<{ error: string }>(),

    'Load Adjustment Lookups': emptyProps(),
    'Load Adjustment Lookups Success': props<{ lookups: AdjustmentLookupsResponse }>(),
    'Load Adjustment Lookups Failure': props<{ error: string }>(),

    'Create Adjustment': props<{ request: CreateInventoryAdjustmentRequest }>(),
    'Create Adjustment Success': emptyProps(),
    'Create Adjustment Failure': props<{ error: string }>(),

    'Clear Error': emptyProps(),
  },
});
