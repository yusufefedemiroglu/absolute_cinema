import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { TokenStore } from '../auth/token.store';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenStore = inject(TokenStore);
  const authService = inject(AuthService);

  const token = tokenStore.get();

  // 1) if token avaliable, add Authorization header
  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((err: unknown) => {
      // 2) dont handle if not 401
      if (!(err instanceof HttpErrorResponse) || err.status !== 401) {
        return throwError(() => err);
      }

      // if refresh endpoint it does not allow to cycle forever
      if (req.url.includes('/auth/refresh')) {
        tokenStore.clear();
        return throwError(() => err);
      }

      const currentToken = tokenStore.get();
      if (!currentToken) {
        return throwError(() => err);
      }

      // 3) if 401 try to refresh token
      return authService.refresh(currentToken).pipe(
        switchMap((res) => {
          // access token name which comes from backend
          const newToken = (res as any).accessToken;
          if (!newToken) return throwError(() => err);

          tokenStore.set(newToken);

          // 4) same request with new token
          const retryReq = req.clone({
            setHeaders: { Authorization: `Bearer ${newToken}` },
          });

          return next(retryReq);
        }),
        catchError((refreshErr) => {
          // if refresh fails, clear stored tokens
          tokenStore.clear();
          return throwError(() => refreshErr);
        })
      );
    })
  );
};