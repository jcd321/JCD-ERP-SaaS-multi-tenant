import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';

import { RolesService } from '../../features/roles/roles.service';
import { RolesActions } from './roles.actions';

@Injectable()
export class RolesEffects {
  private readonly actions$ = inject(Actions);
  private readonly rolesService = inject(RolesService);

  readonly loadRoles$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RolesActions.loadRoles),
      exhaustMap(() =>
        this.rolesService.getRoles().pipe(
          map((roles) => RolesActions.loadRolesSuccess({ roles })),
          catchError((error) =>
            of(RolesActions.loadRolesFailure({ error: error.error?.error ?? 'Roles.LoadFailed' })),
          ),
        ),
      ),
    ),
  );
}
