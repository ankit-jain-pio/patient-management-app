import { describe, it, expect } from 'vitest';
import { renderHook, act } from '@testing-library/react';
import { LoadingProvider, useLoading } from '../contexts/LoadingContext';

describe('LoadingContext', () => {
  it('should provide loading state', () => {
    const { result } = renderHook(() => useLoading(), {
      wrapper: LoadingProvider,
    });

    expect(result.current.isLoading).toBe(false);
  });

  it('should show loading', () => {
    const { result } = renderHook(() => useLoading(), {
      wrapper: LoadingProvider,
    });

    act(() => {
      result.current.showLoading();
    });

    expect(result.current.isLoading).toBe(true);
  });

  it('should hide loading', () => {
    const { result } = renderHook(() => useLoading(), {
      wrapper: LoadingProvider,
    });

    act(() => {
      result.current.showLoading();
    });

    expect(result.current.isLoading).toBe(true);

    act(() => {
      result.current.hideLoading();
    });

    expect(result.current.isLoading).toBe(false);
  });

  it('should handle multiple show/hide calls', () => {
    const { result } = renderHook(() => useLoading(), {
      wrapper: LoadingProvider,
    });

    act(() => {
      result.current.showLoading();
      result.current.showLoading();
    });

    expect(result.current.isLoading).toBe(true);

    act(() => {
      result.current.hideLoading();
    });

    // Since implementation uses simple boolean, not counter
    expect(result.current.isLoading).toBe(false);
  });

  it('should throw error when used outside provider', () => {
    expect(() => {
      renderHook(() => useLoading());
    }).toThrow('useLoading must be used within a LoadingProvider');
  });
});
