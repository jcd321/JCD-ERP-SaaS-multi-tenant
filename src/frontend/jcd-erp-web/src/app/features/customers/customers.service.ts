import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  CreateCustomerRequest,
  CustomersQueryParams,
  PaginatedCustomersResponse,
  UpdateCustomerRequest,
} from './customers.models';

@Injectable({ providedIn: 'root' })
export class CustomersService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/customers`;

  getCustomers(params: CustomersQueryParams = {}): Observable<PaginatedCustomersResponse> {
    let httpParams = new HttpParams();

    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.search?.trim()) httpParams = httpParams.set('search', params.search.trim());
    if (params.isActive !== undefined && params.isActive !== null) {
      httpParams = httpParams.set('isActive', params.isActive);
    }

    return this.http.get<PaginatedCustomersResponse>(this.apiUrl, { params: httpParams });
  }

  createCustomer(request: CreateCustomerRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(this.apiUrl, request);
  }

  updateCustomer(customerId: string, request: UpdateCustomerRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${customerId}`, request);
  }

  deleteCustomer(customerId: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${customerId}`);
  }
}
