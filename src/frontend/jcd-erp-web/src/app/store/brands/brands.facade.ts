import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { BrandsQueryParams, CreateBrandRequest, UpdateBrandRequest } from '../../features/brands/brands.models';
import { BrandsActions } from './brands.actions';
import {
  selectAllBrands,
  selectBrandsError,
  selectBrandsLoading,
  selectBrandsPage,
  selectBrandsPageSize,
  selectBrandsSaving,
  selectBrandsSearch,
  selectBrandsTotalCount,
  selectBrandsTotalPages,
} from './brands.selectors';

@Injectable({ providedIn: 'root' })
export class BrandsFacade {
  private readonly store = inject(Store);

  readonly brands = toSignal(this.store.select(selectAllBrands), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectBrandsPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectBrandsPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectBrandsTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectBrandsTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectBrandsSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectBrandsLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectBrandsSaving), { initialValue: false });
  readonly error = toSignal(this.store.select(selectBrandsError), { initialValue: null });

  loadBrands(params?: BrandsQueryParams): void {
    this.store.dispatch(BrandsActions.loadBrands({ params }));
  }

  createBrand(request: CreateBrandRequest): void {
    this.store.dispatch(BrandsActions.createBrand({ request }));
  }

  updateBrand(brandId: string, request: UpdateBrandRequest): void {
    this.store.dispatch(BrandsActions.updateBrand({ brandId, request }));
  }

  deleteBrand(brandId: string): void {
    this.store.dispatch(BrandsActions.deleteBrand({ brandId }));
  }
}
