import { Routes } from '@angular/router';

export const STOCK_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./stock-list/stock-list.component').then((m) => m.StockListComponent),
  },
];
