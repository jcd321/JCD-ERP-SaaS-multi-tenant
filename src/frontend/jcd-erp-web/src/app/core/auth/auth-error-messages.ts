import { HttpErrorResponse } from '@angular/common/http';

const AUTH_ERROR_KEYS: Record<string, string> = {
  'Auth.InvalidCredentials': 'auth.errors.invalidCredentials',
  'Auth.TenantSlugRequired': 'auth.errors.tenantSlugRequired',
  'Auth.TenantInactive': 'auth.errors.tenantInactive',
  'Auth.LoginFailed': 'auth.errors.loginFailed',
  'Auth.RegisterFailed': 'auth.errors.registerFailed',
  'Auth.ForgotPasswordFailed': 'auth.errors.forgotPasswordFailed',
  'Auth.ResetPasswordFailed': 'auth.errors.resetPasswordFailed',
  'Auth.InvalidResetToken': 'auth.errors.invalidResetToken',
  'Auth.NoRefreshToken': 'auth.errors.noRefreshToken',
  'Auth.RefreshFailed': 'auth.errors.refreshFailed',
  'Tenant.SlugAlreadyExists': 'auth.errors.slugAlreadyExists',
};

export function translateAuthErrorCode(
  code: string | null | undefined,
  translate: (key: string) => string,
): string {
  if (!code) {
    return translate('auth.errors.unexpected');
  }

  const key = AUTH_ERROR_KEYS[code];
  return key ? translate(key) : translate('auth.errors.unexpected');
}

export function resolveAuthErrorMessage(
  code: string | null | undefined,
  translate: (key: string) => string,
): string {
  return translateAuthErrorCode(code, translate);
}

export function extractAuthErrorCode(
  error: unknown,
  fallbackCode: string,
): string {
  if (error instanceof HttpErrorResponse) {
    const code = typeof error.error === 'object' && error.error !== null
      ? (error.error as { error?: string }).error
      : undefined;

    if (code) {
      return code;
    }
  }

  return fallbackCode;
}
