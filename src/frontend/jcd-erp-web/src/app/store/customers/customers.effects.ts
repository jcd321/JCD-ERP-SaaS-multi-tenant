import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap, withLatestFrom } from 'rxjs';
import { Store } from '@ngrx/store';

import { resolvePlatformErrorMessage } from '../../core/platform/platform-error-messages';
import { LocaleService } from '../../core/i18n';
import { CustomersService } from '../../features/customers/customers.service';
import { CustomersActions } from './customers.actions';
import {
  selectCustomersPage,
  selectCustomersPageSize,
  selectCustomersSearch,
} from './customers.selectors';

@Injectable()
export class CustomersEffects {
  private readonly actions$ = inject(Actions);
  private readonly customersService = inject(CustomersService);
  private readonly locale = inject(LocaleService);
  private readonly store = inject(Store);

  readonly loadCustomers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomersActions.loadCustomers),
      withLatestFrom(
        this.store.select(selectCustomersPage),
        this.store.select(selectCustomersPageSize),
        this.store.select(selectCustomersSearch),
      ),
      exhaustMap(([{ params }, page, pageSize, search]) => {
        const query = {
          page: params?.page ?? page,
          pageSize: params?.pageSize ?? pageSize,
          search: params?.search ?? search,
          isActive: params?.isActive,
        };

        return this.customersService.getCustomers(query).pipe(
          map((response) =>
            CustomersActions.loadCustomersSuccess({
              response,
              search: query.search ?? '',
            }),
          ),
          catchError((error) =>
            of(CustomersActions.loadCustomersFailure({
              error: resolvePlatformErrorMessage(error, 'Customers.LoadFailed', (key) => this.locale.t(key)),
            })),
          ),
        );
      }),
    ),
  );

  readonly createCustomer$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomersActions.createCustomer),
      exhaustMap(({ request }) =>
        this.customersService.createCustomer(request).pipe(
          map(() => CustomersActions.createCustomerSuccess()),
          catchError((error) =>
            of(CustomersActions.createCustomerFailure({
              error: resolvePlatformErrorMessage(error, 'Customers.CreateFailed', (key) => this.locale.t(key)),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateCustomer$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomersActions.updateCustomer),
      exhaustMap(({ customerId, request }) =>
        this.customersService.updateCustomer(customerId, request).pipe(
          map(() => CustomersActions.updateCustomerSuccess()),
          catchError((error) =>
            of(CustomersActions.updateCustomerFailure({
              error: resolvePlatformErrorMessage(error, 'Customers.UpdateFailed', (key) => this.locale.t(key)),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteCustomer$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CustomersActions.deleteCustomer),
      exhaustMap(({ customerId }) =>
        this.customersService.deleteCustomer(customerId).pipe(
          map(() => CustomersActions.deleteCustomerSuccess()),
          catchError((error) =>
            of(CustomersActions.deleteCustomerFailure({
              error: resolvePlatformErrorMessage(error, 'Customers.DeleteFailed', (key) => this.locale.t(key)),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(
        CustomersActions.createCustomerSuccess,
        CustomersActions.updateCustomerSuccess,
        CustomersActions.deleteCustomerSuccess,
      ),
      switchMap(() => [CustomersActions.loadCustomers({})]),
    ),
  );
}
