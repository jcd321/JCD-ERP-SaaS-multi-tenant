import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { Permission, Role, RoleFormMode } from '../roles.models';
import { RolesActions } from '../../../store/roles/roles.actions';
import { RolesFacade } from '../../../store/roles/roles.facade';

const PERMISSION_LABELS: Record<string, string> = {
  'users.view': 'Ver usuarios',
  'users.create': 'Crear usuarios',
  'users.update': 'Editar usuarios',
  'users.delete': 'Eliminar usuarios',
  'roles.view': 'Ver roles',
  'roles.create': 'Crear roles',
  'roles.update': 'Editar roles',
  'roles.delete': 'Eliminar roles',
  'settings.view': 'Ver configuración',
  'settings.update': 'Editar configuración',
  'audit.view': 'Ver auditoría',
};

@Component({
  selector: 'app-roles-list',
  imports: [ReactiveFormsModule],
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

  formMode: RoleFormMode = null;
  editingRoleId: string | null = null;

  readonly form = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(100)]],
    description: [''],
    permissionCodes: this.fb.control<string[]>([]),
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(RolesActions.createRoleSuccess, RolesActions.updateRoleSuccess),
        takeUntilDestroyed(),
      )
      .subscribe(() => this.closeForm());
  }

  ngOnInit(): void {
    this.rolesFacade.loadRoles();
    this.rolesFacade.loadPermissions();
  }

  permissionLabel(permission: Permission): string {
    return PERMISSION_LABELS[permission.code] ?? permission.description ?? permission.code;
  }

  permissionsByModule(): { module: string; items: Permission[] }[] {
    const grouped = new Map<string, Permission[]>();

    for (const permission of this.permissions()) {
      const list = grouped.get(permission.module) ?? [];
      list.push(permission);
      grouped.set(permission.module, list);
    }

    return Array.from(grouped.entries()).map(([module, items]) => ({ module, items }));
  }

  moduleLabel(module: string): string {
    const labels: Record<string, string> = {
      users: 'Usuarios',
      roles: 'Roles',
      settings: 'Configuración',
      audit: 'Auditoría',
    };
    return labels[module] ?? module;
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingRoleId = null;
    this.form.reset({ name: '', description: '', permissionCodes: [] });
  }

  openEditForm(role: Role): void {
    if (role.isSystem) return;

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

  confirmDelete(role: Role): void {
    if (role.isSystem) return;

    const confirmed = window.confirm(
      `¿Eliminar el rol "${role.name}"? Esta acción no se puede deshacer.`,
    );
    if (confirmed) {
      this.rolesFacade.deleteRole(role.id);
    }
  }
}
