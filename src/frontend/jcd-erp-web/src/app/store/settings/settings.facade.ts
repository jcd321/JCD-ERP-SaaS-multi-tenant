import { inject, Injectable } from '@angular/core';
import { createLocalizedError } from '../../core/i18n';
import { translatePlatformErrorCode } from '../../core/platform/platform-error-messages';
import { toSignal } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';

import { SettingsActions } from './settings.actions';
import { selectAllSettings, selectSettingsError, selectSettingsLoading } from './settings.selectors';

@Injectable({ providedIn: 'root' })
export class SettingsFacade {
  private readonly store = inject(Store);

  readonly settings = toSignal(this.store.select(selectAllSettings), { initialValue: [] });
  readonly loading = toSignal(this.store.select(selectSettingsLoading), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectSettingsError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translatePlatformErrorCode);

  loadSettings(): void {
    this.store.dispatch(SettingsActions.loadSettings());
  }
}
