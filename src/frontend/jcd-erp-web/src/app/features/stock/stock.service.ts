import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateStockLevelRequest,
  PaginatedStockResponse,
  StockLookupsResponse,
  StockQueryParams,
  UpdateStockLevelRequest,
} from './stock.models';

@Injectable({ providedIn: 'root' })
export class StockService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/stock`;

  getStockLevels(params: StockQueryParams = {}): Observable<PaginatedStockResponse> {
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

    if (params.warehouseId) {
      httpParams = httpParams.set('warehouseId', params.warehouseId);
    }

    if (params.productId) {
      httpParams = httpParams.set('productId', params.productId);
    }

    if (params.belowMinimumOnly) {
      httpParams = httpParams.set('belowMinimumOnly', true);
    }

    return this.http.get<PaginatedStockResponse>(this.apiUrl, { params: httpParams });
  }

  getLookups(): Observable<StockLookupsResponse> {
    return this.http.get<StockLookupsResponse>(`${this.apiUrl}/lookups`);
  }

  createStockLevel(request: CreateStockLevelRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateStockLevel(stockLevelId: string, request: UpdateStockLevelRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${stockLevelId}`, request);
  }

  deleteStockLevel(stockLevelId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${stockLevelId}`);
  }
}
