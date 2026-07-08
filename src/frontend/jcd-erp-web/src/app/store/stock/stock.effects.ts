import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { StockService } from '../../features/stock/stock.service';
import { StockActions } from './stock.actions';
import {
  selectStockBelowMinimumOnly,
  selectStockPage,
  selectStockPageSize,
  selectStockProductFilter,
  selectStockSearch,
  selectStockWarehouseFilter,
} from './stock.selectors';

@Injectable()
export class StockEffects {
  private readonly actions$ = inject(Actions);
  private readonly stockService = inject(StockService);
  private readonly store = inject(Store);

  readonly loadStockLevels$ = createEffect(() =>
    this.actions$.pipe(
      ofType(StockActions.loadStockLevels),
      withLatestFrom(
        this.store.select(selectStockPage),
        this.store.select(selectStockPageSize),
        this.store.select(selectStockSearch),
        this.store.select(selectStockWarehouseFilter),
        this.store.select(selectStockProductFilter),
        this.store.select(selectStockBelowMinimumOnly),
      ),
      exhaustMap(([{ params }, page, pageSize, search, warehouseId, productId, belowMinimumOnly]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          warehouseId: params?.warehouseId !== undefined ? params.warehouseId : warehouseId,
          productId: params?.productId !== undefined ? params.productId : productId,
          belowMinimumOnly: params?.belowMinimumOnly ?? belowMinimumOnly,
        };

        return this.stockService.getStockLevels(query).pipe(
          map((response) =>
            StockActions.loadStockLevelsSuccess({
              response,
              filters: {
                search: query.search ?? '',
                warehouseId: query.warehouseId ?? null,
                productId: query.productId ?? null,
                belowMinimumOnly: query.belowMinimumOnly ?? false,
              },
            }),
          ),
          catchError((error) =>
            of(StockActions.loadStockLevelsFailure({
              error: extractPlatformErrorCode(error, 'Stock.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadStockLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(StockActions.loadStockLookups),
      exhaustMap(() =>
        this.stockService.getLookups().pipe(
          map((lookups) => StockActions.loadStockLookupsSuccess({ lookups })),
          catchError((error) =>
            of(StockActions.loadStockLookupsFailure({
              error: extractPlatformErrorCode(error, 'Stock.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createStockLevel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(StockActions.createStockLevel),
      exhaustMap(({ request }) =>
        this.stockService.createStockLevel(request).pipe(
          map(() => StockActions.createStockLevelSuccess()),
          catchError((error) =>
            of(StockActions.createStockLevelFailure({
              error: extractPlatformErrorCode(error, 'Stock.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateStockLevel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(StockActions.updateStockLevel),
      exhaustMap(({ stockLevelId, request }) =>
        this.stockService.updateStockLevel(stockLevelId, request).pipe(
          map(() => StockActions.updateStockLevelSuccess()),
          catchError((error) =>
            of(StockActions.updateStockLevelFailure({
              error: extractPlatformErrorCode(error, 'Stock.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteStockLevel$ = createEffect(() =>
    this.actions$.pipe(
      ofType(StockActions.deleteStockLevel),
      exhaustMap(({ stockLevelId }) =>
        this.stockService.deleteStockLevel(stockLevelId).pipe(
          map(() => StockActions.deleteStockLevelSuccess()),
          catchError((error) =>
            of(StockActions.deleteStockLevelFailure({
              error: extractPlatformErrorCode(error, 'Stock.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        StockActions.createStockLevelSuccess,
        StockActions.updateStockLevelSuccess,
        StockActions.deleteStockLevelSuccess,
      ),
      switchMap(() => [StockActions.loadStockLevels({})]),
    ),
  );
}
