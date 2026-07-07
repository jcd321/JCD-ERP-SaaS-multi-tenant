import { createFeatureSelector, createSelector } from '@ngrx/store';

import { RolesState } from './roles.state';

export const selectRolesState = createFeatureSelector<RolesState>('roles');

export const selectAllRoles = createSelector(selectRolesState, (state) => state.items);

export const selectAllPermissions = createSelector(selectRolesState, (state) => state.permissions);

export const selectRolesLoading = createSelector(selectRolesState, (state) => state.loading);

export const selectRolesSaving = createSelector(selectRolesState, (state) => state.saving);

export const selectRolesError = createSelector(selectRolesState, (state) => state.error);
