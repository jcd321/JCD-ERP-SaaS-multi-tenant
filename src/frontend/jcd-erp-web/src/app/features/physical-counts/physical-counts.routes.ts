import { Routes } from '@angular/router';

export const PHYSICAL_COUNTS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./physical-counts-list/physical-counts-list.component').then(
        (m) => m.PhysicalCountsListComponent,
      ),
  },
];
