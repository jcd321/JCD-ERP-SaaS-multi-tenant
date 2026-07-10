import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { switchMap } from 'rxjs';

import { AdjustmentsActions } from '../adjustments/adjustments.actions';
import { KardexActions } from '../kardex/kardex.actions';
import { MovementsActions } from '../movements/movements.actions';
import { StockActions } from '../stock/stock.actions';
import { TransfersActions } from '../transfers/transfers.actions';

@Injectable()
export class InventorySyncEffects {
  private readonly actions$ = inject(Actions);

  readonly refreshRelatedInventory$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        AdjustmentsActions.createAdjustmentSuccess,
        TransfersActions.createTransferSuccess,
        MovementsActions.createMovementSuccess,
      ),
      switchMap(() => [
        StockActions.loadStockLevels({}),
        MovementsActions.loadMovements({}),
        KardexActions.loadKardex({}),
      ]),
    ),
  );
}
