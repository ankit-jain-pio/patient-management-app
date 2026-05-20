import { Component } from 'react';
import type { ErrorInfo, ReactNode } from 'react';
import { Box, Button, Paper, Typography, Alert } from '@mui/material';
import { Refresh as RefreshIcon } from '@mui/icons-material';

interface ErrorBoundaryProps {
  children: ReactNode;
  fallback?: ReactNode;
}

interface ErrorBoundaryState {
  hasError: boolean;
  error: Error | null;
  errorInfo: ErrorInfo | null;
}

export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
    };
  }

  static getDerivedStateFromError(error: Error): Partial<ErrorBoundaryState> {
    return {
      hasError: true,
      error,
    };
  }

  componentDidCatch(error: Error, errorInfo: ErrorInfo): void {
    console.error('Error caught by ErrorBoundary:', error, errorInfo);
    this.setState({
      error,
      errorInfo,
    });

    // Log to error tracking service (e.g., Sentry, LogRocket)
    // errorTrackingService.log(error, errorInfo);
  }

  handleReset = (): void => {
    this.setState({
      hasError: false,
      error: null,
      errorInfo: null,
    });
  };

  handleReload = (): void => {
    window.location.reload();
  };

  render(): ReactNode {
    if (this.state.hasError) {
      // Custom fallback UI provided
      if (this.props.fallback) {
        return this.props.fallback;
      }

      // Default error UI
      return (
        <Box
          sx={{
            display: 'flex',
            justifyContent: 'center',
            alignItems: 'center',
            minHeight: '400px',
            p: 3,
          }}
        >
          <Paper sx={{ p: 4, maxWidth: '600px' }}>
            <Alert severity="error" sx={{ mb: 3 }}>
              <Typography variant="h6" gutterBottom>
                Something went wrong
              </Typography>
              <Typography variant="body2" gutterBottom>
                An unexpected error occurred. Please try refreshing the page or contact support if the
                problem persists.
              </Typography>
            </Alert>

            {import.meta.env.DEV && this.state.error && (
              <Box sx={{ mb: 3 }}>
                <Typography variant="subtitle2" gutterBottom>
                  Error Details:
                </Typography>
                <Paper
                  variant="outlined"
                  sx={{
                    p: 2,
                    bgcolor: 'grey.100',
                    maxHeight: '200px',
                    overflow: 'auto',
                  }}
                >
                  <Typography variant="body2" component="pre" sx={{ whiteSpace: 'pre-wrap', m: 0 }}>
                    {this.state.error.message}
                    {this.state.errorInfo?.componentStack}
                  </Typography>
                </Paper>
              </Box>
            )}

            <Box sx={{ display: 'flex', gap: 2 }}>
              <Button variant="contained" startIcon={<RefreshIcon />} onClick={this.handleReload}>
                Reload Page
              </Button>
              <Button variant="outlined" onClick={this.handleReset}>
                Try Again
              </Button>
            </Box>
          </Paper>
        </Box>
      );
    }

    return this.props.children;
  }
}
