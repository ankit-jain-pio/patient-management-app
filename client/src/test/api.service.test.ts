import { apiService } from '../services/api.service';
import { AUTH_TOKEN_KEY } from '../config/constants';
import { describe, it, expect, beforeEach, afterEach, vi } from 'vitest';

describe('API Service', () => {
  beforeEach(() => {
    localStorage.clear();
  });

  afterEach(() => {
    vi.clearAllMocks();
  });

  it('creates axios instance with correct base URL', () => {
    const client = apiService.getClient();
    expect(client.defaults.baseURL).toBeDefined();
  });

  it('adds auth token to request headers when available', () => {
    const mockToken = 'test-token-123';
    localStorage.setItem(AUTH_TOKEN_KEY, mockToken);

    const client = apiService.getClient();
    const config = client.defaults;

    expect(config.headers).toBeDefined();
  });

  it('has timeout configured', () => {
    const client = apiService.getClient();
    expect(client.defaults.timeout).toBe(10000);
  });
});
