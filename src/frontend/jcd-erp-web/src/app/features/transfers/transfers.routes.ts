import { Routes } from '@angular/router';

export const TRANSFERS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./transfers-list/transfers-list.component').then((m) => m.TransfersListComponent),
  },
];
