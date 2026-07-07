import { createReducer, on } from '@ngrx/store';

import { SettingsActions } from './settings.actions';
import { initialSettingsState } from './settings.state';

export const settingsReducer = createReducer(
  initialSettingsState,

  on(SettingsActions.loadSettings, (state) => ({
    ...state,
    loading: true,
    error: null,
  })),

  on(SettingsActions.loadSettingsSuccess, (state, { settings }) => ({
    ...state,
    items: settings,
    loading: false,
    error: null,
  })),

  on(SettingsActions.loadSettingsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error,
  })),
);
