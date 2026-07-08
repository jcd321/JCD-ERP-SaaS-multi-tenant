import { Routes } from '@angular/router';

export const KARDEX_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./kardex-list/kardex-list.component').then((m) => m.KardexListComponent),
  },
];
