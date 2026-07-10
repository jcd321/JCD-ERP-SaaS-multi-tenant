import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Action, Store } from '@ngrx/store';
import { switchMap, withLatestFrom } from 'rxjs';

import { AdjustmentsActions } from '../adjustments/adjustments.actions';
import { PhysicalCountsActions } from '../physical-counts/physical-counts.actions';
import { KardexActions } from '../kardex/kardex.actions';
import {
  selectKardexFromDateFilter,
  selectKardexPage,
  selectKardexPageSize,
  selectKardexProductFilter,
  selectKardexToDateFilter,
  selectKardexWarehouseFilter,
} from '../kardex/kardex.selectors';
import { MovementsActions } from '../movements/movements.actions';
import { StockActions } from '../stock/stock.actions';
import { TransfersActions } from '../transfers/transfers.actions';

@Injectable()
export class InventorySyncEffects {
  private readonly actions$ = inject(Actions);
  private readonly store = inject(Store);

  readonly refreshRelatedInventory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        AdjustmentsActions.createAdjustmentSuccess,
        TransfersActions.createTransferSuccess,
        MovementsActions.createMovementSuccess,
        PhysicalCountsActions.completePhysicalCountSuccess,
      ),
      withLatestFrom(
        this.store.select(selectKardexProductFilter),
        this.store.select(selectKardexWarehouseFilter),
        this.store.select(selectKardexFromDateFilter),
        this.store.select(selectKardexToDateFilter),
        this.store.select(selectKardexPage),
        this.store.select(selectKardexPageSize),
      ),
      switchMap(([, productId, warehouseId, fromDate, toDate, page, pageSize]) => {
        const actions: Action[] = [
          StockActions.loadStockLevels({}),
          MovementsActions.loadMovements({}),
        ];

        if (productId) {
          actions.push(
            KardexActions.loadKardex({
              params: {
                page,
                pageSize,
                productId,
                warehouseId,
                fromDate,
                toDate,
              },
            }),
          );
        }

        return actions;
      }),
    ),
  );
}
