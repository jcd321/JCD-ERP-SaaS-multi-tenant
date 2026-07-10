import { Routes } from '@angular/router';

export const ADJUSTMENTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./adjustments-list/adjustments-list.component').then((m) => m.AdjustmentsListComponent),
  },
];
