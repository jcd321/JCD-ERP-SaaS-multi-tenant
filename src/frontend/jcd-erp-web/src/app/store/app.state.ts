import { AuthState } from './auth/auth.state';
import { RolesState } from './roles/roles.state';
import { SettingsState } from './settings/settings.state';
import { UsersState } from './users/users.state';

export interface AppState {
  auth: AuthState;
  users: UsersState;
  roles: RolesState;
  settings: SettingsState;
}
