import { renderHook, act } from '@testing-library/react';
import { AuthProvider, useAuth } from '../contexts/AuthContext';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import * as authService from '../services/auth.service';

// Mock the auth service
vi.mock('../services/auth.service', () => ({
  authService: {
    login: vi.fn(),
    logout: vi.fn(),
    getToken: vi.fn(() => null),
    getUser: vi.fn(() => null),
    isAuthenticated: vi.fn(() => false),
  },
}));

describe('AuthContext', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('provides authentication state', () => {
    const { result } = renderHook(() => useAuth(), {
      wrapper: AuthProvider,
    });

    expect(result.current.isAuthenticated).toBe(false);
    expect(result.current.user).toBeNull();
  });

  it('updates state after successful login', async () => {
    const mockToken = 'mock-token-123';
    vi.mocked(authService.authService.login).mockResolvedValue({
      token: mockToken,
      expiresAt: new Date(Date.now() + 8 * 60 * 60 * 1000).toISOString(),
    });

    const { result } = renderHook(() => useAuth(), {
      wrapper: AuthProvider,
    });

    await act(async () => {
      await result.current.login({ username: 'admin', password: 'password' });
    });

    expect(result.current.isAuthenticated).toBe(true);
    expect(result.current.user).toEqual({
      username: 'admin',
      token: mockToken,
    });
  });

  it('clears state after logout', async () => {
    const mockToken = 'mock-token-123';
    vi.mocked(authService.authService.login).mockResolvedValue({
      token: mockToken,
      expiresAt: new Date(Date.now() + 8 * 60 * 60 * 1000).toISOString(),
    });

    const { result } = renderHook(() => useAuth(), {
      wrapper: AuthProvider,
    });

    await act(async () => {
      await result.current.login({ username: 'admin', password: 'password' });
    });

    expect(result.current.isAuthenticated).toBe(true);

    act(() => {
      result.current.logout();
    });

    expect(result.current.isAuthenticated).toBe(false);
    expect(result.current.user).toBeNull();
  });
});
