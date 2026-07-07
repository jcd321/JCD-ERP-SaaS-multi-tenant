import { EnvironmentProviders, isDevMode } from '@angular/core';
import { provideEffects } from '@ngrx/effects';
import { provideState, provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { authReducer } from './auth/auth.reducer';
import { AuthEffects } from './auth/auth.effects';
import { rolesReducer } from './roles/roles.reducer';
import { RolesEffects } from './roles/roles.effects';
import { settingsReducer } from './settings/settings.reducer';
import { SettingsEffects } from './settings/settings.effects';
import { usersReducer } from './users/users.reducer';
import { UsersEffects } from './users/users.effects';

export function provideAppStore(): EnvironmentProviders[] {
  return [
    provideStore(),
    provideState('auth', authReducer),
    provideState('users', usersReducer),
    provideState('roles', rolesReducer),
    provideState('settings', settingsReducer),
    provideEffects(AuthEffects, UsersEffects, RolesEffects, SettingsEffects),
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode(),
      autoPause: true,
    }),
  ];
}
