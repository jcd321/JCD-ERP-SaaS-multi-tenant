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

  on(RolesActions.loadPermissionsSuccess, (state, { permissions }) => ({
    ...state,
    permissions,
  })),

  on(RolesActions.createRole, RolesActions.updateRole, RolesActions.deleteRole, (state) => ({
    ...state,
    saving: true,
    error: null,
  })),

  on(
    RolesActions.createRoleSuccess,
    RolesActions.updateRoleSuccess,
    RolesActions.deleteRoleSuccess,
    (state) => ({
      ...state,
      saving: false,
      error: null,
    }),
  ),

  on(
    RolesActions.createRoleFailure,
    RolesActions.updateRoleFailure,
    RolesActions.deleteRoleFailure,
    (state, { error }) => ({
      ...state,
      saving: false,
      error,
    }),
  ),
);
