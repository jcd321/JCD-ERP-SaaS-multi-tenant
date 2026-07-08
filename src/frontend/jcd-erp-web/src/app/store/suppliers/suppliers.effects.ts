import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { SuppliersService } from '../../features/suppliers/suppliers.service';
import { SuppliersActions } from './suppliers.actions';
import {
  selectSuppliersPage,
  selectSuppliersPageSize,
  selectSuppliersSearch,
} from './suppliers.selectors';

@Injectable()
export class SuppliersEffects {
  private readonly actions$ = inject(Actions);
  private readonly suppliersService = inject(SuppliersService);
  private readonly store = inject(Store);

  readonly loadSuppliers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SuppliersActions.loadSuppliers),
      withLatestFrom(
        this.store.select(selectSuppliersPage),
        this.store.select(selectSuppliersPageSize),
        this.store.select(selectSuppliersSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.suppliersService.getSuppliers(query).pipe(
          map((response) =>
            SuppliersActions.loadSuppliersSuccess({
              response,
              search: query.search ?? '',
            }),
          ),
          catchError((error) =>
            of(SuppliersActions.loadSuppliersFailure({
              error: extractPlatformErrorCode(error, 'Suppliers.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly createSupplier$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SuppliersActions.createSupplier),
      exhaustMap(({ request }) =>
        this.suppliersService.createSupplier(request).pipe(
          map(() => SuppliersActions.createSupplierSuccess()),
          catchError((error) =>
            of(SuppliersActions.createSupplierFailure({
              error: extractPlatformErrorCode(error, 'Suppliers.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateSupplier$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SuppliersActions.updateSupplier),
      exhaustMap(({ supplierId, request }) =>
        this.suppliersService.updateSupplier(supplierId, request).pipe(
          map(() => SuppliersActions.updateSupplierSuccess()),
          catchError((error) =>
            of(SuppliersActions.updateSupplierFailure({
              error: extractPlatformErrorCode(error, 'Suppliers.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteSupplier$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SuppliersActions.deleteSupplier),
      exhaustMap(({ supplierId }) =>
        this.suppliersService.deleteSupplier(supplierId).pipe(
          map(() => SuppliersActions.deleteSupplierSuccess()),
          catchError((error) =>
            of(SuppliersActions.deleteSupplierFailure({
              error: extractPlatformErrorCode(error, 'Suppliers.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        SuppliersActions.createSupplierSuccess,
        SuppliersActions.updateSupplierSuccess,
        SuppliersActions.deleteSupplierSuccess,
      ),
      switchMap(() => [SuppliersActions.loadSuppliers({})]),
    ),
  );
}
