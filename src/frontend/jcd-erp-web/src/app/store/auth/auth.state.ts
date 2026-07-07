import { AuthSession } from '../../core/auth/auth.models';
import { extractPermissionsFromAccessToken } from '../../core/auth/jwt-permissions';

export const ACCESS_TOKEN_KEY = 'jcd_erp_access_token';
export const REFRESH_TOKEN_KEY = 'jcd_erp_refresh_token';
export const SESSION_KEY = 'jcd_erp_session';

export type AuthStatus = 'idle' | 'loading' | 'authenticated' | 'error';

export interface AuthState {
  session: AuthSession | null;
  accessToken: string | null;
  refreshToken: string | null;
  status: AuthStatus;
  error: string | null;
}

export function loadSessionFromStorage(): AuthSession | null {
  const raw = localStorage.getItem(SESSION_KEY);
  if (!raw) {
    return null;
  }

  try {
    const session = JSON.parse(raw) as AuthSession;
    const accessToken = localStorage.getItem(ACCESS_TOKEN_KEY);
    if (!accessToken) {
      return session;
    }

    const permissions = extractPermissionsFromAccessToken(accessToken);
    if (permissions && permissions.length > 0) {
      return { ...session, permissions };
    }

    return session;
  } catch {
    return null;
  }
}

export const initialAuthState: AuthState = {
  session: loadSessionFromStorage(),
  accessToken: localStorage.getItem(ACCESS_TOKEN_KEY),
  refreshToken: localStorage.getItem(REFRESH_TOKEN_KEY),
  status: localStorage.getItem(ACCESS_TOKEN_KEY) ? 'authenticated' : 'idle',
  error: null,
};
