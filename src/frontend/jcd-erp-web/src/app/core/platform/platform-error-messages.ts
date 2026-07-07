import { HttpErrorResponse } from '@angular/common/http';

const PLATFORM_ERROR_MESSAGES: Record<string, string> = {
  'Users.LoadFailed': 'No se pudieron cargar los usuarios.',
  'Users.CreateFailed': 'No se pudo crear el usuario.',
  'Users.UpdateFailed': 'No se pudo actualizar el usuario.',
  'Roles.LoadFailed': 'No se pudieron cargar los roles.',
  'Roles.CreateFailed': 'No se pudo crear el rol.',
  'Roles.UpdateFailed': 'No se pudo actualizar el rol.',
  'Roles.DeleteFailed': 'No se pudo eliminar el rol.',
  'Roles.PermissionsLoadFailed': 'No se pudieron cargar los permisos.',
  'User.EmailAlreadyExists': 'Ya existe un usuario con ese correo en tu empresa.',
  'User.NotFound': 'Usuario no encontrado.',
  'Role.NotFound': 'Rol no encontrado.',
  'Role.NameAlreadyExists': 'Ya existe un rol con ese nombre en tu empresa.',
  'Role.IsSystem': 'Los roles de sistema no se pueden modificar ni eliminar.',
  'Role.HasUsers': 'No se puede eliminar un rol que tiene usuarios asignados.',
  'Permission.NotFound': 'Uno o más permisos seleccionados no son válidos.',
  'Auth.TenantRequired': 'No se pudo identificar tu empresa. Cierra sesión e ingresa de nuevo.',
};

export function resolvePlatformErrorMessage(error: unknown, fallbackKey: string): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 405) {
      return 'El servidor API está desactualizado. Reinicia el backend (dotnet run) e intenta de nuevo.';
    }

    if (error.status === 403) {
      return 'No tienes permisos para realizar esta acción.';
    }

    if (error.status === 404) {
      return 'Recurso no disponible en el servidor. Reinicia el backend con la última versión del código.';
    }

    const code = typeof error.error === 'object' && error.error !== null
      ? (error.error as { error?: string }).error
      : undefined;

    if (code && PLATFORM_ERROR_MESSAGES[code]) {
      return PLATFORM_ERROR_MESSAGES[code];
    }

    if (code) {
      return code;
    }
  }

  return PLATFORM_ERROR_MESSAGES[fallbackKey] ?? fallbackKey;
}
