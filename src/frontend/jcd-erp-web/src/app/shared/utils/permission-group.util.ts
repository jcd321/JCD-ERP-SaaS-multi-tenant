import { MODULE_LABELS, PERMISSION_LABELS } from '../constants/permission-labels';

export interface GroupablePermission {
  id: string;
  code: string;
  module: string;
  description: string | null;
}

export function getPermissionLabel(permission: GroupablePermission): string {
  return PERMISSION_LABELS[permission.code] ?? permission.description ?? permission.code;
}

export function getModuleLabel(module: string): string {
  return MODULE_LABELS[module] ?? module;
}

export function groupPermissionsByModule(
  permissions: GroupablePermission[],
): { module: string; items: GroupablePermission[] }[] {
  const grouped = new Map<string, GroupablePermission[]>();

  for (const permission of permissions) {
    const list = grouped.get(permission.module) ?? [];
    list.push(permission);
    grouped.set(permission.module, list);
  }

  return Array.from(grouped.entries()).map(([module, items]) => ({ module, items }));
}
