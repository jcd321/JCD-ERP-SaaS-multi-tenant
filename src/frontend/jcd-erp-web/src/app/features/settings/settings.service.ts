import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import { TenantSetting } from './settings.models';

@Injectable({ providedIn: 'root' })
export class SettingsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/settings`;

  getSettings(): Observable<TenantSetting[]> {
    return this.http.get<TenantSetting[]>(this.apiUrl);
  }
}
