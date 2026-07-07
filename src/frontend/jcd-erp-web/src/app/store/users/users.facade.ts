import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { CreateUserRequest, UpdateUserRequest } from '../../features/users/users.models';
import { UsersActions } from './users.actions';
import { selectAllUsers, selectUsersError, selectUsersLoading, selectUsersSaving } from './users.selectors';

@Injectable({ providedIn: 'root' })
export class UsersFacade {
  private readonly store = inject(Store);

  readonly users = toSignal(this.store.select(selectAllUsers), { initialValue: [] });
  readonly loading = toSignal(this.store.select(selectUsersLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectUsersSaving), { initialValue: false });
  readonly error = toSignal(this.store.select(selectUsersError), { initialValue: null });

  loadUsers(): void {
    this.store.dispatch(UsersActions.loadUsers());
  }

  createUser(request: CreateUserRequest): void {
    this.store.dispatch(UsersActions.createUser({ request }));
  }

  updateUser(userId: string, request: UpdateUserRequest): void {
    this.store.dispatch(UsersActions.updateUser({ userId, request }));
  }

  deleteUser(userId: string): void {
    this.store.dispatch(UsersActions.deleteUser({ userId }));
  }
}
