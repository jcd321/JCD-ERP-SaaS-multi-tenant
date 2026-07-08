import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { KardexService } from '../../features/kardex/kardex.service';
import { KardexActions } from './kardex.actions';
import {
  selectKardexFromDateFilter,
  selectKardexPage,
  selectKardexPageSize,
  selectKardexProductFilter,
  selectKardexToDateFilter,
  selectKardexWarehouseFilter,
} from './kardex.selectors';

@Injectable()
export class KardexEffects {
  private readonly actions$ = inject(Actions);
  private readonly kardexService = inject(KardexService);
  private readonly store = inject(Store);

  readonly loadKardex$ = createEffect(() =>
    this.actions$.pipe(
      ofType(KardexActions.loadKardex),
      withLatestFrom(
        this.store.select(selectKardexPage),
        this.store.select(selectKardexPageSize),
        this.store.select(selectKardexProductFilter),
        this.store.select(selectKardexWarehouseFilter),
        this.store.select(selectKardexFromDateFilter),
        this.store.select(selectKardexToDateFilter),
      ),
      exhaustMap(([{ params }, page, pageSize, productId, warehouseId, fromDate, toDate]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          productId: params?.productId !== undefined ? params.productId : productId,
          warehouseId: params?.warehouseId !== undefined ? params.warehouseId : warehouseId,
          fromDate: params?.fromDate !== undefined ? params.fromDate : fromDate,
          toDate: params?.toDate !== undefined ? params.toDate : toDate,
        };

        if (!query.productId) {
          return of(KardexActions.loadKardexSuccess({
            response: {
              items: [],
              page: 1,
              pageSize: query.pageSize ?? 50,
              totalCount: 0,
              totalPages: 0,
            },
            filters: query,
          }));
        }

        return this.kardexService.getKardex(query).pipe(
          map((response) => KardexActions.loadKardexSuccess({ response, filters: query })),
          catchError((error) =>
            of(KardexActions.loadKardexFailure({
              error: extractPlatformErrorCode(error, 'Kardex.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadKardexLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(KardexActions.loadKardexLookups),
      exhaustMap(() =>
        this.kardexService.getLookups().pipe(
          map((lookups) => KardexActions.loadKardexLookupsSuccess({ lookups })),
          catchError((error) =>
            of(KardexActions.loadKardexLookupsFailure({
              error: extractPlatformErrorCode(error, 'Kardex.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );
}
