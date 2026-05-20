import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import { ErrorBoundary } from '../components/ErrorBoundary';

// Component that throws an error
const ThrowError = ({ error }: { error?: Error }) => {
  if (error) {
    throw error;
  }
  return <div>No error</div>;
};

describe('ErrorBoundary', () => {
  // Suppress console.error for tests
  const originalError = console.error;
  beforeEach(() => {
    console.error = vi.fn();
  });

  afterEach(() => {
    console.error = originalError;
  });

  it('should render children when no error', () => {
    render(
      <ErrorBoundary>
        <div>Test Content</div>
      </ErrorBoundary>
    );

    expect(screen.getByText('Test Content')).toBeInTheDocument();
  });

  it('should render error UI when error occurs', () => {
    const error = new Error('Test error');

    render(
      <ErrorBoundary>
        <ThrowError error={error} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Something went wrong')).toBeInTheDocument();
    expect(screen.getByText(/An unexpected error occurred/)).toBeInTheDocument();
  });

  it('should render custom fallback when provided', () => {
    const error = new Error('Test error');
    const fallback = <div>Custom Error UI</div>;

    render(
      <ErrorBoundary fallback={fallback}>
        <ThrowError error={error} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Custom Error UI')).toBeInTheDocument();
  });

  it('should render custom fallback as ReactNode', () => {
    const error = new Error('Test error');
    const fallback = <div>Custom Error UI</div>;

    render(
      <ErrorBoundary fallback={fallback}>
        <ThrowError error={error} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Custom Error UI')).toBeInTheDocument();
  });

  it('should render reset button', () => {
    const error = new Error('Test error');

    render(
      <ErrorBoundary>
        <ThrowError error={error} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Try Again')).toBeInTheDocument();
  });

  it('should render reload button', () => {
    const error = new Error('Test error');

    render(
      <ErrorBoundary>
        <ThrowError error={error} />
      </ErrorBoundary>
    );

    expect(screen.getByText('Reload Page')).toBeInTheDocument();
  });

  it('should not render error details in production mode', () => {
    const error = new Error('Test error');
    const originalNodeEnv = process.env.NODE_ENV;
    process.env.NODE_ENV = 'production';

    render(
      <ErrorBoundary>
        <ThrowError error={error} />
      </ErrorBoundary>
    );

    expect(screen.queryByText(/Error Details/)).not.toBeInTheDocument();

    process.env.NODE_ENV = originalNodeEnv;
  });
});
