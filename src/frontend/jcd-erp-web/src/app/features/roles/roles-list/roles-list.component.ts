import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import {
  getModuleLabel,
  getPermissionLabel,
  groupPermissionsByModule,
} from '../../../shared/utils/permission-group.util';
import { RolesActions } from '../../../store/roles/roles.actions';
import { RolesFacade } from '../../../store/roles/roles.facade';
import { Role, RoleFormMode } from '../roles.models';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent],
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.scss',
})
export class RolesListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly rolesFacade = inject(RolesFacade);
  private readonly actions$ = inject(Actions);

  readonly roles = this.rolesFacade.roles;
  readonly permissions = this.rolesFacade.permissions;
  readonly loading = this.rolesFacade.loading;
  readonly saving = this.rolesFacade.saving;
  readonly errorMessage = this.rolesFacade.error;

  readonly getPermissionLabel = getPermissionLabel;
  readonly getModuleLabel = getModuleLabel;
  readonly groupPermissionsByModule = groupPermissionsByModule;

  formMode: RoleFormMode = null;
  editingRoleId: string | null = null;
  roleToDelete: Role | null = null;

  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    permissionCodes: this.fb.control<string[]>([]),
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(RolesActions.createRoleSuccess, RolesActions.updateRoleSuccess, RolesActions.deleteRoleSuccess),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.roleToDelete = null;
      });
  }

  ngOnInit(): void {
    this.rolesFacade.loadRoles();
    this.rolesFacade.loadPermissions();
  }

  get modalTitle(): string {
    return this.formMode === 'create' ? 'Crear rol' : 'Editar rol';
  }

  get deleteMessage(): string {
    if (!this.roleToDelete) {
      return '';
    }

    return `¿Eliminar el rol "${this.roleToDelete.name}"? Esta acción no se puede deshacer.`;
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingRoleId = null;
    this.form.reset({ name: '', description: '', permissionCodes: [] });
  }

  openEditForm(role: Role): void {
    if (role.isSystem) {
      return;
    }

    this.formMode = 'edit';
    this.editingRoleId = role.id;
    this.form.reset({
      name: role.name,
      description: role.description ?? '',
      permissionCodes: [...role.permissions],
    });
  }

  closeForm(): void {
    this.formMode = null;
    this.editingRoleId = null;
  }

  togglePermission(code: string, checked: boolean): void {
    const current = this.form.controls.permissionCodes.value;
    const next = checked ? [...current, code] : current.filter((c) => c !== code);
    this.form.controls.permissionCodes.setValue(next);
  }

  isPermissionSelected(code: string): boolean {
    return this.form.controls.permissionCodes.value.includes(code);
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { name, description, permissionCodes } = this.form.getRawValue();
    const payload = {
      name,
      description: description || null,
      permissionCodes,
    };

    if (this.formMode === 'create') {
      this.rolesFacade.createRole(payload);
      return;
    }

    if (this.editingRoleId) {
      this.rolesFacade.updateRole(this.editingRoleId, payload);
    }
  }

  openDeleteDialog(role: Role): void {
    if (role.isSystem) {
      return;
    }

    this.roleToDelete = role;
  }

  closeDeleteDialog(): void {
    this.roleToDelete = null;
  }

  confirmDelete(): void {
    if (!this.roleToDelete) {
      return;
    }

    this.rolesFacade.deleteRole(this.roleToDelete.id);
  }
}
