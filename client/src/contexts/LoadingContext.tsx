import React, { createContext, useContext, useState } from 'react';
import type { ReactNode } from 'react';
import { Backdrop, CircularProgress, Box, Typography } from '@mui/material';

interface LoadingContextType {
  isLoading: boolean;
  loadingMessage: string;
  showLoading: (message?: string) => void;
  hideLoading: () => void;
}

const LoadingContext = createContext<LoadingContextType | undefined>(undefined);

export const useLoading = (): LoadingContextType => {
  const context = useContext(LoadingContext);
  if (!context) {
    throw new Error('useLoading must be used within a LoadingProvider');
  }
  return context;
};

interface LoadingProviderProps {
  children: ReactNode;
}

export const LoadingProvider: React.FC<LoadingProviderProps> = ({ children }) => {
  const [isLoading, setIsLoading] = useState(false);
  const [loadingMessage, setLoadingMessage] = useState('');

  const showLoading = (message: string = 'Loading...') => {
    setLoadingMessage(message);
    setIsLoading(true);
  };

  const hideLoading = () => {
    setIsLoading(false);
    setLoadingMessage('');
  };

  return (
    <LoadingContext.Provider value={{ isLoading, loadingMessage, showLoading, hideLoading }}>
      {children}
      <Backdrop
        sx={{
          color: '#fff',
          zIndex: (theme) => theme.zIndex.drawer + 1000,
          backdropFilter: 'blur(3px)',
        }}
        open={isLoading}
      >
        <Box
          sx={{
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
            gap: 2,
          }}
        >
          <CircularProgress color="inherit" size={60} />
          {loadingMessage && (
            <Typography variant="h6" sx={{ textShadow: '0 2px 4px rgba(0,0,0,0.5)' }}>
              {loadingMessage}
            </Typography>
          )}
        </Box>
      </Backdrop>
    </LoadingContext.Provider>
  );
};
