import { createActionGroup, emptyProps, props } from '@ngrx/store';

import { TenantSetting } from '../../features/settings/settings.models';

export const SettingsActions = createActionGroup({
  source: 'Settings',
  events: {
    'Load Settings': emptyProps(),
    'Load Settings Success': props<{ settings: TenantSetting[] }>(),
    'Load Settings Failure': props<{ error: string }>(),
  },
});
