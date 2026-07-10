import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { TransfersService } from '../../features/transfers/transfers.service';
import { TransfersActions } from './transfers.actions';
import {
  selectTransfersDestinationFilter,
  selectTransfersPage,
  selectTransfersPageSize,
  selectTransfersSearch,
  selectTransfersSourceFilter,
} from './transfers.selectors';

@Injectable()
export class TransfersEffects {
  private readonly actions$ = inject(Actions);
  private readonly transfersService = inject(TransfersService);
  private readonly store = inject(Store);

  readonly loadTransfers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TransfersActions.loadTransfers),
      withLatestFrom(
        this.store.select(selectTransfersPage),
        this.store.select(selectTransfersPageSize),
        this.store.select(selectTransfersSearch),
        this.store.select(selectTransfersSourceFilter),
        this.store.select(selectTransfersDestinationFilter),
      ),
      exhaustMap(([{ params }, page, pageSize, search, sourceWarehouseId, destinationWarehouseId]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          sourceWarehouseId: params?.sourceWarehouseId !== undefined ? params.sourceWarehouseId : sourceWarehouseId,
          destinationWarehouseId: params?.destinationWarehouseId !== undefined ? params.destinationWarehouseId : destinationWarehouseId,
        };

        return this.transfersService.getTransfers(query).pipe(
          map((response) =>
            TransfersActions.loadTransfersSuccess({
              response,
              filters: {
                search: query.search ?? '',
                sourceWarehouseId: query.sourceWarehouseId ?? null,
                destinationWarehouseId: query.destinationWarehouseId ?? null,
              },
            }),
          ),
          catchError((error) =>
            of(TransfersActions.loadTransfersFailure({
              error: extractPlatformErrorCode(error, 'Transfers.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadTransferLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TransfersActions.loadTransferLookups),
      exhaustMap(() =>
        this.transfersService.getLookups().pipe(
          map((lookups) => TransfersActions.loadTransferLookupsSuccess({ lookups })),
          catchError((error) =>
            of(TransfersActions.loadTransferLookupsFailure({
              error: extractPlatformErrorCode(error, 'Transfers.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createTransfer$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TransfersActions.createTransfer),
      exhaustMap(({ request }) =>
        this.transfersService.createTransfer(request).pipe(
          map(() => TransfersActions.createTransferSuccess()),
          catchError((error) =>
            of(TransfersActions.createTransferFailure({
              error: extractPlatformErrorCode(error, 'Transfers.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterCreate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(TransfersActions.createTransferSuccess),
      switchMap(() => [
        TransfersActions.loadTransfers({}),
        TransfersActions.loadTransferLookups(),
      ]),
    ),
  );
}
