import { apiService } from './api.service';
import { AUTH_TOKEN_KEY, AUTH_USER_KEY } from '../config/constants';
import type { LoginCredentials, AuthResponse, User } from '../types/auth.types';

class AuthService {
  public async login(credentials: LoginCredentials): Promise<AuthResponse> {
    const response = await apiService.post<AuthResponse>('/auth/login', credentials);
    
    // Store token in localStorage
    if (response.token) {
      localStorage.setItem(AUTH_TOKEN_KEY, response.token);
      
      // Store user info
      const user: User = {
        username: credentials.username,
        token: response.token,
      };
      localStorage.setItem(AUTH_USER_KEY, JSON.stringify(user));
    }
    
    return response;
  }

  public logout(): void {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    localStorage.removeItem(AUTH_USER_KEY);
  }

  public getToken(): string | null {
    return localStorage.getItem(AUTH_TOKEN_KEY);
  }

  public getUser(): User | null {
    const userJson = localStorage.getItem(AUTH_USER_KEY);
    if (!userJson) return null;
    
    try {
      return JSON.parse(userJson) as User;
    } catch {
      return null;
    }
  }

  public isAuthenticated(): boolean {
    const token = this.getToken();
    return token !== null;
  }
}

export const authService = new AuthService();
export default authService;
