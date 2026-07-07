import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  BrandsQueryParams,
  CreateBrandRequest,
  PaginatedBrandsResponse,
  UpdateBrandRequest,
} from '../../features/brands/brands.models';

export const BrandsActions = createActionGroup({
  source: 'Brands',
  events: {
    'Load Brands': props<{ params?: BrandsQueryParams }>(),
    'Load Brands Success': props<{ response: PaginatedBrandsResponse; search: string }>(),
    'Load Brands Failure': props<{ error: string }>(),

    'Create Brand': props<{ request: CreateBrandRequest }>(),
    'Create Brand Success': emptyProps(),
    'Create Brand Failure': props<{ error: string }>(),

    'Update Brand': props<{ brandId: string; request: UpdateBrandRequest }>(),
    'Update Brand Success': emptyProps(),
    'Update Brand Failure': props<{ error: string }>(),

    'Delete Brand': props<{ brandId: string }>(),
    'Delete Brand Success': emptyProps(),
    'Delete Brand Failure': props<{ error: string }>(),
  },
});
