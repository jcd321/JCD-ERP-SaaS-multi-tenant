import { Role } from '../../features/roles/roles.models';

export interface RolesState {
  items: Role[];
  loading: boolean;
  error: string | null;
}

export const initialRolesState: RolesState = {
  items: [],
  loading: false,
  error: null,
};
