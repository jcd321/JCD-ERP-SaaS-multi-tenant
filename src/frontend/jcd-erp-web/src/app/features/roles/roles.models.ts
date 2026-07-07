export interface Role {
  id: string;
  name: string;
  description: string | null;
  isSystem: boolean;
  permissions: string[];
}

export interface Permission {
  id: string;
  code: string;
  module: string;
  action: string;
  description: string | null;
}

export interface CreateRoleRequest {
  name: string;
  description: string | null;
  permissionCodes: string[];
}

export interface UpdateRoleRequest {
  name: string;
  description: string | null;
  permissionCodes: string[];
}

export type RoleFormMode = 'create' | 'edit' | null;
