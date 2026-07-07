import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment';
import {
  LoginRequest,
  LoginResponse,
  LogoutRequest,
  RefreshTokenRequest,
  RefreshTokenResponse,
  RegisterRequest,
  RegisterResponse,
} from './auth.models';

@Injectable({ providedIn: 'root' })
export class AuthApiService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/api/v1/auth`;

  login(request: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, request);
  }

  register(request: RegisterRequest): Observable<RegisterResponse> {
    return this.http.post<RegisterResponse>(`${this.apiUrl}/register`, request);
  }

  refreshToken(refreshToken: string): Observable<RefreshTokenResponse> {
    const body: RefreshTokenRequest = { refreshToken };
    return this.http.post<RefreshTokenResponse>(`${this.apiUrl}/refresh`, body);
  }

  logout(refreshToken: string | null): Observable<void> {
    const body: LogoutRequest = { refreshToken };
    return this.http.post<void>(`${this.apiUrl}/logout`, body);
  }
}
