import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthStore } from '../auth/auth.store';
import { toObservable } from '@angular/core/rxjs-interop';
import { filter, map, take } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
  const authStore = inject(AuthStore);
  const router = inject(Router);

  // If still loading, wait for it to complete
  if (authStore.loading()) {
    return toObservable(authStore.loaded).pipe(
      filter(loaded => loaded), // Wait until loading is complete
      take(1),
      map(() => {
        console.log('Auth guard - after loading - user:', JSON.stringify(authStore.user));
        if (!authStore.authenticated()) {
          console.log("ASYNC: not logged in, visited route: " + state.url);
          console.log("ASYNC: navigating to login with returnUrl:", state.url);
          router.navigate(['/login'], {
            queryParams: { returnUrl: state.url }
          });
          return false;
        }
        return true;
      })
    );
  }

  // If not loading, check immediately
  if (!authStore.authenticated()) {
    router.navigate(['/login'], {
      queryParams: { returnUrl: state.url }
    });
    return false;
  }

  // console.log('Auth guard - user is authenticated, allowing access');
  return true;
};
