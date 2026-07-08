import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateInventoryMovementRequest,
  MovementLookupsResponse,
  MovementsQueryParams,
  PaginatedMovementsResponse,
} from './movements.models';

@Injectable({ providedIn: 'root' })
export class MovementsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/movements`;

  getMovements(params: MovementsQueryParams = {}): Observable<PaginatedMovementsResponse> {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.warehouseId) httpParams = httpParams.set('warehouseId', params.warehouseId);
    if (params.productId) httpParams = httpParams.set('productId', params.productId);
    if (params.movementType) httpParams = httpParams.set('movementType', params.movementType);

    return this.http.get<PaginatedMovementsResponse>(this.apiUrl, { params: httpParams });
  }

  getLookups(): Observable<MovementLookupsResponse> {
    return this.http.get<MovementLookupsResponse>(`${this.apiUrl}/lookups`);
  }

  createMovement(request: CreateInventoryMovementRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }
}
