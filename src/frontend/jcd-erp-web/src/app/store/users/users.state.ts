import { User } from '../../features/users/users.models';

export interface UsersState {
  items: User[];
  loading: boolean;
  saving: boolean;
  error: string | null;
}

export const initialUsersState: UsersState = {
  items: [],
  loading: false,
  saving: false,
  error: null,
};
