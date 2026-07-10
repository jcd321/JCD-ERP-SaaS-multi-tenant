import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  AdjustmentLookupOption,
  AdjustmentLookupsResponse,
  AdjustmentStockLevel,
  AdjustmentsQueryParams,
  CreateInventoryAdjustmentRequest,
  PaginatedAdjustmentsResponse,
} from './adjustments.models';

@Injectable({ providedIn: 'root' })
export class AdjustmentsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/adjustments`;

  getAdjustments(params: AdjustmentsQueryParams = {}): Observable<PaginatedAdjustmentsResponse> {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.warehouseId) httpParams = httpParams.set('warehouseId', params.warehouseId);
    if (params.fromDate) httpParams = httpParams.set('fromDate', params.fromDate);
    if (params.toDate) httpParams = httpParams.set('toDate', params.toDate);

    return this.http.get<PaginatedAdjustmentsResponse>(this.apiUrl, { params: httpParams });
  }

  getLookups(): Observable<AdjustmentLookupsResponse> {
    return this.http.get<unknown>(`${this.apiUrl}/lookups`).pipe(
      map((raw) => this.normalizeLookups(raw)),
    );
  }

  createAdjustment(request: CreateInventoryAdjustmentRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  private normalizeLookups(raw: unknown): AdjustmentLookupsResponse {
    const data = (raw ?? {}) as Record<string, unknown>;

    const products = this.normalizeOptions(data['products'] ?? data['Products']);
    const warehouses = this.normalizeOptions(data['warehouses'] ?? data['Warehouses']);
    const stockLevels = this.normalizeStockLevels(data['stockLevels'] ?? data['StockLevels']);

    return { products, warehouses, stockLevels };
  }

  private normalizeOptions(raw: unknown): AdjustmentLookupOption[] {
    if (!Array.isArray(raw)) return [];

    return raw.map((item) => {
      const option = item as Record<string, unknown>;
      return {
        id: this.normalizeId(option['id'] ?? option['Id']),
        label: String(option['label'] ?? option['Label'] ?? ''),
      };
    });
  }

  private normalizeStockLevels(raw: unknown): AdjustmentStockLevel[] {
    if (!Array.isArray(raw)) return [];

    return raw.map((item) => {
      const level = item as Record<string, unknown>;
      return {
        productId: this.normalizeId(level['productId'] ?? level['ProductId']),
        warehouseId: this.normalizeId(level['warehouseId'] ?? level['WarehouseId']),
        quantityOnHand: Number(level['quantityOnHand'] ?? level['QuantityOnHand'] ?? 0),
      };
    });
  }

  private normalizeId(value: unknown): string {
    return String(value ?? '').trim().toLowerCase();
  }
}
