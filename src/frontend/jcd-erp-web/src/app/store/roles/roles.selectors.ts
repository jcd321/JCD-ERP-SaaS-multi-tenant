import { createFeatureSelector, createSelector } from '@ngrx/store';

import { RolesState } from './roles.state';

export const selectRolesState = createFeatureSelector<RolesState>('roles');

export const selectAllRoles = createSelector(selectRolesState, (state) => state.items);

export const selectRolesLoading = createSelector(selectRolesState, (state) => state.loading);

export const selectRolesError = createSelector(selectRolesState, (state) => state.error);
