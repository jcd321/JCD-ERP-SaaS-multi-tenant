import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  CreateRoleRequest,
  Permission,
  Role,
  UpdateRoleRequest,
} from '../../features/roles/roles.models';

export const RolesActions = createActionGroup({
  source: 'Roles',
  events: {
    'Load Roles': emptyProps(),
    'Load Roles Success': props<{ roles: Role[] }>(),
    'Load Roles Failure': props<{ error: string }>(),

    'Load Permissions': emptyProps(),
    'Load Permissions Success': props<{ permissions: Permission[] }>(),
    'Load Permissions Failure': props<{ error: string }>(),

    'Create Role': props<{ request: CreateRoleRequest }>(),
    'Create Role Success': emptyProps(),
    'Create Role Failure': props<{ error: string }>(),

    'Update Role': props<{ roleId: string; request: UpdateRoleRequest }>(),
    'Update Role Success': emptyProps(),
    'Update Role Failure': props<{ error: string }>(),

    'Delete Role': props<{ roleId: string }>(),
    'Delete Role Success': emptyProps(),
    'Delete Role Failure': props<{ error: string }>(),
  },
});
