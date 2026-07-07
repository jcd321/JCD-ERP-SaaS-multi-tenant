import { createFeatureSelector, createSelector } from '@ngrx/store';

import { AuthState } from './auth.state';

export const selectAuthState = createFeatureSelector<AuthState>('auth');

export const selectAuthSession = createSelector(selectAuthState, (state) => state.session);

export const selectAccessToken = createSelector(selectAuthState, (state) => state.accessToken);

export const selectRefreshToken = createSelector(selectAuthState, (state) => state.refreshToken);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (state) => !!state.accessToken,
);

export const selectAuthStatus = createSelector(selectAuthState, (state) => state.status);

export const selectAuthLoading = createSelector(
  selectAuthState,
  (state) => state.status === 'loading',
);

export const selectAuthError = createSelector(selectAuthState, (state) => state.error);

export const selectUserPermissions = createSelector(
  selectAuthSession,
  (session) => session?.permissions ?? [],
);

export const selectHasPermission = (permission: string) =>
  createSelector(selectUserPermissions, (permissions) => permissions.includes(permission));
