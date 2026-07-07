import { createActionGroup, emptyProps, props } from '@ngrx/store';

import { CreateUserRequest, UpdateUserRequest, User } from '../../features/users/users.models';

export const UsersActions = createActionGroup({
  source: 'Users',
  events: {
    'Load Users': emptyProps(),
    'Load Users Success': props<{ users: User[] }>(),
    'Load Users Failure': props<{ error: string }>(),

    'Create User': props<{ request: CreateUserRequest }>(),
    'Create User Success': emptyProps(),
    'Create User Failure': props<{ error: string }>(),

    'Update User': props<{ userId: string; request: UpdateUserRequest }>(),
    'Update User Success': emptyProps(),
    'Update User Failure': props<{ error: string }>(),
  },
});
