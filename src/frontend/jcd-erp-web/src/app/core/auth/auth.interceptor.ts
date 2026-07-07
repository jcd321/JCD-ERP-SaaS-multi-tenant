import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, finalize, shareReplay, switchMap, throwError } from 'rxjs';

import { AuthFacade } from '../../store/auth/auth.facade';

let refreshInProgress: ReturnType<AuthFacade['refreshToken']> | null = null;

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authFacade = inject(AuthFacade);
  const router = inject(Router);

  const token = authFacade.accessToken();
  const isAuthEndpoint = req.url.includes('/api/v1/auth/');
  const authorizedReq =
    token && !isAuthEndpoint
      ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
      : req;

  return next(authorizedReq).pipe(
    catchError((error: HttpErrorResponse) => {
      const shouldRefresh =
        error.status === 401 &&
        !isAuthEndpoint &&
        !req.url.includes('/api/v1/auth/refresh');

      if (!shouldRefresh) {
        return throwError(() => error);
      }

      if (!refreshInProgress) {
        refreshInProgress = authFacade.refreshToken().pipe(
          shareReplay(1),
          finalize(() => {
            refreshInProgress = null;
          }),
        );
      }

      return refreshInProgress.pipe(
        switchMap((newToken) => {
          const retryReq = req.clone({
            setHeaders: { Authorization: `Bearer ${newToken}` },
          });
          return next(retryReq);
        }),
        catchError((refreshError) => {
          authFacade.clearAuth();
          void router.navigate(['/auth/login']);
          return throwError(() => refreshError);
        }),
      );
    }),
  );
};
