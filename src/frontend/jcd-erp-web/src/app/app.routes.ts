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
    ],
  },
  {
    path: '**',
    redirectTo: '',
  },
];
