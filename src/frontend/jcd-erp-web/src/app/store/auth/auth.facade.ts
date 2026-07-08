import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { Actions, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { map, Observable, of, switchMap, take, throwError } from 'rxjs';

import { translateAuthErrorCode } from '../../core/auth/auth-error-messages';
import { LoginRequest, RegisterRequest, ForgotPasswordRequest, ResetPasswordRequest } from '../../core/auth/auth.models';
import { createLocalizedError } from '../../core/i18n';
import { AuthActions } from './auth.actions';
import {
  selectAccessToken,
  selectAuthError,
  selectAuthLoading,
  selectAuthSession,
  selectIsAuthenticated,
} from './auth.selectors';

@Injectable({ providedIn: 'root' })
export class AuthFacade {
  private readonly store = inject(Store);
  private readonly actions$ = inject(Actions);

  readonly session = toSignal(this.store.select(selectAuthSession), { initialValue: null });
  readonly accessToken = toSignal(this.store.select(selectAccessToken), { initialValue: null });
  readonly isAuthenticated = toSignal(this.store.select(selectIsAuthenticated), { initialValue: false });
  readonly loading = toSignal(this.store.select(selectAuthLoading), { initialValue: false });
  private readonly errorCode = toSignal(this.store.select(selectAuthError), { initialValue: null });
  readonly error = createLocalizedError(this.errorCode, translateAuthErrorCode);

  login(request: LoginRequest): void {
    this.store.dispatch(AuthActions.login({ request }));
  }

  register(request: RegisterRequest): void {
    this.store.dispatch(AuthActions.register({ request }));
  }

  logout(): void {
    this.store.dispatch(AuthActions.logout());
  }

  forgotPassword(request: ForgotPasswordRequest): void {
    this.store.dispatch(AuthActions.forgotPassword({ request }));
  }

  resetPassword(request: ResetPasswordRequest): void {
    this.store.dispatch(AuthActions.resetPassword({ request }));
  }

  clearAuth(): void {
    this.store.dispatch(AuthActions.clearAuth());
  }

  refreshToken(): Observable<string> {
    this.store.dispatch(AuthActions.refreshToken());

    return this.actions$.pipe(
      ofType(AuthActions.refreshTokenSuccess, AuthActions.refreshTokenFailure),
      take(1),
      switchMap((action) => {
        if (action.type === AuthActions.refreshTokenFailure.type) {
          this.clearAuth();
          return throwError(() => new Error(action.error));
        }
        return of(action.accessToken);
      }),
    );
  }
}
