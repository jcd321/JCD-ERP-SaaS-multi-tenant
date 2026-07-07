import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import {
  CategoriesQueryParams,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from '../../features/categories/categories.models';
import { CategoriesActions } from './categories.actions';
import {
  selectAllCategories,
  selectCategoriesError,
  selectCategoriesLoading,
  selectCategoriesPage,
  selectCategoriesPageSize,
  selectCategoriesSaving,
  selectCategoriesSearch,
  selectCategoriesTotalCount,
  selectCategoriesTotalPages,
  selectCategoryParentOptions,
} from './categories.selectors';

@Injectable({ providedIn: 'root' })
export class CategoriesFacade {
  private readonly store = inject(Store);

  readonly categories = toSignal(this.store.select(selectAllCategories), { initialValue: [] });
  readonly parentOptions = toSignal(this.store.select(selectCategoryParentOptions), { initialValue: [] });
  readonly page = toSignal(this.store.select(selectCategoriesPage), { initialValue: 1 });
  readonly pageSize = toSignal(this.store.select(selectCategoriesPageSize), { initialValue: 20 });
  readonly totalCount = toSignal(this.store.select(selectCategoriesTotalCount), { initialValue: 0 });
  readonly totalPages = toSignal(this.store.select(selectCategoriesTotalPages), { initialValue: 0 });
  readonly search = toSignal(this.store.select(selectCategoriesSearch), { initialValue: '' });
  readonly loading = toSignal(this.store.select(selectCategoriesLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectCategoriesSaving), { initialValue: false });
  readonly error = toSignal(this.store.select(selectCategoriesError), { initialValue: null });

  loadCategories(params?: CategoriesQueryParams): void {
    this.store.dispatch(CategoriesActions.loadCategories({ params }));
  }

  loadParentOptions(excludeId?: string): void {
    this.store.dispatch(CategoriesActions.loadParentOptions({ excludeId }));
  }

  createCategory(request: CreateCategoryRequest): void {
    this.store.dispatch(CategoriesActions.createCategory({ request }));
  }

  updateCategory(categoryId: string, request: UpdateCategoryRequest): void {
    this.store.dispatch(CategoriesActions.updateCategory({ categoryId, request }));
  }

  deleteCategory(categoryId: string): void {
    this.store.dispatch(CategoriesActions.deleteCategory({ categoryId }));
  }
}
