const AUTH_ERROR_KEYS: Record<string, string> = {
  'Auth.InvalidCredentials': 'auth.errors.invalidCredentials',
  'Auth.TenantSlugRequired': 'auth.errors.tenantSlugRequired',
  'Auth.TenantInactive': 'auth.errors.tenantInactive',
  'Auth.LoginFailed': 'auth.errors.loginFailed',
  'Auth.RegisterFailed': 'auth.errors.registerFailed',
  'Auth.ForgotPasswordFailed': 'auth.errors.forgotPasswordFailed',
  'Auth.ResetPasswordFailed': 'auth.errors.resetPasswordFailed',
  'Auth.InvalidResetToken': 'auth.errors.invalidResetToken',
  'Tenant.SlugAlreadyExists': 'auth.errors.slugAlreadyExists',
};

export function resolveAuthErrorMessage(
  code: string | null | undefined,
  translate: (key: string) => string,
): string {
  if (!code) {
    return translate('auth.errors.unexpected');
  }

  const key = AUTH_ERROR_KEYS[code];
  return key ? translate(key) : code;
}
