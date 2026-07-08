import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { CategoriesService } from '../../features/categories/categories.service';
import { CategoriesActions } from './categories.actions';
import {
  selectCategoriesPage,
  selectCategoriesPageSize,
  selectCategoriesSearch,
} from './categories.selectors';

@Injectable()
export class CategoriesEffects {
  private readonly actions$ = inject(Actions);
  private readonly categoriesService = inject(CategoriesService);
  private readonly store = inject(Store);

  readonly loadCategories$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CategoriesActions.loadCategories),
      withLatestFrom(
        this.store.select(selectCategoriesPage),
        this.store.select(selectCategoriesPageSize),
        this.store.select(selectCategoriesSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.categoriesService.getCategories(query).pipe(
          map((response) =>
            CategoriesActions.loadCategoriesSuccess({ response, search: query.search ?? '' }),
          ),
          catchError((error) =>
            of(CategoriesActions.loadCategoriesFailure({
              error: extractPlatformErrorCode(error, 'Categories.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadParentOptions$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CategoriesActions.loadParentOptions),
      exhaustMap(({ excludeId }) =>
        this.categoriesService.getParentOptions(excludeId).pipe(
          map((options) => CategoriesActions.loadParentOptionsSuccess({ options })),
          catchError((error) =>
            of(CategoriesActions.loadParentOptionsFailure({
              error: extractPlatformErrorCode(error, 'Categories.ParentOptionsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createCategory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CategoriesActions.createCategory),
      exhaustMap(({ request }) =>
        this.categoriesService.createCategory(request).pipe(
          map(() => CategoriesActions.createCategorySuccess()),
          catchError((error) =>
            of(CategoriesActions.createCategoryFailure({
              error: extractPlatformErrorCode(error, 'Categories.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateCategory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CategoriesActions.updateCategory),
      exhaustMap(({ categoryId, request }) =>
        this.categoriesService.updateCategory(categoryId, request).pipe(
          map(() => CategoriesActions.updateCategorySuccess()),
          catchError((error) =>
            of(CategoriesActions.updateCategoryFailure({
              error: extractPlatformErrorCode(error, 'Categories.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteCategory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CategoriesActions.deleteCategory),
      exhaustMap(({ categoryId }) =>
        this.categoriesService.deleteCategory(categoryId).pipe(
          map(() => CategoriesActions.deleteCategorySuccess()),
          catchError((error) =>
            of(CategoriesActions.deleteCategoryFailure({
              error: extractPlatformErrorCode(error, 'Categories.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        CategoriesActions.createCategorySuccess,
        CategoriesActions.updateCategorySuccess,
        CategoriesActions.deleteCategorySuccess,
      ),
      switchMap(() => [
        CategoriesActions.loadCategories({}),
        CategoriesActions.loadParentOptions({}),
      ]),
    ),
  );
}
