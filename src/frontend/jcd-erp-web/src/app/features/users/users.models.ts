export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  isActive: boolean;
  roles: string[];
}

export interface CreateUserRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  roleIds: string[];
}

export interface UpdateUserRequest {
  firstName: string;
  lastName: string;
  isActive: boolean;
  roleIds: string[];
}

export type UserFormMode = 'create' | 'edit' | null;
