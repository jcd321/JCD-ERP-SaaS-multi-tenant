import { Routes } from '@angular/router';

export const CATEGORIES_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./locations-list/locations-list.component').then((m) => m.LocationsListComponent),
  },
];
