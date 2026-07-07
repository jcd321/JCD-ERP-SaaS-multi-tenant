import { createReducer, on } from '@ngrx/store';

import { UsersActions } from './users.actions';
import { initialUsersState } from './users.state';

export const usersReducer = createReducer(
  initialUsersState,

  on(UsersActions.loadUsers, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(UsersActions.loadUsersSuccess, (state, { users }) => ({
    ...state,
    items: users,
    loading: false,
    error: null,
  })),

  on(UsersActions.loadUsersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),

  on(UsersActions.createUser, UsersActions.updateUser, UsersActions.deleteUser, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(UsersActions.createUserSuccess, UsersActions.updateUserSuccess, UsersActions.deleteUserSuccess, (state) => ({
    ...state,
    saving: false,
    error: null,
  })),

  on(
    UsersActions.createUserFailure,
    UsersActions.updateUserFailure,
    UsersActions.deleteUserFailure,
    (state, { error }) => ({
    ...state,
    saving: false,
    error,
  })),
);
