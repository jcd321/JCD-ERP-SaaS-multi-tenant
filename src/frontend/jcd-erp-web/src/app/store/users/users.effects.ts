import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';

import { extractPlatformErrorCode } from '../../core/platform/platform-error-messages';
import { UsersService } from '../../features/users/users.service';
import { UsersActions } from './users.actions';

@Injectable()
export class UsersEffects {
  private readonly actions$ = inject(Actions);
  private readonly usersService = inject(UsersService);

  readonly loadUsers$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersActions.loadUsers),
      exhaustMap(() =>
        this.usersService.getUsers().pipe(
          map((users) => UsersActions.loadUsersSuccess({ users })),
          catchError((error) =>
            of(UsersActions.loadUsersFailure({
              error: extractPlatformErrorCode(error, 'Users.LoadFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly createUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersActions.createUser),
      exhaustMap(({ request }) =>
        this.usersService.createUser(request).pipe(
          map(() => UsersActions.createUserSuccess()),
          catchError((error) =>
            of(UsersActions.createUserFailure({
              error: extractPlatformErrorCode(error, 'Users.CreateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly updateUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersActions.updateUser),
      exhaustMap(({ userId, request }) =>
        this.usersService.updateUser(userId, request).pipe(
          map(() => UsersActions.updateUserSuccess()),
          catchError((error) =>
            of(UsersActions.updateUserFailure({
              error: extractPlatformErrorCode(error, 'Users.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersActions.createUserSuccess, UsersActions.updateUserSuccess, UsersActions.deleteUserSuccess),
      map(() => UsersActions.loadUsers()),
    ),
  );

  readonly deleteUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersActions.deleteUser),
      exhaustMap(({ userId }) =>
        this.usersService.deleteUser(userId).pipe(
          map(() => UsersActions.deleteUserSuccess()),
          catchError((error) =>
            of(UsersActions.deleteUserFailure({
              error: extractPlatformErrorCode(error, 'Users.DeleteFailed'),
            })),
          ),
        ),
      ),
    ),
  );
}
