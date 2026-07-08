import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateStockLevelRequest,
  PaginatedStockResponse,
  StockLookupsResponse,
  StockQueryParams,
  UpdateStockLevelRequest,
} from '../../features/stock/stock.models';

export const StockActions = createActionGroup({
  source: 'Stock',
  events: {
    'Load Stock Levels': props<{ params?: StockQueryParams }>(),
    'Load Stock Levels Success': props<{ response: PaginatedStockResponse; filters: StockQueryParams }>(),
    'Load Stock Levels Failure': props<{ error: string }>(),

    'Load Stock Lookups': emptyProps(),
    'Load Stock Lookups Success': props<{ lookups: StockLookupsResponse }>(),
    'Load Stock Lookups Failure': props<{ error: string }>(),

    'Create Stock Level': props<{ request: CreateStockLevelRequest }>(),
    'Create Stock Level Success': emptyProps(),
    'Create Stock Level Failure': props<{ error: string }>(),

    'Update Stock Level': props<{ stockLevelId: string; request: UpdateStockLevelRequest }>(),
    'Update Stock Level Success': emptyProps(),
    'Update Stock Level Failure': props<{ error: string }>(),

    'Delete Stock Level': props<{ stockLevelId: string }>(),
    'Delete Stock Level Success': emptyProps(),
    'Delete Stock Level Failure': props<{ error: string }>(),
  },
});
