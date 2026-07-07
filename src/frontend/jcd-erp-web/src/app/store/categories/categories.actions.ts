import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CategoryParentOption,
  CreateCategoryRequest,
  PaginatedCategoriesResponse,
  CategoriesQueryParams,
  UpdateCategoryRequest,
} from '../../features/categories/categories.models';

export const CategoriesActions = createActionGroup({
  source: 'Categories',
  events: {
    'Load Categories': props<{ params?: CategoriesQueryParams }>(),
    'Load Categories Success': props<{ response: PaginatedCategoriesResponse; search: string }>(),
    'Load Categories Failure': props<{ error: string }>(),

    'Load Parent Options': props<{ excludeId?: string }>(),
    'Load Parent Options Success': props<{ options: CategoryParentOption[] }>(),
    'Load Parent Options Failure': props<{ error: string }>(),

    'Create Category': props<{ request: CreateCategoryRequest }>(),
    'Create Category Success': emptyProps(),
    'Create Category Failure': props<{ error: string }>(),

    'Update Category': props<{ categoryId: string; request: UpdateCategoryRequest }>(),
    'Update Category Success': emptyProps(),
    'Update Category Failure': props<{ error: string }>(),

    'Delete Category': props<{ categoryId: string }>(),
    'Delete Category Success': emptyProps(),
    'Delete Category Failure': props<{ error: string }>(),
  },
});
