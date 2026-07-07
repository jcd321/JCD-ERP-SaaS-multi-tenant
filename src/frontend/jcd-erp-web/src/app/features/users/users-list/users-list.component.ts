import { Component, inject, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Actions, ofType } from '@ngrx/effects';

import { RolesFacade } from '../../../store/roles/roles.facade';
import { UsersActions } from '../../../store/users/users.actions';
import { UsersFacade } from '../../../store/users/users.facade';
import { User, UserFormMode } from '../users.models';

@Component({
  selector: 'app-users-list',
  imports: [ReactiveFormsModule],
  templateUrl: './users-list.component.html',
  styleUrl: './users-list.component.scss',
})
export class UsersListComponent implements OnInit {
  private readonly fb = inject(NonNullableFormBuilder);
  private readonly usersFacade = inject(UsersFacade);
  private readonly rolesFacade = inject(RolesFacade);
  private readonly actions$ = inject(Actions);

  readonly users = this.usersFacade.users;
  readonly roles = this.rolesFacade.roles;
  readonly loading = this.usersFacade.loading;
  readonly saving = this.usersFacade.saving;
  readonly errorMessage = this.usersFacade.error;

  formMode: UserFormMode = null;
  editingUserId: string | null = null;

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
        ofType(UsersActions.createUserSuccess, UsersActions.updateUserSuccess),
        takeUntilDestroyed(),
      )
      .subscribe(() => this.closeForm());
  }

  ngOnInit(): void {
    this.usersFacade.loadUsers();
    this.rolesFacade.loadRoles();
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
