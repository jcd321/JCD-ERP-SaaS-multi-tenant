import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { Role } from './roles.models';

@Injectable({ providedIn: 'root' })
export class RolesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/roles`;

  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl);
  }
}
