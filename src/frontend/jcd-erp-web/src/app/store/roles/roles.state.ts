import { Role, Permission } from '../../features/roles/roles.models';

export interface RolesState {
  items: Role[];
  permissions: Permission[];
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialRolesState: RolesState = {
  items: [],
  permissions: [],
  loading: false,
  saving: false,
  error: null,
};
