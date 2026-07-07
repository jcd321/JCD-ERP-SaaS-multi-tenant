import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { RolesActions } from './roles.actions';
import { selectAllRoles, selectRolesError, selectRolesLoading } from './roles.selectors';

@Injectable({ providedIn: 'root' })
export class RolesFacade {
  private readonly store = inject(Store);

  readonly roles = toSignal(this.store.select(selectAllRoles), { initialValue: [] });
  readonly loading = toSignal(this.store.select(selectRolesLoading), { initialValue: false });
  readonly error = toSignal(this.store.select(selectRolesError), { initialValue: null });

  loadRoles(): void {
    this.store.dispatch(RolesActions.loadRoles());
  }
}
