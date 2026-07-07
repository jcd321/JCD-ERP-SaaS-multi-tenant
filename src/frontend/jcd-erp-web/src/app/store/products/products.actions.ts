import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateProductRequest,
  PaginatedProductsResponse,
  ProductLookups,
  ProductsQueryParams,
  UpdateProductRequest,
} from '../../features/products/products.models';

export const ProductsActions = createActionGroup({
  source: 'Products',
  events: {
    'Load Products': props<{ params?: ProductsQueryParams }>(),
    'Load Products Success': props<{ response: PaginatedProductsResponse; search: string }>(),
    'Load Products Failure': props<{ error: string }>(),

    'Load Lookups': emptyProps(),
    'Load Lookups Success': props<{ lookups: ProductLookups }>(),
    'Load Lookups Failure': props<{ error: string }>(),

    'Create Product': props<{ request: CreateProductRequest }>(),
    'Create Product Success': emptyProps(),
    'Create Product Failure': props<{ error: string }>(),

    'Update Product': props<{ productId: string; request: UpdateProductRequest }>(),
    'Update Product Success': emptyProps(),
    'Update Product Failure': props<{ error: string }>(),

    'Delete Product': props<{ productId: string }>(),
    'Delete Product Success': emptyProps(),
    'Delete Product Failure': props<{ error: string }>(),
  },
});
