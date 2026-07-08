import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import {
  getModuleLabel as resolveModuleLabel,
  getPermissionLabel as resolvePermissionLabel,
  groupPermissionsByModule,
} from '../../../shared/utils/permission-group.util';
import { RolesActions } from '../../../store/roles/roles.actions';
import { RolesFacade } from '../../../store/roles/roles.facade';
import { Role, RoleFormMode } from '../roles.models';

@Component({
  selector: 'app-roles-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './roles-list.component.html',
  styleUrl: './roles-list.component.scss',
})
export class RolesListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly rolesFacade = inject(RolesFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly roles = this.rolesFacade.roles;
  readonly permissions = this.rolesFacade.permissions;
  readonly loading = this.rolesFacade.loading;
  readonly saving = this.rolesFacade.saving;
  readonly errorMessage = this.rolesFacade.error;

  readonly groupPermissionsByModule = groupPermissionsByModule;
  readonly getPermissionLabel = (permission: Parameters<typeof resolvePermissionLabel>[0]) => {
    this.locale.locale();
    return resolvePermissionLabel(permission, (key) => this.locale.t(key));
  };
  readonly getModuleLabel = (module: string) => {
    this.locale.locale();
    return resolveModuleLabel(module, (key) => this.locale.t(key));
  };

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
    return this.formMode === 'create'
      ? this.locale.t('roles.createTitle')
      : this.locale.t('roles.editTitle');
  }

  get deleteMessage(): string {
    if (!this.roleToDelete) {
      return '';
    }

    return this.locale.t('roles.deleteMessage', { name: this.roleToDelete.name });
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

  isModuleFullySelected(items: { code: string }[]): boolean {
    if (items.length === 0) {
      return false;
    }

    const selected = this.form.controls.permissionCodes.value;
    return items.every((item) => selected.includes(item.code));
  }

  isModulePartiallySelected(items: { code: string }[]): boolean {
    const selected = this.form.controls.permissionCodes.value;
    const selectedCount = items.filter((item) => selected.includes(item.code)).length;
    return selectedCount > 0 && selectedCount < items.length;
  }

  toggleModulePermissions(items: { code: string }[], checked: boolean): void {
    const codes = items.map((item) => item.code);
    const current = this.form.controls.permissionCodes.value;
    const next = checked
      ? [...new Set([...current, ...codes])]
      : current.filter((code) => !codes.includes(code));

    this.form.controls.permissionCodes.setValue(next);
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
