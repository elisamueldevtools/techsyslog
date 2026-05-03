import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { TokenRefreshCoordinatorService } from '../services/token-refresh-coordinator.service';

const AUTH_ROUTES = ['/auth/login', '/auth/register', '/auth/refresh', '/auth/logout'];

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);
  const coordinator = inject(TokenRefreshCoordinatorService);

  const isAuthRoute = AUTH_ROUTES.some(r => req.url.endsWith(r));

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status !== 401) return throwError(() => err);

      if (isAuthRoute) {
        auth.logout();
        router.navigateByUrl('/login');
        return throwError(() => err);
      }

      if (!auth.getRefreshToken()) {
        auth.logout();
        router.navigateByUrl('/login');
        return throwError(() => err);
      }

      return coordinator.coordinatedRefresh().pipe(
        switchMap(newAccess => {
          const retried = req.clone({ setHeaders: { Authorization: `Bearer ${newAccess}` } });
          return next(retried);
        }),
        catchError(refreshErr => {
          coordinator.abort();
          auth.logout();
          router.navigateByUrl('/login');
          return throwError(() => refreshErr);
        })
      );
    })
  );
};
