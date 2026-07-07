export function getPermissionLabel(
  permission: { code: string; description: string | null },
  translate: (key: string) => string,
): string {
  const key = `permissions.codes.${permission.code}`;
  const translated = translate(key);
  return translated !== key ? translated : (permission.description ?? permission.code);
}

export function getModuleLabel(module: string, translate: (key: string) => string): string {
  const key = `permissions.modules.${module}`;
  const translated = translate(key);
  return translated !== key ? translated : module;
}

export function groupPermissionsByModule<T extends { module: string }>(
  permissions: T[],
): { module: string; items: T[] }[] {
  const grouped = new Map<string, T[]>();

  for (const permission of permissions) {
    const list = grouped.get(permission.module) ?? [];
    list.push(permission);
    grouped.set(permission.module, list);
  }

  return Array.from(grouped.entries()).map(([module, items]) => ({ module, items }));
}
