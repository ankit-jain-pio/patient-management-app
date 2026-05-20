import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from '../contexts/AuthContext';
import { Login } from '../pages/Login';
import { describe, it, expect, vi, beforeEach } from 'vitest';
import * as authService from '../services/auth.service';

// Mock the auth service
vi.mock('../services/auth.service', () => ({
  authService: {
    login: vi.fn(),
    logout: vi.fn(),
    getToken: vi.fn(),
    getUser: vi.fn(),
    isAuthenticated: vi.fn(),
  },
}));

const renderWithRouter = (component: React.ReactElement) => {
  return render(
    <BrowserRouter>
      <AuthProvider>{component}</AuthProvider>
    </BrowserRouter>
  );
};

describe('Login Component', () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  it('renders login form', () => {
    renderWithRouter(<Login />);
    
    expect(screen.getByText('Patient Management System')).toBeInTheDocument();
    expect(screen.getByLabelText(/username/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
  });

  it('disables submit button when fields are empty', () => {
    renderWithRouter(<Login />);
    
    const submitButton = screen.getByRole('button', { name: /sign in/i });
    expect(submitButton).toBeDisabled();
  });

  it('enables submit button when fields are filled', async () => {
    const user = userEvent.setup();
    renderWithRouter(<Login />);
    
    const usernameInput = screen.getByLabelText(/username/i);
    const passwordInput = screen.getByLabelText(/password/i);
    const submitButton = screen.getByRole('button', { name: /sign in/i });

    await user.type(usernameInput, 'admin');
    await user.type(passwordInput, 'password');

    expect(submitButton).not.toBeDisabled();
  });

  it('displays error message on login failure', async () => {
    const user = userEvent.setup();
    vi.mocked(authService.authService.login).mockRejectedValue(new Error('Login failed'));
    
    renderWithRouter(<Login />);
    
    const usernameInput = screen.getByLabelText(/username/i);
    const passwordInput = screen.getByLabelText(/password/i);
    const submitButton = screen.getByRole('button', { name: /sign in/i });

    await user.type(usernameInput, 'admin');
    await user.type(passwordInput, 'wrongpassword');
    await user.click(submitButton);

    await waitFor(() => {
      expect(screen.getByText(/invalid username or password/i)).toBeInTheDocument();
    });
  });
});
