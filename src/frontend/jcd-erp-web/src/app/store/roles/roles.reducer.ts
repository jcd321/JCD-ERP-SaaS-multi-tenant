import { createReducer, on } from '@ngrx/store';

import { RolesActions } from './roles.actions';
import { initialRolesState } from './roles.state';

export const rolesReducer = createReducer(
  initialRolesState,

  on(RolesActions.loadRoles, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(RolesActions.loadRolesSuccess, (state, { roles }) => ({
    ...state,
    items: roles,
    loading: false,
    error: null,
  })),

  on(RolesActions.loadRolesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),
);
