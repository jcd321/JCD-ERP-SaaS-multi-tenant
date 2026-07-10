import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { PhysicalCountsService } from '../../features/physical-counts/physical-counts.service';
import { PhysicalCountsActions } from './physical-counts.actions';
import {
  selectPhysicalCountsPage,
  selectPhysicalCountsPageSize,
  selectPhysicalCountsSearch,
  selectPhysicalCountsStatusFilter,
  selectPhysicalCountsWarehouseFilter,
} from './physical-counts.selectors';

@Injectable()
export class PhysicalCountsEffects {
  private readonly actions$ = inject(Actions);
  private readonly physicalCountsService = inject(PhysicalCountsService);
  private readonly store = inject(Store);

  readonly loadPhysicalCounts$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.loadPhysicalCounts),
      withLatestFrom(
        this.store.select(selectPhysicalCountsPage),
        this.store.select(selectPhysicalCountsPageSize),
        this.store.select(selectPhysicalCountsSearch),
        this.store.select(selectPhysicalCountsWarehouseFilter),
        this.store.select(selectPhysicalCountsStatusFilter),
      ),
      exhaustMap(([{ params }, page, pageSize, search, warehouseId, status]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          warehouseId: params?.warehouseId !== undefined ? params.warehouseId : warehouseId ?? undefined,
          status: params?.status !== undefined ? params.status : status ?? undefined,
        };

        return this.physicalCountsService.getPhysicalCounts(query).pipe(
          map((response) =>
            PhysicalCountsActions.loadPhysicalCountsSuccess({
              response,
              filters: {
                search: query.search ?? '',
                warehouseId: query.warehouseId ?? undefined,
                status: query.status ?? undefined,
              },
            }),
          ),
          catchError((error) =>
            of(PhysicalCountsActions.loadPhysicalCountsFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.LoadFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly loadPhysicalCount$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.loadPhysicalCount),
      exhaustMap(({ id }) =>
        this.physicalCountsService.getById(id).pipe(
          map((count) => PhysicalCountsActions.loadPhysicalCountSuccess({ count })),
          catchError((error) =>
            of(PhysicalCountsActions.loadPhysicalCountFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.LoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly loadPhysicalCountLookups$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.loadPhysicalCountLookups),
      exhaustMap(() =>
        this.physicalCountsService.getLookups().pipe(
          map((lookups) => PhysicalCountsActions.loadPhysicalCountLookupsSuccess({ lookups })),
          catchError((error) =>
            of(PhysicalCountsActions.loadPhysicalCountLookupsFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.LookupsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createPhysicalCount$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.createPhysicalCount),
      exhaustMap(({ request }) =>
        this.physicalCountsService.createPhysicalCount(request).pipe(
          map((response) => PhysicalCountsActions.createPhysicalCountSuccess({ id: response.id })),
          catchError((error) =>
            of(PhysicalCountsActions.createPhysicalCountFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updatePhysicalCountLines$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.updatePhysicalCountLines),
      exhaustMap(({ countId, lines }) =>
        this.physicalCountsService.updateLines(countId, lines).pipe(
          map(() => PhysicalCountsActions.updatePhysicalCountLinesSuccess()),
          catchError((error) =>
            of(PhysicalCountsActions.updatePhysicalCountLinesFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly completePhysicalCount$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.completePhysicalCount),
      exhaustMap(({ countId, lines }) => {
        const complete$ = this.physicalCountsService.complete(countId).pipe(
          map(() => PhysicalCountsActions.completePhysicalCountSuccess()),
          catchError((error) =>
            of(PhysicalCountsActions.completePhysicalCountFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.CompleteFailed'),
            })),
          ),
        );

        if (!lines?.length) {
          return complete$;
        }

        return this.physicalCountsService.updateLines(countId, lines).pipe(
          switchMap(() => complete$),
          catchError((error) =>
            of(PhysicalCountsActions.completePhysicalCountFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.CompleteFailed'),
            })),
          ),
        );
      }),
    ),
  );

  readonly cancelPhysicalCount$ = createEffect(() =>
    this.actions$.pipe(
      ofType(PhysicalCountsActions.cancelPhysicalCount),
      exhaustMap(({ countId }) =>
        this.physicalCountsService.cancel(countId).pipe(
          map(() => PhysicalCountsActions.cancelPhysicalCountSuccess()),
          catchError((error) =>
            of(PhysicalCountsActions.cancelPhysicalCountFailure({
              error: extractPlatformErrorCode(error, 'PhysicalCounts.CancelFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        PhysicalCountsActions.createPhysicalCountSuccess,
        PhysicalCountsActions.updatePhysicalCountLinesSuccess,
        PhysicalCountsActions.completePhysicalCountSuccess,
        PhysicalCountsActions.cancelPhysicalCountSuccess,
      ),
      switchMap(() => [PhysicalCountsActions.loadPhysicalCounts({})]),
    ),
  );
}
