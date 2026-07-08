import { Routes } from '@angular/router';

export const SUPPLIERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./suppliers-list/suppliers-list.component').then((m) => m.SuppliersListComponent),
  },
];
