import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of } from 'rxjs';

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
            of(UsersActions.loadUsersFailure({ error: error.error?.error ?? 'Users.LoadFailed' })),
          ),
        ),
      ),
    ),
  );
}
