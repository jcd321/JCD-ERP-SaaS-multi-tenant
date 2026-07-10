import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreatePhysicalCountRequest,
  PaginatedPhysicalCountsResponse,
  PhysicalCountLineUpdate,
  PhysicalCountLookupsResponse,
  PhysicalCountsQueryParams,
  PhysicalInventoryCount,
} from '../../features/physical-counts/physical-counts.models';

export const PhysicalCountsActions = createActionGroup({
  source: 'PhysicalCounts',
  events: {
    'Load Physical Counts': props<{ params?: PhysicalCountsQueryParams }>(),
    'Load Physical Counts Success': props<{ response: PaginatedPhysicalCountsResponse; filters: PhysicalCountsQueryParams }>(),
    'Load Physical Counts Failure': props<{ error: string }>(),

    'Load Physical Count': props<{ id: string }>(),
    'Load Physical Count Success': props<{ count: PhysicalInventoryCount }>(),
    'Load Physical Count Failure': props<{ error: string }>(),

    'Load Physical Count Lookups': emptyProps(),
    'Load Physical Count Lookups Success': props<{ lookups: PhysicalCountLookupsResponse }>(),
    'Load Physical Count Lookups Failure': props<{ error: string }>(),

    'Create Physical Count': props<{ request: CreatePhysicalCountRequest }>(),
    'Create Physical Count Success': props<{ id: string }>(),
    'Create Physical Count Failure': props<{ error: string }>(),

    'Update Physical Count Lines': props<{ countId: string; lines: PhysicalCountLineUpdate[] }>(),
    'Update Physical Count Lines Success': emptyProps(),
    'Update Physical Count Lines Failure': props<{ error: string }>(),

    'Complete Physical Count': props<{ countId: string; lines?: PhysicalCountLineUpdate[] }>(),
    'Complete Physical Count Success': emptyProps(),
    'Complete Physical Count Failure': props<{ error: string }>(),

    'Cancel Physical Count': props<{ countId: string }>(),
    'Cancel Physical Count Success': emptyProps(),
    'Cancel Physical Count Failure': props<{ error: string }>(),

    'Clear Error': emptyProps(),
  },
});
