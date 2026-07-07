import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  BrandsQueryParams,
  CreateBrandRequest,
  PaginatedBrandsResponse,
  UpdateBrandRequest,
} from './brands.models';

@Injectable({ providedIn: 'root' })
export class BrandsService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/brands`;

  getBrands(params: BrandsQueryParams = {}): Observable<PaginatedBrandsResponse> {
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

    return this.http.get<PaginatedBrandsResponse>(this.apiUrl, { params: httpParams });
  }

  createBrand(request: CreateBrandRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateBrand(brandId: string, request: UpdateBrandRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${brandId}`, request);
  }

  deleteBrand(brandId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${brandId}`);
  }
}
