import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { CreateRoleRequest, UpdateRoleRequest } from '../../features/roles/roles.models';
import { RolesActions } from './roles.actions';
import {
  selectAllPermissions,
  selectAllRoles,
  selectRolesError,
  selectRolesLoading,
  selectRolesSaving,
} from './roles.selectors';

@Injectable({ providedIn: 'root' })
export class RolesFacade {
  private readonly store = inject(Store);

  readonly roles = toSignal(this.store.select(selectAllRoles), { initialValue: [] });
  readonly permissions = toSignal(this.store.select(selectAllPermissions), { initialValue: [] });
  readonly loading = toSignal(this.store.select(selectRolesLoading), { initialValue: false });
  readonly saving = toSignal(this.store.select(selectRolesSaving), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectRolesError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadRoles(): void {
    this.store.dispatch(RolesActions.loadRoles());
  }

  loadPermissions(): void {
    this.store.dispatch(RolesActions.loadPermissions());
  }

  createRole(request: CreateRoleRequest): void {
    this.store.dispatch(RolesActions.createRole({ request }));
  }

  updateRole(roleId: string, request: UpdateRoleRequest): void {
    this.store.dispatch(RolesActions.updateRole({ roleId, request }));
  }

  deleteRole(roleId: string): void {
    this.store.dispatch(RolesActions.deleteRole({ roleId }));
  }
}
