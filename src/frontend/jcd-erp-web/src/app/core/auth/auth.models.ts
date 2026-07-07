export interface LoginRequest {
  email: string;
  password: string;
  tenantSlug: string | null;
  rememberMe: boolean;
}

export interface RegisterRequest {
  companyName: string;
  slug: string | null;
  adminEmail: string;
  adminPassword: string;
  adminFirstName: string;
  adminLastName: string;
}

export interface AuthTokens {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAt: string;
  refreshTokenExpiresAt: string;
}

export interface LoginResponse extends AuthTokens {
  tenantId: string;
  tenantSlug: string;
  userId: string;
  email: string;
  fullName: string;
  permissions: string[];
}

export interface RegisterResponse extends AuthTokens {
  tenantId: string;
  tenantSlug: string;
  userId: string;
}

export interface RefreshTokenRequest {
  refreshToken: string;
}

export interface RefreshTokenResponse extends AuthTokens {}

export interface LogoutRequest {
  refreshToken: string | null;
}

export interface ForgotPasswordRequest {
  email: string;
  tenantSlug: string | null;
}

export interface ResetPasswordRequest {
  token: string;
  email: string;
  newPassword: string;
}

export interface AuthSession {
  tenantId: string;
  tenantSlug: string;
  userId: string;
  email: string;
  fullName: string;
  permissions: string[];
}

export interface ApiError {
  error?: string;
}
