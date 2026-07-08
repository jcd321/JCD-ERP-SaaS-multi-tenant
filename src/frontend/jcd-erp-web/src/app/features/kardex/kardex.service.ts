import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  KardexLookupsResponse,
  KardexQueryParams,
  PaginatedKardexResponse,
} from './kardex.models';

@Injectable({ providedIn: 'root' })
export class KardexService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/kardex`;

  getKardex(params: KardexQueryParams): Observable<PaginatedKardexResponse> {
    let httpParams = new HttpParams();

    if (params.productId) httpParams = httpParams.set('productId', params.productId);
    if (params.page) httpParams = httpParams.set('page', params.page);
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize);
    if (params.warehouseId) httpParams = httpParams.set('warehouseId', params.warehouseId);
    if (params.fromDate) httpParams = httpParams.set('fromDate', params.fromDate);
    if (params.toDate) httpParams = httpParams.set('toDate', params.toDate);

    return this.http.get<PaginatedKardexResponse>(this.apiUrl, { params: httpParams });
  }

  getLookups(): Observable<KardexLookupsResponse> {
    return this.http.get<KardexLookupsResponse>(`${this.apiUrl}/lookups`);
  }
}
