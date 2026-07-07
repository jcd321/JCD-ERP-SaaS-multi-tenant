import { createReducer, on } from '@ngrx/store';

import { AuthActions } from './auth.actions';
import {
  ACCESS_TOKEN_KEY,
  AuthState,
  initialAuthState,
  REFRESH_TOKEN_KEY,
  SESSION_KEY,
} from './auth.state';

function persistAuth(
  accessToken: string,
  refreshToken: string,
  session: AuthState['session'],
): void {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);
  if (session) {
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
  }
}

function clearStorage(): void {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(SESSION_KEY);
}

export const authReducer = createReducer(
  initialAuthState,

  on(AuthActions.login, AuthActions.register, AuthActions.forgotPassword, AuthActions.resetPassword, (state) => ({
    ...state,
    status: 'loading' as const,
    error: null,
  })),

  on(AuthActions.loginSuccess, (state, { response }) => {
    const session = {
      tenantId: response.tenantId,
      tenantSlug: response.tenantSlug,
      userId: response.userId,
      email: response.email,
      fullName: response.fullName,
      permissions: response.permissions,
    };

    persistAuth(response.accessToken, response.refreshToken, session);

    return {
      ...state,
      session,
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
      status: 'authenticated' as const,
      error: null,
    };
  }),

  on(AuthActions.registerSuccess, (state, { response, session }) => {
    persistAuth(response.accessToken, response.refreshToken, session);

    return {
      ...state,
      session,
      accessToken: response.accessToken,
      refreshToken: response.refreshToken,
      status: 'authenticated' as const,
      error: null,
    };
  }),

  on(AuthActions.loginFailure, AuthActions.registerFailure, AuthActions.forgotPasswordFailure, AuthActions.resetPasswordFailure, (state, { error }) => ({
    ...state,
    status: 'error' as const,
    error,
  })),

  on(AuthActions.forgotPasswordSuccess, AuthActions.resetPasswordSuccess, (state) => ({
    ...state,
    status: 'idle' as const,
    error: null,
  })),

  on(AuthActions.refreshTokenSuccess, (state, { accessToken, refreshToken }) => {
    localStorage.setItem(ACCESS_TOKEN_KEY, accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken);

    return {
      ...state,
      accessToken,
      refreshToken,
      status: 'authenticated' as const,
    };
  }),

  on(AuthActions.refreshTokenFailure, AuthActions.clearAuth, (state) => {
    clearStorage();
    return {
      ...state,
      session: null,
      accessToken: null,
      refreshToken: null,
      status: 'idle' as const,
      error: null,
    };
  }),

  on(AuthActions.logoutSuccess, (state) => {
    clearStorage();
    return {
      ...state,
      session: null,
      accessToken: null,
      refreshToken: null,
      status: 'idle' as const,
      error: null,
    };
  }),
);
