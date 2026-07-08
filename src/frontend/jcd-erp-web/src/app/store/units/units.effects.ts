import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { UnitsService } from '../../features/units/units.service';
import { UnitsActions } from './units.actions';
import { selectUnitsPage, selectUnitsPageSize, selectUnitsSearch } from './units.selectors';

@Injectable()
export class UnitsEffects {
  private readonly actions$ = inject(Actions);
  private readonly unitsService = inject(UnitsService);
  private readonly store = inject(Store);

  readonly loadUnits$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UnitsActions.loadUnits),
      withLatestFrom(
        this.store.select(selectUnitsPage),
        this.store.select(selectUnitsPageSize),
        this.store.select(selectUnitsSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.unitsService.getUnits(query).pipe(
          map((response) =>
            UnitsActions.loadUnitsSuccess({
              response,
              search: query.search ?? '',
            }),
          ),
          catchError((error) =>
            of(UnitsActions.loadUnitsFailure({
              error: extractPlatformErrorCode(error, 'Units.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly createUnit$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UnitsActions.createUnit),
      exhaustMap(({ request }) =>
        this.unitsService.createUnit(request).pipe(
          map(() => UnitsActions.createUnitSuccess()),
          catchError((error) =>
            of(UnitsActions.createUnitFailure({
              error: extractPlatformErrorCode(error, 'Units.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateUnit$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UnitsActions.updateUnit),
      exhaustMap(({ unitId, request }) =>
        this.unitsService.updateUnit(unitId, request).pipe(
          map(() => UnitsActions.updateUnitSuccess()),
          catchError((error) =>
            of(UnitsActions.updateUnitFailure({
              error: extractPlatformErrorCode(error, 'Units.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteUnit$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UnitsActions.deleteUnit),
      exhaustMap(({ unitId }) =>
        this.unitsService.deleteUnit(unitId).pipe(
          map(() => UnitsActions.deleteUnitSuccess()),
          catchError((error) =>
            of(UnitsActions.deleteUnitFailure({
              error: extractPlatformErrorCode(error, 'Units.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UnitsActions.createUnitSuccess, UnitsActions.updateUnitSuccess, UnitsActions.deleteUnitSuccess),
      switchMap(() => [UnitsActions.loadUnits({})]),
    ),
  );
}
