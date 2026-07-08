import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { MovementsService } from '../../features/movements/movements.service';
import { MovementType } from '../../features/movements/movements.models';
import { MovementsActions } from './movements.actions';
import {
  selectMovementsPage,
  selectMovementsPageSize,
  selectMovementsProductFilter,
  selectMovementsSearch,
  selectMovementsTypeFilter,
  selectMovementsWarehouseFilter,
} from './movements.selectors';

@Injectable()
export class MovementsEffects {
  private readonly actions$ = inject(Actions);
  private readonly movementsService = inject(MovementsService);
  private readonly store = inject(Store);

  readonly loadMovements$ = createEffect(() =>
    this.actions$.pipe(
      ofType(MovementsActions.loadMovements),
      withLatestFrom(
        this.store.select(selectMovementsPage),
        this.store.select(selectMovementsPageSize),
        this.store.select(selectMovementsSearch),
        this.store.select(selectMovementsWarehouseFilter),
        this.store.select(selectMovementsProductFilter),
        this.store.select(selectMovementsTypeFilter),
      ),
      exhaustMap(([{ params }, page, pageSize, search, warehouseId, productId, movementType]) => {
        const resolvedMovementType = params?.movementType !== undefined
          ? params.movementType
          : movementType;

        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          warehouseId: params?.warehouseId !== undefined ? params.warehouseId : warehouseId,
          productId: params?.productId !== undefined ? params.productId : productId,
          movementType: (resolvedMovementType ?? null) as MovementType | null,
        };

        return this.movementsService.getMovements(query).pipe(
          map((response) =>
            MovementsActions.loadMovementsSuccess({
              response,
              filters: {
                search: query.search ?? '',
                warehouseId: query.warehouseId ?? null,
                productId: query.productId ?? null,
                movementType: query.movementType ?? null,
              },
            }),
          ),
          catchError((error) =>
            of(MovementsActions.loadMovementsFailure({
              error: extractPlatformErrorCode(error, 'Movements.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadMovementLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(MovementsActions.loadMovementLookups),
      exhaustMap(() =>
        this.movementsService.getLookups().pipe(
          map((lookups) => MovementsActions.loadMovementLookupsSuccess({ lookups })),
          catchError((error) =>
            of(MovementsActions.loadMovementLookupsFailure({
              error: extractPlatformErrorCode(error, 'Movements.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createMovement$ = createEffect(() =>
    this.actions$.pipe(
      ofType(MovementsActions.createMovement),
      exhaustMap(({ request }) =>
        this.movementsService.createMovement(request).pipe(
          map(() => MovementsActions.createMovementSuccess()),
          catchError((error) =>
            of(MovementsActions.createMovementFailure({
              error: extractPlatformErrorCode(error, 'Movements.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterCreate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(MovementsActions.createMovementSuccess),
      switchMap(() => [MovementsActions.loadMovements({})]),
    ),
  );
}
