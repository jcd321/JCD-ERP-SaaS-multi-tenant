import { Component, computed, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { FormModalComponent } from '../../../shared/components/form-modal/form-modal.component';
import { LocaleService, TranslatePipe } from '../../../core/i18n';
import { AuthFacade } from '../../../store/auth/auth.facade';
import { RolesFacade } from '../../../store/roles/roles.facade';
import { UsersActions } from '../../../store/users/users.actions';
import { UsersFacade } from '../../../store/users/users.facade';
import { User, UserFormMode } from '../users.models';

@Component({
  selector: 'app-users-list',
  standalone: true,
  imports: [ReactiveFormsModule, FormModalComponent, ConfirmDialogComponent, TranslatePipe],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.scss',
})
export class UsersListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly usersFacade = inject(UsersFacade);
  private readonly rolesFacade = inject(RolesFacade);
  private readonly authFacade = inject(AuthFacade);
  private readonly actions$ = inject(Actions);
  private readonly locale = inject(LocaleService);

  readonly users = this.usersFacade.users;
  readonly roles = this.rolesFacade.roles;
  readonly loading = this.usersFacade.loading;
  readonly saving = this.usersFacade.saving;
  readonly errorMessage = this.usersFacade.error;

  readonly canDelete = computed(() =>
    (this.authFacade.session()?.permissions ?? []).includes('users.delete'),
  );

  formMode: UserFormMode = null;
  editingUserId: string | null = null;
  userToDelete: User | null = null;

  readonly form = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    firstName: ['', [Validators.required]],
    lastName: [''],
    isActive: [true],
    roleIds: this.fb.control<string[]>([]),
  });

  constructor() {
    this.actions$
      .pipe(
        ofType(UsersActions.createUserSuccess, UsersActions.updateUserSuccess, UsersActions.deleteUserSuccess),
        takeUntilDestroyed(),
      )
      .subscribe(() => {
        this.closeForm();
        this.userToDelete = null;
      });
  }

  ngOnInit(): void {
    this.usersFacade.loadUsers();
    this.rolesFacade.loadRoles();
  }

  get modalTitle(): string {
    return this.formMode === 'create'
      ? this.locale.t('users.createTitle')
      : this.locale.t('users.editTitle');
  }

  get deleteMessage(): string {
    if (!this.userToDelete) {
      return '';
    }

    return this.locale.t('users.deleteMessage', { name: this.userToDelete.fullName });
  }

  canDeleteUser(user: User): boolean {
    return this.canDelete() && user.id !== this.authFacade.session()?.userId;
  }

  openCreateForm(): void {
    this.formMode = 'create';
    this.editingUserId = null;
    this.form.reset({
      email: '',
      password: '',
      firstName: '',
      lastName: '',
      isActive: true,
      roleIds: [],
    });
    this.form.controls.email.enable();
    this.form.controls.password.setValidators([Validators.required, Validators.minLength(8)]);
    this.form.controls.password.updateValueAndValidity();
  }

  openEditForm(user: User): void {
    this.formMode = 'edit';
    this.editingUserId = user.id;

    const roleIds = this.roles()
      .filter((role) => user.roles.includes(role.name))
      .map((role) => role.id);

    this.form.reset({
      email: user.email,
      password: '',
      firstName: user.firstName,
      lastName: user.lastName,
      isActive: user.isActive,
      roleIds,
    });

    this.form.controls.email.disable();
    this.form.controls.password.clearValidators();
    this.form.controls.password.updateValueAndValidity();
  }

  closeForm(): void {
    this.formMode = null;
    this.editingUserId = null;
    this.form.controls.email.enable();
  }

  openDeleteDialog(user: User): void {
    if (!this.canDeleteUser(user)) {
      return;
    }

    this.userToDelete = user;
  }

  closeDeleteDialog(): void {
    this.userToDelete = null;
  }

  confirmDelete(): void {
    if (!this.userToDelete) {
      return;
    }

    this.usersFacade.deleteUser(this.userToDelete.id);
  }

  toggleRole(roleId: string, checked: boolean): void {
    const current = this.form.controls.roleIds.value;
    const next = checked ? [...current, roleId] : current.filter((id) => id !== roleId);
    this.form.controls.roleIds.setValue(next);
  }

  isRoleSelected(roleId: string): boolean {
    return this.form.controls.roleIds.value.includes(roleId);
  }

  submit(): void {
    if (this.form.invalid || !this.formMode) {
      this.form.markAllAsTouched();
      return;
    }

    const { email, password, firstName, lastName, isActive, roleIds } = this.form.getRawValue();

    if (this.formMode === 'create') {
      this.usersFacade.createUser({
        email,
        password,
        firstName,
        lastName,
        roleIds,
      });
      return;
    }

    if (this.editingUserId) {
      this.usersFacade.updateUser(this.editingUserId, {
        firstName,
        lastName,
        isActive,
        roleIds,
      });
    }
  }
}
