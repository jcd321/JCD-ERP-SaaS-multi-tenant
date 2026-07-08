import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { catchError, exhaustMap, map, of, tap } from 'rxjs';

import { AuthApiService } from '../../core/auth/auth-api.service';
import { extractAuthErrorCode } from '../../core/auth/auth-error-messages';
import { AuthActions } from './auth.actions';

@Injectable()
export class AuthEffects {
  private readonly actions$ = inject(Actions);
  private readonly authApi = inject(AuthApiService);
  private readonly router = inject(Router);

  readonly login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ request }) =>
        this.authApi.login(request).pipe(
          map((response) => AuthActions.loginSuccess({ response })),
          catchError((error) =>
            of(AuthActions.loginFailure({
              error: extractAuthErrorCode(error, 'Auth.LoginFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly loginSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.loginSuccess, AuthActions.registerSuccess),
        tap(() => void this.router.navigate(['/'])),
      ),
    { dispatch: false },
  );

  readonly register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      exhaustMap(({ request }) =>
        this.authApi.register(request).pipe(
          map((response) =>
            AuthActions.registerSuccess({
              response,
              session: {
                tenantId: response.tenantId,
                tenantSlug: response.tenantSlug,
                userId: response.userId,
                email: request.adminEmail,
                fullName: `${request.adminFirstName} ${request.adminLastName}`.trim(),
                permissions: [],
              },
            }),
          ),
          catchError((error) =>
            of(
              AuthActions.registerFailure({
                error: extractAuthErrorCode(error, 'Auth.RegisterFailed'),
              }),
            ),
          ),
        ),
      ),
    ),
  );

  readonly logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      exhaustMap(() => {
        const refreshToken = localStorage.getItem('jcd_erp_refresh_token');
        return this.authApi.logout(refreshToken).pipe(
          map(() => AuthActions.logoutSuccess()),
          catchError(() => of(AuthActions.logoutSuccess())),
        );
      }),
    ),
  );

  readonly logoutSuccess$ = createEffect(
    () =>
      this.actions$.pipe(
        ofType(AuthActions.logoutSuccess),
        tap(() => void this.router.navigate(['/auth/login'])),
      ),
    { dispatch: false },
  );

  readonly refreshToken$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.refreshToken),
      exhaustMap(() => {
        const refreshToken = localStorage.getItem('jcd_erp_refresh_token');
        if (!refreshToken) {
          return of(AuthActions.refreshTokenFailure({ error: 'Auth.NoRefreshToken' }));
        }

        return this.authApi.refreshToken(refreshToken).pipe(
          map((response) =>
            AuthActions.refreshTokenSuccess({
              accessToken: response.accessToken,
              refreshToken: response.refreshToken,
            }),
          ),
          catchError((error) =>
            of(
              AuthActions.refreshTokenFailure({
                error: extractAuthErrorCode(error, 'Auth.RefreshFailed'),
              }),
            ),
          ),
        );
      }),
    ),
  );

  readonly forgotPassword$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.forgotPassword),
      exhaustMap(({ request }) =>
        this.authApi.forgotPassword(request).pipe(
          map(() => AuthActions.forgotPasswordSuccess()),
          catchError((error) =>
            of(AuthActions.forgotPasswordFailure({
              error: extractAuthErrorCode(error, 'Auth.ForgotPasswordFailed'),
            })),
          ),
        ),
      ),
    ),
  );

  readonly resetPassword$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.resetPassword),
      exhaustMap(({ request }) =>
        this.authApi.resetPassword(request).pipe(
          map(() => AuthActions.resetPasswordSuccess()),
          catchError((error) =>
            of(AuthActions.resetPasswordFailure({
              error: extractAuthErrorCode(error, 'Auth.ResetPasswordFailed'),
            })),
          ),
        ),
      ),
    ),
  );
}
