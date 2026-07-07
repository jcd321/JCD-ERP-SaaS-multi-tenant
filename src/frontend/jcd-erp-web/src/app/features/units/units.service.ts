import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateUnitRequest,
  PaginatedUnitsResponse,
  UnitsQueryParams,
  UpdateUnitRequest,
} from './units.models';

@Injectable({ providedIn: 'root' })
export class UnitsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/units`;

  getUnits(params: UnitsQueryParams = {}): Observable<PaginatedUnitsResponse> {
    let httpParams = new HttpParams();

    if (params.page) {
      httpParams = httpParams.set('page', params.page);
    }

    if (params.pageSize) {
      httpParams = httpParams.set('pageSize', params.pageSize);
    }

    if (params.search?.trim()) {
      httpParams = httpParams.set('search', params.search.trim());
    }

    if (params.isActive !== undefined && params.isActive !== null) {
      httpParams = httpParams.set('isActive', params.isActive);
    }

    return this.http.get<PaginatedUnitsResponse>(this.apiUrl, { params: httpParams });
  }

  createUnit(request: CreateUnitRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateUnit(unitId: string, request: UpdateUnitRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${unitId}`, request);
  }

  deleteUnit(unitId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${unitId}`);
  }
}
