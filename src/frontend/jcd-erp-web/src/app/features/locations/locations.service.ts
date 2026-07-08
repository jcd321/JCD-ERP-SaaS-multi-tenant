import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateLocationRequest,
  LocationParentOption,
  LocationsQueryParams,
  PaginatedLocationsResponse,
  UpdateLocationRequest,
} from './locations.models';

@Injectable({ providedIn: 'root' })
export class LocationsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/locations`;

  getLocations(params: LocationsQueryParams): Observable<PaginatedLocationsResponse> {
    let httpParams = new HttpParams().set('warehouseId', params.warehouseId);

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.isActive !== undefined && params.isActive !== null) {
      httpParams = httpParams.set('isActive', params.isActive);
    }

    return this.http.get<PaginatedLocationsResponse>(this.apiUrl, { params: httpParams });
  }

  getParentOptions(warehouseId: string, excludeId?: string): Observable<LocationParentOption[]> {
    let httpParams = new HttpParams().set('warehouseId', warehouseId);
    if (excludeId) httpParams = httpParams.set('excludeId', excludeId);
    return this.http.get<LocationParentOption[]>(`${this.apiUrl}/parent-options`, { params: httpParams });
  }

  createLocation(request: CreateLocationRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateLocation(locationId: string, request: UpdateLocationRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${locationId}`, request);
  }

  deleteLocation(locationId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${locationId}`);
  }
}
