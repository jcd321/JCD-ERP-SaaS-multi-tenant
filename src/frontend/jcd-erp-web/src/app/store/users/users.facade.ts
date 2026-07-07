import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { UsersActions } from './users.actions';
import { selectAllUsers, selectUsersError, selectUsersLoading } from './users.selectors';

@Injectable({ providedIn: 'root' })
export class UsersFacade {
  private readonly store = inject(Store);

  readonly users = toSignal(this.store.select(selectAllUsers), { initialValue: [] });
  readonly loading = toSignal(this.store.select(selectUsersLoading), { initialValue: false });
  readonly error = toSignal(this.store.select(selectUsersError), { initialValue: null });

  loadUsers(): void {
    this.store.dispatch(UsersActions.loadUsers());
  }
}
