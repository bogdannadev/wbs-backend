import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Box, CircularProgress, Typography, Paper } from '@mui/material';
import { isDemoMode } from '../../services/api';
import { useTranslation } from 'react-i18next';

const ProtectedRoute = ({ children, roles = [] }) => {
  const { user, loading } = useAuth();
  const { t } = useTranslation();

  // Show loading state
  if (loading) {
    return (
      <Box sx={{ 
        display: 'flex', 
        flexDirection: 'column',
        alignItems: 'center', 
        justifyContent: 'center', 
        height: '100vh' 
      }}>
        <CircularProgress size={40} />
        <Typography variant="body1" sx={{ mt: 2 }}>
          {t('common.loadingUserData')}
        </Typography>
      </Box>
    );
  }

  // Special handling for demo mode with missing authentication
  if (!user && isDemoMode()) {
    // In demo mode without user, get the role from the path and try to recover
    const path = window.location.pathname;
    let role = null;
    
    if (path.startsWith('/buyer')) role = 'Buyer';
    else if (path.startsWith('/seller')) role = 'Seller';
    else if (path.startsWith('/admin')) role = 'SystemAdmin';
    else if (path.startsWith('/observer')) role = 'CompanyObserver';
    
    if (role && (!roles.length || roles.includes(role))) {
      // Show a warning but allow access in demo mode
      return (
        <>
          <Paper sx={{ p: 2, m: 2, bgcolor: 'warning.light' }}>
            <Typography color="warning.dark">
              Authentication warning: Session data is missing but demo mode is active. 
              To avoid this message, please logout and log in again.
            </Typography>
          </Paper>
          {children}
        </>
      );
    }
  }

  // If not authenticated, redirect to login
  if (!user) {
    return <Navigate to="/" replace />;
  }

  // If roles are specified, check if user has permission
  if (roles.length > 0 && !roles.includes(user.role)) {
    // Redirect to the appropriate dashboard based on role
    switch (user.role) {
      case 'Buyer':
        return <Navigate to="/buyer" replace />;
      case 'Seller':
        return <Navigate to="/seller" replace />;
      case 'SystemAdmin':
        return <Navigate to="/admin" replace />;
      case 'CompanyObserver':
      case 'SystemObserver':
        return <Navigate to="/observer" replace />;
      default:
        return <Navigate to="/" replace />;
    }
  }

  // If they have permission, show the children
  return children;
};

export default ProtectedRoute;
