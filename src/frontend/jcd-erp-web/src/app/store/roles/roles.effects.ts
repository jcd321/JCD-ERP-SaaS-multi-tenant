import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap } from 'rxjs';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
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
            of(RolesActions.loadRolesFailure({
              error: extractPlatformErrorCode(error, 'Roles.LoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly loadPermissions$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RolesActions.loadPermissions),
      exhaustMap(() =>
        this.rolesService.getPermissions().pipe(
          map((permissions) => RolesActions.loadPermissionsSuccess({ permissions })),
          catchError((error) =>
            of(RolesActions.loadPermissionsFailure({
              error: extractPlatformErrorCode(error, 'Roles.PermissionsLoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RolesActions.createRole),
      exhaustMap(({ request }) =>
        this.rolesService.createRole(request).pipe(
          map(() => RolesActions.createRoleSuccess()),
          catchError((error) =>
            of(RolesActions.createRoleFailure({
              error: extractPlatformErrorCode(error, 'Roles.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RolesActions.updateRole),
      exhaustMap(({ roleId, request }) =>
        this.rolesService.updateRole(roleId, request).pipe(
          map(() => RolesActions.updateRoleSuccess()),
          catchError((error) =>
            of(RolesActions.updateRoleFailure({
              error: extractPlatformErrorCode(error, 'Roles.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly deleteRole$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RolesActions.deleteRole),
      exhaustMap(({ roleId }) =>
        this.rolesService.deleteRole(roleId).pipe(
          map(() => RolesActions.deleteRoleSuccess()),
          catchError((error) =>
            of(RolesActions.deleteRoleFailure({
              error: extractPlatformErrorCode(error, 'Roles.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(RolesActions.createRoleSuccess, RolesActions.updateRoleSuccess, RolesActions.deleteRoleSuccess),
      switchMap(() => [RolesActions.loadRoles(), RolesActions.loadPermissions()]),
    ),
  );
}
