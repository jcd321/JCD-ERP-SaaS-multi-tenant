import { TenantSetting } from '../../features/settings/settings.models';

export interface SettingsState {
  items: TenantSetting[];
  loading: boolean;
  error: string | null;
}

export const initialSettingsState: SettingsState = {
  items: [],
  loading: false,
  error: null,
};
