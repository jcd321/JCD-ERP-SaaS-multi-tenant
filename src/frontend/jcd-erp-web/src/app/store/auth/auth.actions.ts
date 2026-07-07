import { createActionGroup, emptyProps, props } from '@ngrx/store';

import {
  AuthSession,
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  RegisterResponse,
} from '../../core/auth/auth.models';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    Login: props<{ request: LoginRequest }>(),
    'Login Success': props<{ response: LoginResponse }>(),
    'Login Failure': props<{ error: string }>(),

    Register: props<{ request: RegisterRequest }>(),
    'Register Success': props<{ response: RegisterResponse; session: AuthSession }>(),
    'Register Failure': props<{ error: string }>(),

    Logout: emptyProps(),
    'Logout Success': emptyProps(),
    'Logout Failure': props<{ error: string }>(),

    'Refresh Token': emptyProps(),
    'Refresh Token Success': props<{ accessToken: string; refreshToken: string }>(),
    'Refresh Token Failure': props<{ error: string }>(),

    'Clear Auth': emptyProps(),
  },
});
