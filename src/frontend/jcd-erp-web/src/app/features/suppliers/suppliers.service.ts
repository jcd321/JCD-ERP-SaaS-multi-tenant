import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateSupplierRequest,
  SuppliersQueryParams,
  PaginatedSuppliersResponse,
  UpdateSupplierRequest,
} from './suppliers.models';

@Injectable({ providedIn: 'root' })
export class SuppliersService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/suppliers`;

  getSuppliers(params: SuppliersQueryParams = {}): Observable<PaginatedSuppliersResponse> {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.isActive !== undefined && params.isActive !== null) {
      httpParams = httpParams.set('isActive', params.isActive);
    }

    return this.http.get<PaginatedSuppliersResponse>(this.apiUrl, { params: httpParams });
  }

  createSupplier(request: CreateSupplierRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateSupplier(supplierId: string, request: UpdateSupplierRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${supplierId}`, request);
  }

  deleteSupplier(supplierId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${supplierId}`);
  }
}
