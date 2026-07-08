import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { ProductsService } from '../../features/products/products.service';
import { ProductsActions } from './products.actions';
import {
  selectProductsPage,
  selectProductsPageSize,
  selectProductsSearch,
} from './products.selectors';

@Injectable()
export class ProductsEffects {
  private readonly actions$ = inject(Actions);
  private readonly productsService = inject(ProductsService);
  private readonly store = inject(Store);

  readonly loadProducts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductsActions.loadProducts),
      withLatestFrom(
        this.store.select(selectProductsPage),
        this.store.select(selectProductsPageSize),
        this.store.select(selectProductsSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.productsService.getProducts(query).pipe(
          map((response) =>
            ProductsActions.loadProductsSuccess({ response, search: query.search ?? '' }),
          ),
          catchError((error) =>
            of(ProductsActions.loadProductsFailure({
              error: extractPlatformErrorCode(error, 'Products.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductsActions.loadLookups),
      exhaustMap(() =>
        this.productsService.getLookups().pipe(
          map((lookups) => ProductsActions.loadLookupsSuccess({ lookups })),
          catchError((error) =>
            of(ProductsActions.loadLookupsFailure({
              error: extractPlatformErrorCode(error, 'Products.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductsActions.createProduct),
      exhaustMap(({ request }) =>
        this.productsService.createProduct(request).pipe(
          map(() => ProductsActions.createProductSuccess()),
          catchError((error) =>
            of(ProductsActions.createProductFailure({
              error: extractPlatformErrorCode(error, 'Products.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductsActions.updateProduct),
      exhaustMap(({ productId, request }) =>
        this.productsService.updateProduct(productId, request).pipe(
          map(() => ProductsActions.updateProductSuccess()),
          catchError((error) =>
            of(ProductsActions.updateProductFailure({
              error: extractPlatformErrorCode(error, 'Products.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteProduct$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ProductsActions.deleteProduct),
      exhaustMap(({ productId }) =>
        this.productsService.deleteProduct(productId).pipe(
          map(() => ProductsActions.deleteProductSuccess()),
          catchError((error) =>
            of(ProductsActions.deleteProductFailure({
              error: extractPlatformErrorCode(error, 'Products.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        ProductsActions.createProductSuccess,
        ProductsActions.updateProductSuccess,
        ProductsActions.deleteProductSuccess,
      ),
      switchMap(() => [ProductsActions.loadProducts({})]),
    ),
  );
}
