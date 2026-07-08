import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  CreateProductRequest,
  ProductsQueryParams,
  UpdateProductRequest,
} from '../../features/products/products.models';
import { ProductsActions } from './products.actions';
import {
  selectAllProducts,
  selectProductLookups,
  selectProductsError,
  selectProductsLoading,
  selectProductsPage,
  selectProductsPageSize,
  selectProductsSaving,
  selectProductsSearch,
  selectProductsTotalCount,
  selectProductsTotalPages,
} from './products.selectors';

@Injectable({ providedIn: 'root' })
export class ProductsFacade {
  private readonly store = inject(Store);

  readonly products = toSignal(this.store.select(selectAllProducts), { initialValue: [] });
  readonly lookups = toSignal(this.store.select(selectProductLookups), {
    initialValue: { categories: [], brands: [], units: [] },
  });
  readonly page = toSignal(this.store.select(selectProductsPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectProductsPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectProductsTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectProductsTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectProductsSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectProductsLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectProductsSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectProductsError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadProducts(params?: ProductsQueryParams): void {
    this.store.dispatch(ProductsActions.loadProducts({ params }));
  }

  loadLookups(): void {
    this.store.dispatch(ProductsActions.loadLookups());
  }

  createProduct(request: CreateProductRequest): void {
    this.store.dispatch(ProductsActions.createProduct({ request }));
  }

  updateProduct(productId: string, request: UpdateProductRequest): void {
    this.store.dispatch(ProductsActions.updateProduct({ productId, request }));
  }

  deleteProduct(productId: string): void {
    this.store.dispatch(ProductsActions.deleteProduct({ productId }));
  }
}
