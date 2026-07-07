import { HttpErrorResponse } from '@angular/common/http';

const PLATFORM_ERROR_KEYS: Record<string, string> = {
  'Users.LoadFailed': 'errors.usersLoadFailed',
  'Users.CreateFailed': 'errors.usersCreateFailed',
  'Users.UpdateFailed': 'errors.usersUpdateFailed',
  'Users.DeleteFailed': 'errors.usersDeleteFailed',
  'Roles.LoadFailed': 'errors.rolesLoadFailed',
  'Roles.CreateFailed': 'errors.rolesCreateFailed',
  'Roles.UpdateFailed': 'errors.rolesUpdateFailed',
  'Roles.DeleteFailed': 'errors.rolesDeleteFailed',
  'Roles.PermissionsLoadFailed': 'errors.rolesPermissionsLoadFailed',
  'Units.LoadFailed': 'errors.unitsLoadFailed',
  'Units.CreateFailed': 'errors.unitsCreateFailed',
  'Units.UpdateFailed': 'errors.unitsUpdateFailed',
  'Units.DeleteFailed': 'errors.unitsDeleteFailed',
  'Categories.LoadFailed': 'errors.categoriesLoadFailed',
  'Categories.CreateFailed': 'errors.categoriesCreateFailed',
  'Categories.UpdateFailed': 'errors.categoriesUpdateFailed',
  'Categories.DeleteFailed': 'errors.categoriesDeleteFailed',
  'Categories.ParentOptionsLoadFailed': 'errors.categoriesParentOptionsLoadFailed',
  'User.EmailAlreadyExists': 'errors.userEmailAlreadyExists',
  'User.NotFound': 'errors.userNotFound',
  'User.CannotDeleteSelf': 'errors.userCannotDeleteSelf',
  'Role.NotFound': 'errors.roleNotFound',
  'Role.NameAlreadyExists': 'errors.roleNameAlreadyExists',
  'Role.IsSystem': 'errors.roleIsSystem',
  'Role.HasUsers': 'errors.roleHasUsers',
  'Unit.NotFound': 'errors.unitNotFound',
  'Unit.CodeAlreadyExists': 'errors.unitCodeAlreadyExists',
  'Unit.CodeRequired': 'errors.unitCodeRequired',
  'Unit.NameRequired': 'errors.unitNameRequired',
  'Category.NotFound': 'errors.categoryNotFound',
  'Category.NameAlreadyExists': 'errors.categoryNameAlreadyExists',
  'Category.NameRequired': 'errors.categoryNameRequired',
  'Category.ParentNotFound': 'errors.categoryParentNotFound',
  'Category.CannotBeOwnParent': 'errors.categoryCannotBeOwnParent',
  'Category.HasChildren': 'errors.categoryHasChildren',
  'Permission.NotFound': 'errors.permissionNotFound',
  'Auth.TenantRequired': 'errors.tenantRequired',
};

const FALLBACK_KEYS: Record<string, string> = {
  'Users.LoadFailed': 'errors.usersLoadFailed',
  'Users.CreateFailed': 'errors.usersCreateFailed',
  'Users.UpdateFailed': 'errors.usersUpdateFailed',
  'Users.DeleteFailed': 'errors.usersDeleteFailed',
  'Roles.LoadFailed': 'errors.rolesLoadFailed',
  'Roles.CreateFailed': 'errors.rolesCreateFailed',
  'Roles.UpdateFailed': 'errors.rolesUpdateFailed',
  'Roles.DeleteFailed': 'errors.rolesDeleteFailed',
  'Roles.PermissionsLoadFailed': 'errors.rolesPermissionsLoadFailed',
  'Units.LoadFailed': 'errors.unitsLoadFailed',
  'Units.CreateFailed': 'errors.unitsCreateFailed',
  'Units.UpdateFailed': 'errors.unitsUpdateFailed',
  'Units.DeleteFailed': 'errors.unitsDeleteFailed',
  'Categories.LoadFailed': 'errors.categoriesLoadFailed',
  'Categories.CreateFailed': 'errors.categoriesCreateFailed',
  'Categories.UpdateFailed': 'errors.categoriesUpdateFailed',
  'Categories.DeleteFailed': 'errors.categoriesDeleteFailed',
  'Categories.ParentOptionsLoadFailed': 'errors.categoriesParentOptionsLoadFailed',
};

export function resolvePlatformErrorMessage(
  error: unknown,
  fallbackKey: string,
  translate: (key: string) => string,
): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 405) {
      return translate('errors.apiOutdated');
    }

    if (error.status === 403) {
      return translate('errors.forbidden');
    }

    if (error.status === 404) {
      return translate('errors.notFound');
    }

    const code = typeof error.error === 'object' && error.error !== null
      ? (error.error as { error?: string }).error
      : undefined;

    if (code) {
      const key = PLATFORM_ERROR_KEYS[code];
      if (key) {
        return translate(key);
      }

      return code;
    }
  }

  const fallback = FALLBACK_KEYS[fallbackKey];
  return fallback ? translate(fallback) : fallbackKey;
}
