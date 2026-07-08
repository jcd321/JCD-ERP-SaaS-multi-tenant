import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  WarehousesQueryParams,
  CreateWarehouseRequest,
  PaginatedWarehousesResponse,
  UpdateWarehouseRequest,
} from './warehouses.models';

@Injectable({ providedIn: 'root' })
export class WarehousesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/warehouses`;

  getWarehouses(params: WarehousesQueryParams = {}): Observable<PaginatedWarehousesResponse> {
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

    return this.http.get<PaginatedWarehousesResponse>(this.apiUrl, { params: httpParams });
  }

  createWarehouse(request: CreateWarehouseRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateWarehouse(warehouseId: string, request: UpdateWarehouseRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${warehouseId}`, request);
  }

  deleteWarehouse(warehouseId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${warehouseId}`);
  }
}
