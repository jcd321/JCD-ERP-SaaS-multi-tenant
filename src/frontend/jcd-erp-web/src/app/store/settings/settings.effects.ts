import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';

import { SettingsService } from '../../features/settings/settings.service';
import { SettingsActions } from './settings.actions';

@Injectable()
export class SettingsEffects {
  private readonly actions$ = inject(Actions);
  private readonly settingsService = inject(SettingsService);

  readonly loadSettings$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SettingsActions.loadSettings),
      exhaustMap(() =>
        this.settingsService.getSettings().pipe(
          map((settings) => SettingsActions.loadSettingsSuccess({ settings })),
          catchError((error) =>
            of(
              SettingsActions.loadSettingsFailure({
                error: error.error?.error ?? 'Settings.LoadFailed',
              }),
            ),
          ),
        ),
      ),
    ),
  );
}
