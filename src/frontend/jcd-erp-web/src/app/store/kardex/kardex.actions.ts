import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  KardexLookupsResponse,
  KardexQueryParams,
  PaginatedKardexResponse,
} from '../../features/kardex/kardex.models';

export const KardexActions = createActionGroup({
  source: 'Kardex',
  events: {
    'Load Kardex': props<{ params?: KardexQueryParams }>(),
    'Load Kardex Success': props<{ response: PaginatedKardexResponse; filters: KardexQueryParams }>(),
    'Load Kardex Failure': props<{ error: string }>(),

    'Load Kardex Lookups': emptyProps(),
    'Load Kardex Lookups Success': props<{ lookups: KardexLookupsResponse }>(),
    'Load Kardex Lookups Failure': props<{ error: string }>(),
  },
});
