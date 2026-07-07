import { createActionGroup, emptyProps, props } from '@ngrx/store';

import { Role } from '../../features/roles/roles.models';

export const RolesActions = createActionGroup({
  source: 'Roles',
  events: {
    'Load Roles': emptyProps(),
    'Load Roles Success': props<{ roles: Role[] }>(),
    'Load Roles Failure': props<{ error: string }>(),
  },
});
