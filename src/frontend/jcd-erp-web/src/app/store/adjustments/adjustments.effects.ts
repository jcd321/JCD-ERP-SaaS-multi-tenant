import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { AdjustmentsService } from '../../features/adjustments/adjustments.service';
import { AdjustmentsActions } from './adjustments.actions';
import {
  selectAdjustmentsPage,
  selectAdjustmentsPageSize,
  selectAdjustmentsSearch,
  selectAdjustmentsWarehouseFilter,
} from './adjustments.selectors';

@Injectable()
export class AdjustmentsEffects {
  private readonly actions$ = inject(Actions);
  private readonly adjustmentsService = inject(AdjustmentsService);
  private readonly store = inject(Store);

  readonly loadAdjustments$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdjustmentsActions.loadAdjustments),
      withLatestFrom(
        this.store.select(selectAdjustmentsPage),
        this.store.select(selectAdjustmentsPageSize),
        this.store.select(selectAdjustmentsSearch),
        this.store.select(selectAdjustmentsWarehouseFilter),
      ),
      exhaustMap(([{ params }, page, pageSize, search, warehouseId]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          warehouseId: params?.warehouseId !== undefined ? params.warehouseId : warehouseId,
        };

        return this.adjustmentsService.getAdjustments(query).pipe(
          map((response) =>
            AdjustmentsActions.loadAdjustmentsSuccess({
              response,
              filters: {
                search: query.search ?? '',
                warehouseId: query.warehouseId ?? null,
              },
            }),
          ),
          catchError((error) =>
            of(AdjustmentsActions.loadAdjustmentsFailure({
              error: extractPlatformErrorCode(error, 'Adjustments.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadAdjustmentLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdjustmentsActions.loadAdjustmentLookups),
      exhaustMap(() =>
        this.adjustmentsService.getLookups().pipe(
          map((lookups) => AdjustmentsActions.loadAdjustmentLookupsSuccess({ lookups })),
          catchError((error) =>
            of(AdjustmentsActions.loadAdjustmentLookupsFailure({
              error: extractPlatformErrorCode(error, 'Adjustments.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createAdjustment$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdjustmentsActions.createAdjustment),
      exhaustMap(({ request }) =>
        this.adjustmentsService.createAdjustment(request).pipe(
          map(() => AdjustmentsActions.createAdjustmentSuccess()),
          catchError((error) =>
            of(AdjustmentsActions.createAdjustmentFailure({
              error: extractPlatformErrorCode(error, 'Adjustments.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterCreate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AdjustmentsActions.createAdjustmentSuccess),
      switchMap(() => [
        AdjustmentsActions.loadAdjustments({}),
        AdjustmentsActions.loadAdjustmentLookups(),
      ]),
    ),
  );
}
