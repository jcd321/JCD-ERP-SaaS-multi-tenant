import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { map, take } from 'rxjs';

import { selectUserPermissions } from '../../store/auth/auth.selectors';

export function permissionGuard(requiredPermission: string): CanActivateFn {
  return () => {
    const store = inject(Store);
    const router = inject(Router);

    return store.select(selectUserPermissions).pipe(
      take(1),
      map((permissions) =>
        permissions.includes(requiredPermission)
          ? true
          : router.createUrlTree(['/']),
      ),
    );
  };
}
