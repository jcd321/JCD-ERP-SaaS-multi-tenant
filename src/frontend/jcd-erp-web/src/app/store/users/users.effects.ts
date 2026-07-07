import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, switchMap } from 'rxjs';

import { resolvePlatformErrorMessage } from '../../core/platform/platform-error-messages';
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
              error: resolvePlatformErrorMessage(error, 'Users.LoadFailed'),
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
              error: resolvePlatformErrorMessage(error, 'Users.CreateFailed'),
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
              error: resolvePlatformErrorMessage(error, 'Users.UpdateFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly reloadAfterMutation$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UsersActions.createUserSuccess, UsersActions.updateUserSuccess),
      map(() => UsersActions.loadUsers()),
    ),
  );
}
