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
  'Settings.LoadFailed': 'errors.settingsLoadFailed',
  'Units.LoadFailed': 'errors.unitsLoadFailed',
  'Units.CreateFailed': 'errors.unitsCreateFailed',
  'Units.UpdateFailed': 'errors.unitsUpdateFailed',
  'Units.DeleteFailed': 'errors.unitsDeleteFailed',
  'Categories.LoadFailed': 'errors.categoriesLoadFailed',
  'Categories.CreateFailed': 'errors.categoriesCreateFailed',
  'Categories.UpdateFailed': 'errors.categoriesUpdateFailed',
  'Categories.DeleteFailed': 'errors.categoriesDeleteFailed',
  'Categories.ParentOptionsLoadFailed': 'errors.categoriesParentOptionsLoadFailed',
  'Brands.LoadFailed': 'errors.brandsLoadFailed',
  'Brands.CreateFailed': 'errors.brandsCreateFailed',
  'Brands.UpdateFailed': 'errors.brandsUpdateFailed',
  'Brands.DeleteFailed': 'errors.brandsDeleteFailed',
  'Products.LoadFailed': 'errors.productsLoadFailed',
  'Products.LookupsLoadFailed': 'errors.productsLookupsLoadFailed',
  'Products.CreateFailed': 'errors.productsCreateFailed',
  'Products.UpdateFailed': 'errors.productsUpdateFailed',
  'Products.DeleteFailed': 'errors.productsDeleteFailed',
  'Customers.LoadFailed': 'errors.customersLoadFailed',
  'Customers.CreateFailed': 'errors.customersCreateFailed',
  'Customers.UpdateFailed': 'errors.customersUpdateFailed',
  'Customers.DeleteFailed': 'errors.customersDeleteFailed',
  'Suppliers.LoadFailed': 'errors.suppliersLoadFailed',
  'Suppliers.CreateFailed': 'errors.suppliersCreateFailed',
  'Suppliers.UpdateFailed': 'errors.suppliersUpdateFailed',
  'Suppliers.DeleteFailed': 'errors.suppliersDeleteFailed',
  'User.NotFound': 'errors.userNotFound',
  'User.EmailAlreadyExists': 'errors.userEmailAlreadyExists',
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
  'Brand.NotFound': 'errors.brandNotFound',
  'Brand.CodeAlreadyExists': 'errors.brandCodeAlreadyExists',
  'Brand.CodeRequired': 'errors.brandCodeRequired',
  'Brand.NameRequired': 'errors.brandNameRequired',
  'Product.NotFound': 'errors.productNotFound',
  'Product.SkuAlreadyExists': 'errors.productSkuAlreadyExists',
  'Product.SkuRequired': 'errors.productSkuRequired',
  'Product.NameRequired': 'errors.productNameRequired',
  'Product.CategoryRequired': 'errors.productCategoryRequired',
  'Product.CategoryNotFound': 'errors.productCategoryNotFound',
  'Product.BrandNotFound': 'errors.productBrandNotFound',
  'Product.UnitRequired': 'errors.productUnitRequired',
  'Product.UnitNotFound': 'errors.productUnitNotFound',
  'Customer.NotFound': 'errors.customerNotFound',
  'Customer.CodeAlreadyExists': 'errors.customerCodeAlreadyExists',
  'Customer.TaxIdAlreadyExists': 'errors.customerTaxIdAlreadyExists',
  'Customer.CodeRequired': 'errors.customerCodeRequired',
  'Customer.LegalNameRequired': 'errors.customerLegalNameRequired',
  'Supplier.NotFound': 'errors.supplierNotFound',
  'Supplier.CodeAlreadyExists': 'errors.supplierCodeAlreadyExists',
  'Supplier.TaxIdAlreadyExists': 'errors.supplierTaxIdAlreadyExists',
  'Supplier.CodeRequired': 'errors.supplierCodeRequired',
  'Supplier.LegalNameRequired': 'errors.supplierLegalNameRequired',
  'Permission.NotFound': 'errors.permissionNotFound',
  'Auth.TenantRequired': 'errors.tenantRequired',
  '__HTTP_FORBIDDEN__': 'errors.forbidden',
  '__HTTP_NOT_FOUND__': 'errors.notFound',
  '__HTTP_API_OUTDATED__': 'errors.apiOutdated',
};

export function extractPlatformErrorCode(error: unknown, fallbackKey: string): string {
  if (error instanceof HttpErrorResponse) {
    if (error.status === 405) {
      return '__HTTP_API_OUTDATED__';
    }

    if (error.status === 403) {
      return '__HTTP_FORBIDDEN__';
    }

    if (error.status === 404) {
      return '__HTTP_NOT_FOUND__';
    }

    const code = typeof error.error === 'object' && error.error !== null
      ? (error.error as { error?: string }).error
      : undefined;

    if (code) {
      return code;
    }
  }

  return fallbackKey;
}

export function translatePlatformErrorCode(
  code: string,
  translate: (key: string) => string,
): string {
  const key = PLATFORM_ERROR_KEYS[code];
  if (key) {
    return translate(key);
  }

  return translate('errors.unexpected');
}

export function resolvePlatformErrorMessage(
  error: unknown,
  fallbackKey: string,
  translate: (key: string) => string,
): string {
  return translatePlatformErrorCode(extractPlatformErrorCode(error, fallbackKey), translate);
}
