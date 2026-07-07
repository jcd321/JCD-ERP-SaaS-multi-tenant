const AUTH_ERROR_MESSAGES: Record<string, string> = {
  'Auth.InvalidCredentials':
    'Email, contraseña o slug de empresa incorrectos. Verifica tus datos e intenta de nuevo.',
  'Auth.TenantSlugRequired':
    'Tu email está registrado en más de una empresa. Indica el slug de la empresa (ej. jcdprogramer).',
  'Auth.TenantInactive': 'La empresa está inactiva. Contacta al administrador.',
  'Auth.LoginFailed': 'No se pudo iniciar sesión. Intenta de nuevo.',
  'Auth.RegisterFailed': 'No se pudo completar el registro. Intenta de nuevo.',
  'Auth.ForgotPasswordFailed': 'No se pudo enviar el enlace de recuperación. Intenta de nuevo.',
  'Auth.ResetPasswordFailed': 'No se pudo restablecer la contraseña. Intenta de nuevo.',
  'Auth.InvalidResetToken': 'El enlace de recuperación no es válido o ya expiró. Solicita uno nuevo.',
  'Tenant.SlugAlreadyExists': 'Ese slug de empresa ya existe. Usa otro nombre o slug.',
};

export function resolveAuthErrorMessage(code: string | null | undefined): string {
  if (!code) {
    return 'Ocurrió un error inesperado.';
  }

  return AUTH_ERROR_MESSAGES[code] ?? code;
}
