import { HttpHandlerFn, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, catchError, filter, finalize, Observable, switchMap, take, throwError } from 'rxjs';
import { AuthService } from '../auth/auth.service';
import { AuthStore } from '../auth/auth.store';

// This interceptor handles the refresh logic

let isRefreshing = false;
let refreshTokenSubject = new BehaviorSubject<boolean>(false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const authService = inject(AuthService);
  const authStore = inject(AuthStore);

  // to avoid infinite loop
  if (req.url.includes('refresh') || req.url.includes('/login')) {
    return next(req);
  }

  return next(req).pipe(
    catchError((err) => {

      if (err.status === 403) {
        router.navigate(['/login']);
      }
      if (err.status === 401) {
        return handle401Error(req, next, router, authService, authStore);
      }
      return throwError(() => err);
    })
  );
};


function handle401Error(req: HttpRequest<unknown>, next: HttpHandlerFn, router: Router, authService: AuthService, authStore: AuthStore): Observable<any> {
  if (isRefreshing) {
    return refreshTokenSubject.pipe(
      filter(refreshCompleted => refreshCompleted === true),
      take(1),
      switchMap(() => next(req))
    );
  }

  // Start refresh process
  isRefreshing = true;
  refreshTokenSubject.next(false);

  // Attempt to refresh the token
  return authService.refresh().pipe(
    switchMap(() => {
      // Refresh successful
      isRefreshing = false;
      refreshTokenSubject.next(true);

      authStore.loadStore();

      // Retry the original request
      return next(req);
    }),
    catchError((refreshError) => {
      // Refresh failed - logout user
      isRefreshing = false;
      refreshTokenSubject.next(false);

      // Clear user data and redirect to login
      authStore.logout();
      router.navigate(['/login']);

      return throwError(() => refreshError);
    }),
    finalize(() => {
      isRefreshing = false;
    })
  );
}