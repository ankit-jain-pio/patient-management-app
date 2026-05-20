import { describe, it, expect, vi, beforeEach } from 'vitest';
import { renderHook } from '@testing-library/react';
import {
  useKeyboardShortcut,
  useKeyboardShortcuts,
  formatShortcut,
  commonShortcuts,
} from '../hooks/useKeyboardShortcut';

describe('useKeyboardShortcut', () => {
  beforeEach(() => {
    // Clear any existing event listeners
    vi.clearAllMocks();
  });

  it('should call callback when shortcut is pressed', () => {
    const callback = vi.fn();
    const shortcut = { key: 's', ctrl: true, callback };

    renderHook(() => useKeyboardShortcut(shortcut));

    // Simulate Ctrl+S
    const event = new KeyboardEvent('keydown', {
      key: 's',
      ctrlKey: true,
    });
    window.dispatchEvent(event);

    expect(callback).toHaveBeenCalledTimes(1);
  });

  it('should not call callback when disabled', () => {
    const callback = vi.fn();
    const shortcut = { key: 's', ctrl: true, callback };

    renderHook(() => useKeyboardShortcut(shortcut, false));

    const event = new KeyboardEvent('keydown', {
      key: 's',
      ctrlKey: true,
    });
    window.dispatchEvent(event);

    expect(callback).not.toHaveBeenCalled();
  });

  it('should not call callback for wrong key', () => {
    const callback = vi.fn();
    const shortcut = { key: 's', ctrl: true, callback };

    renderHook(() => useKeyboardShortcut(shortcut));

    const event = new KeyboardEvent('keydown', {
      key: 'a',
      ctrlKey: true,
    });
    window.dispatchEvent(event);

    expect(callback).not.toHaveBeenCalled();
  });

  it('should handle shortcuts with multiple modifiers', () => {
    const callback = vi.fn();
    const shortcut = { key: 's', ctrl: true, shift: true, callback };

    renderHook(() => useKeyboardShortcut(shortcut));

    const event = new KeyboardEvent('keydown', {
      key: 's',
      ctrlKey: true,
      shiftKey: true,
    });
    window.dispatchEvent(event);

    expect(callback).toHaveBeenCalledTimes(1);
  });

  it('should clean up event listeners on unmount', () => {
    const callback = vi.fn();
    const shortcut = { key: 's', ctrl: true, callback };

    const { unmount } = renderHook(() => useKeyboardShortcut(shortcut));

    unmount();

    const event = new KeyboardEvent('keydown', {
      key: 's',
      ctrlKey: true,
    });
    window.dispatchEvent(event);

    expect(callback).not.toHaveBeenCalled();
  });
});

describe('useKeyboardShortcuts', () => {
  it('should handle multiple shortcuts', () => {
    const callback1 = vi.fn();
    const callback2 = vi.fn();
    const shortcuts = [
      { key: 's', ctrl: true, callback: callback1 },
      { key: 'n', ctrl: true, callback: callback2 },
    ];

    renderHook(() => useKeyboardShortcuts(shortcuts));

    const event1 = new KeyboardEvent('keydown', {
      key: 's',
      ctrlKey: true,
    });
    window.dispatchEvent(event1);

    expect(callback1).toHaveBeenCalledTimes(1);

    const event2 = new KeyboardEvent('keydown', {
      key: 'n',
      ctrlKey: true,
    });
    window.dispatchEvent(event2);

    expect(callback2).toHaveBeenCalledTimes(1);
  });

  it('should only trigger first matching shortcut', () => {
    const callback1 = vi.fn();
    const callback2 = vi.fn();
    const shortcuts = [
      { key: 's', ctrl: true, callback: callback1 },
      { key: 's', ctrl: true, callback: callback2 },
    ];

    renderHook(() => useKeyboardShortcuts(shortcuts));

    const event = new KeyboardEvent('keydown', {
      key: 's',
      ctrlKey: true,
    });
    window.dispatchEvent(event);

    expect(callback1).toHaveBeenCalledTimes(1);
    expect(callback2).not.toHaveBeenCalled();
  });
});

describe('formatShortcut', () => {
  it('should format single key', () => {
    const shortcut = { key: 's', callback: () => {} };
    expect(formatShortcut(shortcut)).toBe('S');
  });

  it('should format Ctrl+Key', () => {
    const shortcut = { key: 's', ctrl: true, callback: () => {} };
    expect(formatShortcut(shortcut)).toBe('Ctrl+S');
  });

  it('should format Shift+Key', () => {
    const shortcut = { key: '?', shift: true, callback: () => {} };
    expect(formatShortcut(shortcut)).toBe('Shift+?');
  });

  it('should format complex shortcut', () => {
    const shortcut = { key: 's', ctrl: true, shift: true, callback: () => {} };
    expect(formatShortcut(shortcut)).toBe('Ctrl+Shift+S');
  });
});

describe('commonShortcuts', () => {
  it('should create save shortcut', () => {
    const callback = vi.fn();
    const shortcut = commonShortcuts.save(callback);

    expect(shortcut.key).toBe('s');
    expect(shortcut.ctrl).toBe(true);
    expect(shortcut.description).toBe('Save (Ctrl+S)');
  });

  it('should create escape shortcut', () => {
    const callback = vi.fn();
    const shortcut = commonShortcuts.escape(callback);

    expect(shortcut.key).toBe('Escape');
    expect(shortcut.description).toBe('Cancel/Close (Esc)');
  });

  it('should create help shortcut', () => {
    const callback = vi.fn();
    const shortcut = commonShortcuts.help(callback);

    expect(shortcut.key).toBe('?');
    expect(shortcut.shift).toBe(true);
    expect(shortcut.description).toBe('Help (Shift+?)');
  });
});
