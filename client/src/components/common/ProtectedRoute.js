import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { Box, CircularProgress, Typography } from '@mui/material';

const ProtectedRoute = ({ children, roles = [] }) => {
  const { user, loading } = useAuth();

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
          Loading user data...
        </Typography>
      </Box>
    );
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
