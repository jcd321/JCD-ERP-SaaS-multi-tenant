import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { map, Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreatePhysicalCountRequest,
  PaginatedPhysicalCountsResponse,
  PhysicalCountLineUpdate,
  PhysicalCountLookupOption,
  PhysicalCountLookupsResponse,
  PhysicalCountsQueryParams,
  PhysicalInventoryCount,
} from './physical-counts.models';

@Injectable({ providedIn: 'root' })
export class PhysicalCountsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/physical-counts`;

  getPhysicalCounts(params: PhysicalCountsQueryParams = {}): Observable<PaginatedPhysicalCountsResponse> {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.warehouseId) httpParams = httpParams.set('warehouseId', params.warehouseId);
    if (params.status) httpParams = httpParams.set('status', params.status);
    if (params.fromDate) httpParams = httpParams.set('fromDate', params.fromDate);
    if (params.toDate) httpParams = httpParams.set('toDate', params.toDate);

    return this.http.get<unknown>(this.apiUrl, { params: httpParams }).pipe(
      map((raw) => this.normalizePaged(raw)),
    );
  }

  getById(id: string): Observable<PhysicalInventoryCount> {
    return this.http.get<unknown>(`${this.apiUrl}/${id}`).pipe(
      map((raw) => this.normalizeCount(raw)),
    );
  }

  getLookups(): Observable<PhysicalCountLookupsResponse> {
    return this.http.get<unknown>(`${this.apiUrl}/lookups`).pipe(
      map((raw) => this.normalizeLookups(raw)),
    );
  }

  createPhysicalCount(request: CreatePhysicalCountRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateLines(countId: string, lines: PhysicalCountLineUpdate[]): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${countId}/lines`, lines);
  }

  complete(countId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${countId}/complete`, {});
  }

  cancel(countId: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${countId}/cancel`, {});
  }

  private normalizePaged(raw: unknown): PaginatedPhysicalCountsResponse {
    const data = (raw ?? {}) as Record<string, unknown>;
    const itemsRaw = data['items'] ?? data['Items'];
    const items = Array.isArray(itemsRaw)
      ? itemsRaw.map((item: unknown) => this.normalizeCount(item))
      : [];

    return {
      items,
      page: Number(data['page'] ?? data['Page'] ?? 1),
      pageSize: Number(data['pageSize'] ?? data['PageSize'] ?? 20),
      totalCount: Number(data['totalCount'] ?? data['TotalCount'] ?? 0),
      totalPages: Number(data['totalPages'] ?? data['TotalPages'] ?? 0),
    };
  }

  private normalizeCount(raw: unknown): PhysicalInventoryCount {
    const data = (raw ?? {}) as Record<string, unknown>;
    const linesRaw = data['lines'] ?? data['Lines'];
    const lines = Array.isArray(linesRaw)
      ? linesRaw.map((line: unknown) => this.normalizeLine(line))
      : [];

    return {
      id: this.normalizeId(data['id'] ?? data['Id']),
      documentNumber: String(data['documentNumber'] ?? data['DocumentNumber'] ?? ''),
      warehouseId: this.normalizeId(data['warehouseId'] ?? data['WarehouseId']),
      warehouseCode: String(data['warehouseCode'] ?? data['WarehouseCode'] ?? ''),
      warehouseName: String(data['warehouseName'] ?? data['WarehouseName'] ?? ''),
      countDate: String(data['countDate'] ?? data['CountDate'] ?? ''),
      status: String(data['status'] ?? data['Status'] ?? 'DRAFT').toUpperCase() as PhysicalInventoryCount['status'],
      notes: (data['notes'] ?? data['Notes'] ?? null) as string | null,
      lineCount: Number(data['lineCount'] ?? data['LineCount'] ?? lines.length),
      countedLineCount: Number(data['countedLineCount'] ?? data['CountedLineCount'] ?? 0),
      varianceLineCount: Number(data['varianceLineCount'] ?? data['VarianceLineCount'] ?? 0),
      lines,
      createdAt: String(data['createdAt'] ?? data['CreatedAt'] ?? ''),
      completedAt: (data['completedAt'] ?? data['CompletedAt'] ?? null) as string | null,
    };
  }

  private normalizeLine(raw: unknown): PhysicalInventoryCount['lines'][number] {
    const data = (raw ?? {}) as Record<string, unknown>;
    const counted = data['countedQuantity'] ?? data['CountedQuantity'];

    return {
      id: this.normalizeId(data['id'] ?? data['Id']),
      productId: this.normalizeId(data['productId'] ?? data['ProductId']),
      productSku: String(data['productSku'] ?? data['ProductSku'] ?? ''),
      productName: String(data['productName'] ?? data['ProductName'] ?? ''),
      unitSymbol: (data['unitSymbol'] ?? data['UnitSymbol'] ?? null) as string | null,
      systemQuantity: Number(data['systemQuantity'] ?? data['SystemQuantity'] ?? 0),
      countedQuantity: counted === null || counted === undefined ? null : Number(counted),
      lineNumber: Number(data['lineNumber'] ?? data['LineNumber'] ?? 0),
      hasVariance: Boolean(data['hasVariance'] ?? data['HasVariance'] ?? false),
    };
  }

  private normalizeLookups(raw: unknown): PhysicalCountLookupsResponse {
    const data = (raw ?? {}) as Record<string, unknown>;
    const warehouses = this.normalizeWarehouseOptions(data['warehouses'] ?? data['Warehouses']);
    return { warehouses };
  }

  private normalizeWarehouseOptions(raw: unknown): PhysicalCountLookupOption[] {
    if (!Array.isArray(raw)) return [];

    return raw.map((item) => {
      const option = item as Record<string, unknown>;
      const code = String(option['code'] ?? option['Code'] ?? '');
      const name = String(option['name'] ?? option['Name'] ?? '');
      const label = code && name ? `${code} — ${name}` : code || name;

      return {
        id: this.normalizeId(option['id'] ?? option['Id']),
        label,
      };
    });
  }

  private normalizeId(value: unknown): string {
    return String(value ?? '').trim().toLowerCase();
  }
}
