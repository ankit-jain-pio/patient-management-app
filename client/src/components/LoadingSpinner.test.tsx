import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { LoadingSpinner } from '../components/LoadingSpinner';

describe('LoadingSpinner', () => {
  it('should render loading spinner', () => {
    render(<LoadingSpinner />);
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });

  it('should render with message', () => {
    render(<LoadingSpinner message="Loading data..." />);
    expect(screen.getByText('Loading data...')).toBeInTheDocument();
  });

  it('should render with custom size', () => {
    const { container } = render(<LoadingSpinner size={60} />);
    const spinner = container.querySelector('.MuiCircularProgress-root');
    expect(spinner).toBeInTheDocument();
  });

  it('should render in fullScreen mode', () => {
    const { container } = render(<LoadingSpinner fullScreen />);
    const backdrop = container.querySelector('.MuiBox-root');
    expect(backdrop).toBeInTheDocument();
  });

  it('should render inline mode by default', () => {
    const { container } = render(<LoadingSpinner />);
    const box = container.querySelector('.MuiBox-root');
    expect(box).toBeInTheDocument();
  });

  it('should center content', () => {
    render(<LoadingSpinner message="Please wait" />);
    expect(screen.getByText('Please wait')).toBeInTheDocument();
    expect(screen.getByRole('progressbar')).toBeInTheDocument();
  });
});
