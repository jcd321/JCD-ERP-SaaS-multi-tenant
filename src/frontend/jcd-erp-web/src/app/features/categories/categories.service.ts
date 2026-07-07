import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CategoriesQueryParams,
  CategoryParentOption,
  CreateCategoryRequest,
  PaginatedCategoriesResponse,
  UpdateCategoryRequest,
} from './categories.models';

@Injectable({ providedIn: 'root' })
export class CategoriesService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/categories`;

  getCategories(params: CategoriesQueryParams = {}): Observable<PaginatedCategoriesResponse> {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.isActive !== undefined && params.isActive !== null) {
      httpParams = httpParams.set('isActive', params.isActive);
    }

    return this.http.get<PaginatedCategoriesResponse>(this.apiUrl, { params: httpParams });
  }

  getParentOptions(excludeId?: string): Observable<CategoryParentOption[]> {
    let params = new HttpParams();
    if (excludeId) params = params.set('excludeId', excludeId);
    return this.http.get<CategoryParentOption[]>(`${this.apiUrl}/parent-options`, { params });
  }

  createCategory(request: CreateCategoryRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateCategory(categoryId: string, request: UpdateCategoryRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${categoryId}`, request);
  }

  deleteCategory(categoryId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${categoryId}`);
  }
}
