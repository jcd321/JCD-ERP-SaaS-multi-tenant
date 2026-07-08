import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { BrandsService } from '../../features/brands/brands.service';
import { BrandsActions } from './brands.actions';
import { selectBrandsPage, selectBrandsPageSize, selectBrandsSearch } from './brands.selectors';

@Injectable()
export class BrandsEffects {
  private readonly actions$ = inject(Actions);
  private readonly brandsService = inject(BrandsService);
  private readonly store = inject(Store);

  readonly loadBrands$ = createEffect(() =>
    this.actions$.pipe(
      ofType(BrandsActions.loadBrands),
      withLatestFrom(
        this.store.select(selectBrandsPage),
        this.store.select(selectBrandsPageSize),
        this.store.select(selectBrandsSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.brandsService.getBrands(query).pipe(
          map((response) =>
            BrandsActions.loadBrandsSuccess({
              response,
              search: query.search ?? '',
            }),
          ),
          catchError((error) =>
            of(BrandsActions.loadBrandsFailure({
              error: extractPlatformErrorCode(error, 'Brands.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly createBrand$ = createEffect(() =>
    this.actions$.pipe(
      ofType(BrandsActions.createBrand),
      exhaustMap(({ request }) =>
        this.brandsService.createBrand(request).pipe(
          map(() => BrandsActions.createBrandSuccess()),
          catchError((error) =>
            of(BrandsActions.createBrandFailure({
              error: extractPlatformErrorCode(error, 'Brands.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateBrand$ = createEffect(() =>
    this.actions$.pipe(
      ofType(BrandsActions.updateBrand),
      exhaustMap(({ brandId, request }) =>
        this.brandsService.updateBrand(brandId, request).pipe(
          map(() => BrandsActions.updateBrandSuccess()),
          catchError((error) =>
            of(BrandsActions.updateBrandFailure({
              error: extractPlatformErrorCode(error, 'Brands.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteBrand$ = createEffect(() =>
    this.actions$.pipe(
      ofType(BrandsActions.deleteBrand),
      exhaustMap(({ brandId }) =>
        this.brandsService.deleteBrand(brandId).pipe(
          map(() => BrandsActions.deleteBrandSuccess()),
          catchError((error) =>
            of(BrandsActions.deleteBrandFailure({
              error: extractPlatformErrorCode(error, 'Brands.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(BrandsActions.createBrandSuccess, BrandsActions.updateBrandSuccess, BrandsActions.deleteBrandSuccess),
      switchMap(() => [BrandsActions.loadBrands({})]),
    ),
  );
}
