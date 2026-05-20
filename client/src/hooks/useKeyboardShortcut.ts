import { useEffect, useRef } from 'react';

export interface KeyboardShortcut {
  key: string;
  ctrl?: boolean;
  alt?: boolean;
  shift?: boolean;
  meta?: boolean;
  callback: () => void;
  description?: string;
}

// Check if the pressed keys match the shortcut
const matchesShortcut = (event: KeyboardEvent, shortcut: KeyboardShortcut): boolean => {
  const keyMatches = event.key.toLowerCase() === shortcut.key.toLowerCase();
  const ctrlMatches = shortcut.ctrl === undefined || event.ctrlKey === shortcut.ctrl;
  const altMatches = shortcut.alt === undefined || event.altKey === shortcut.alt;
  const shiftMatches = shortcut.shift === undefined || event.shiftKey === shortcut.shift;
  const metaMatches = shortcut.meta === undefined || event.metaKey === shortcut.meta;

  return keyMatches && ctrlMatches && altMatches && shiftMatches && metaMatches;
};

// Hook for keyboard shortcuts
export const useKeyboardShortcut = (shortcut: KeyboardShortcut, enabled: boolean = true): void => {
  const callbackRef = useRef(shortcut.callback);

  // Update callback ref when it changes
  useEffect(() => {
    callbackRef.current = shortcut.callback;
  }, [shortcut.callback]);

  useEffect(() => {
    if (!enabled) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      // Don't trigger shortcuts when typing in inputs (except for specific shortcuts)
      const target = event.target as HTMLElement;
      const isInputField =
        target.tagName === 'INPUT' ||
        target.tagName === 'TEXTAREA' ||
        target.isContentEditable;

      if (isInputField && !shortcut.ctrl && !shortcut.alt && !shortcut.meta) {
        return;
      }

      if (matchesShortcut(event, shortcut)) {
        event.preventDefault();
        callbackRef.current();
      }
    };

    window.addEventListener('keydown', handleKeyDown);

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [enabled, shortcut.key, shortcut.ctrl, shortcut.alt, shortcut.shift, shortcut.meta]);
};

// Hook for multiple keyboard shortcuts
export const useKeyboardShortcuts = (
  shortcuts: KeyboardShortcut[],
  enabled: boolean = true
): void => {
  const shortcutsRef = useRef(shortcuts);

  // Update shortcuts ref when they change
  useEffect(() => {
    shortcutsRef.current = shortcuts;
  }, [shortcuts]);

  useEffect(() => {
    if (!enabled) return;

    const handleKeyDown = (event: KeyboardEvent) => {
      // Don't trigger shortcuts when typing in inputs (except for specific shortcuts)
      const target = event.target as HTMLElement;
      const isInputField =
        target.tagName === 'INPUT' ||
        target.tagName === 'TEXTAREA' ||
        target.isContentEditable;

      for (const shortcut of shortcutsRef.current) {
        if (isInputField && !shortcut.ctrl && !shortcut.alt && !shortcut.meta) {
          continue;
        }

        if (matchesShortcut(event, shortcut)) {
          event.preventDefault();
          shortcut.callback();
          break; // Only trigger the first matching shortcut
        }
      }
    };

    window.addEventListener('keydown', handleKeyDown);

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [enabled]);
};

// Common keyboard shortcuts
export const commonShortcuts = {
  save: (callback: () => void): KeyboardShortcut => ({
    key: 's',
    ctrl: true,
    callback,
    description: 'Save (Ctrl+S)',
  }),

  search: (callback: () => void): KeyboardShortcut => ({
    key: 'f',
    ctrl: true,
    callback,
    description: 'Search (Ctrl+F)',
  }),

  newItem: (callback: () => void): KeyboardShortcut => ({
    key: 'n',
    ctrl: true,
    callback,
    description: 'New Item (Ctrl+N)',
  }),

  escape: (callback: () => void): KeyboardShortcut => ({
    key: 'Escape',
    callback,
    description: 'Cancel/Close (Esc)',
  }),

  enter: (callback: () => void): KeyboardShortcut => ({
    key: 'Enter',
    ctrl: true,
    callback,
    description: 'Submit (Ctrl+Enter)',
  }),

  refresh: (callback: () => void): KeyboardShortcut => ({
    key: 'r',
    ctrl: true,
    callback,
    description: 'Refresh (Ctrl+R)',
  }),

  help: (callback: () => void): KeyboardShortcut => ({
    key: '?',
    shift: true,
    callback,
    description: 'Help (Shift+?)',
  }),
};

// Format shortcut for display
export const formatShortcut = (shortcut: KeyboardShortcut): string => {
  const parts: string[] = [];

  if (shortcut.ctrl) parts.push('Ctrl');
  if (shortcut.alt) parts.push('Alt');
  if (shortcut.shift) parts.push('Shift');
  if (shortcut.meta) parts.push('Cmd');
  parts.push(shortcut.key.toUpperCase());

  return parts.join('+');
};
