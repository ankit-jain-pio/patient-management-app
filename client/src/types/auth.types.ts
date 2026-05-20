export interface LoginCredentials {
  username: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  expiresAt: string;
}

export interface User {
  username: string;
  token: string;
}
