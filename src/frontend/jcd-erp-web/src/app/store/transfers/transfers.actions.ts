import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateInventoryTransferRequest,
  PaginatedTransfersResponse,
  TransferLookupsResponse,
  TransfersQueryParams,
} from '../../features/transfers/transfers.models';

export const TransfersActions = createActionGroup({
  source: 'Transfers',
  events: {
    'Load Transfers': props<{ params?: TransfersQueryParams }>(),
    'Load Transfers Success': props<{ response: PaginatedTransfersResponse; filters: TransfersQueryParams }>(),
    'Load Transfers Failure': props<{ error: string }>(),

    'Load Transfer Lookups': emptyProps(),
    'Load Transfer Lookups Success': props<{ lookups: TransferLookupsResponse }>(),
    'Load Transfer Lookups Failure': props<{ error: string }>(),

    'Create Transfer': props<{ request: CreateInventoryTransferRequest }>(),
    'Create Transfer Success': emptyProps(),
    'Create Transfer Failure': props<{ error: string }>(),

    'Clear Error': emptyProps(),
  },
});
