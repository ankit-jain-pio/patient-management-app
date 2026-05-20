import { describe, it, expect } from 'vitest';
import { render, screen, fireEvent } from '@testing-library/react';
import { KeyboardShortcutsDialog } from '../components/KeyboardShortcutsDialog';

describe('KeyboardShortcutsDialog', () => {
  it('should render help button', () => {
    render(<KeyboardShortcutsDialog />);
    expect(screen.getByLabelText('Show keyboard shortcuts')).toBeInTheDocument();
  });

  it('should open dialog when button clicked', () => {
    render(<KeyboardShortcutsDialog />);
    
    const button = screen.getByLabelText('Show keyboard shortcuts');
    fireEvent.click(button);

    expect(screen.getByText('Keyboard Shortcuts')).toBeInTheDocument();
  });

  it('should close dialog when close button clicked', () => {
    render(<KeyboardShortcutsDialog />);
    
    // Open dialog
    const button = screen.getByLabelText('Show keyboard shortcuts');
    fireEvent.click(button);

    // Close dialog
    const closeButtons = screen.getAllByRole('button');
    const closeButton = closeButtons.find((btn) => 
      btn.querySelector('.MuiSvgIcon-root')
    );
    
    if (closeButton) {
      fireEvent.click(closeButton);
    }

    // Dialog should be closed (not visible)
    expect(screen.queryByText('Global Shortcuts')).not.toBeVisible();
  });

  it('should display default shortcuts', () => {
    render(<KeyboardShortcutsDialog />);
    
    const button = screen.getByLabelText('Show keyboard shortcuts');
    fireEvent.click(button);

    expect(screen.getByText('Global Shortcuts')).toBeInTheDocument();
    expect(screen.getByText('Show keyboard shortcuts')).toBeInTheDocument();
    expect(screen.getByText('New item (context-dependent)')).toBeInTheDocument();
    expect(screen.getByText('Save (context-dependent)')).toBeInTheDocument();
  });

  it('should display custom shortcut groups', () => {
    const customGroups = [
      {
        title: 'Custom Shortcuts',
        shortcuts: [
          { key: 'x', ctrl: true, callback: () => {}, description: 'Custom action' },
        ],
      },
    ];

    render(<KeyboardShortcutsDialog shortcutGroups={customGroups} />);
    
    const button = screen.getByLabelText('Show keyboard shortcuts');
    fireEvent.click(button);

    expect(screen.getByText('Custom Shortcuts')).toBeInTheDocument();
    expect(screen.getByText('Custom action')).toBeInTheDocument();
  });

  it('should display tip about opening shortcuts', () => {
    render(<KeyboardShortcutsDialog />);
    
    const button = screen.getByLabelText('Show keyboard shortcuts');
    fireEvent.click(button);

    expect(screen.getByText(/anytime to view this list/)).toBeInTheDocument();
  });
});
