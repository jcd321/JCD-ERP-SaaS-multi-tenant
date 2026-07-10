import { Routes } from '@angular/router';

import { authGuard } from './core/auth/auth.guard';
import { permissionGuard } from './core/auth/permission.guard';

export const routes: Routes = [
  {
    path: 'auth',
    loadComponent: () =>
      import('./layouts/auth-layout/auth-layout.component').then(
        (m) => m.AuthLayoutComponent,
      ),
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./layouts/main-layout/main-layout.component').then(
        (m) => m.MainLayoutComponent,
      ),
    children: [
      {
        path: '',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then((m) => m.DASHBOARD_ROUTES),
      },
      {
        path: 'users',
        canActivate: [permissionGuard('users.view')],
        loadChildren: () =>
          import('./features/users/users.routes').then((m) => m.USERS_ROUTES),
      },
      {
        path: 'roles',
        canActivate: [permissionGuard('roles.view')],
        loadChildren: () =>
          import('./features/roles/roles.routes').then((m) => m.ROLES_ROUTES),
      },
      {
        path: 'settings',
        canActivate: [permissionGuard('settings.view')],
        loadChildren: () =>
          import('./features/settings/settings.routes').then((m) => m.SETTINGS_ROUTES),
      },
      {
        path: 'units',
        canActivate: [permissionGuard('units.view')],
        loadChildren: () =>
          import('./features/units/units.routes').then((m) => m.UNITS_ROUTES),
      },
      {
        path: 'categories',
        canActivate: [permissionGuard('categories.view')],
        loadChildren: () =>
          import('./features/categories/categories.routes').then((m) => m.CATEGORIES_ROUTES),
      },
      {
        path: 'brands',
        canActivate: [permissionGuard('brands.view')],
        loadChildren: () =>
          import('./features/brands/brands.routes').then((m) => m.BRANDS_ROUTES),
      },
      {
        path: 'products',
        canActivate: [permissionGuard('products.view')],
        loadChildren: () =>
          import('./features/products/products.routes').then((m) => m.PRODUCTS_ROUTES),
      },
      {
        path: 'customers',
        canActivate: [permissionGuard('customers.view')],
        loadChildren: () =>
          import('./features/customers/customers.routes').then((m) => m.CUSTOMERS_ROUTES),
      },
      {
        path: 'suppliers',
        canActivate: [permissionGuard('suppliers.view')],
        loadChildren: () =>
          import('./features/suppliers/suppliers.routes').then((m) => m.SUPPLIERS_ROUTES),
      },
      {
        path: 'warehouses',
        canActivate: [permissionGuard('warehouses.view')],
        loadChildren: () =>
          import('./features/warehouses/warehouses.routes').then((m) => m.WAREHOUSES_ROUTES),
      },
      {
        path: 'stock',
        canActivate: [permissionGuard('stock.view')],
        loadChildren: () =>
          import('./features/stock/stock.routes').then((m) => m.STOCK_ROUTES),
      },
      {
        path: 'movements',
        canActivate: [permissionGuard('movements.view')],
        loadChildren: () =>
          import('./features/movements/movements.routes').then((m) => m.MOVEMENTS_ROUTES),
      },
      {
        path: 'kardex',
        canActivate: [permissionGuard('kardex.view')],
        loadChildren: () =>
          import('./features/kardex/kardex.routes').then((m) => m.KARDEX_ROUTES),
      },
      {
        path: 'transfers',
        canActivate: [permissionGuard('transfers.view')],
        loadChildren: () =>
          import('./features/transfers/transfers.routes').then((m) => m.TRANSFERS_ROUTES),
      },
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
