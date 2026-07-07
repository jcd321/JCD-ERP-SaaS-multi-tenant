import { createFeatureSelector, createSelector } from '@ngrx/store';

import { SettingsState } from './settings.state';

export const selectSettingsState = createFeatureSelector<SettingsState>('settings');

export const selectAllSettings = createSelector(selectSettingsState, (state) => state.items);

export const selectSettingsLoading = createSelector(selectSettingsState, (state) => state.loading);

export const selectSettingsError = createSelector(selectSettingsState, (state) => state.error);
