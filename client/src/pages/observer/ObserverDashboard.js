import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { 
  Dashboard as DashboardIcon, 
  Business as BusinessIcon, 
  TrendingUp as TrendingUpIcon, 
  Analytics as AnalyticsIcon
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import ObserverHome from './ObserverHome';
import ObserverCompanies from './ObserverCompanies';
import { observerService, isDemoMode } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';
import { Paper, Typography, Box, CircularProgress } from '@mui/material';
import { useTranslation } from 'react-i18next';

// Placeholder components for future implementation
const ObserverTransactions = () => {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>{t('menu.transactions')}</Typography>
      <Typography>This feature will be available in the complete version.</Typography>
    </Paper>
  );
};

const ObserverAnalytics = () => {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>{t('menu.analytics')}</Typography>
      <Typography>This feature will be available in the complete version.</Typography>
    </Paper>
  );
};

const ObserverDashboard = () => {
  const { user } = useAuth();
  const { t } = useTranslation();
  const [userContext, setUserContext] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUserContext = async () => {
      try {
        setLoading(true);
        
        if (isDemoMode()) {
          // If in demo mode, use data from localStorage/auth context
          setUserContext({
            username: user?.username || localStorage.getItem('username') || 'Observer User',
            role: 'CompanyObserver'
          });
          setLoading(false);
          return;
        }
        
        // Try the real API
        const response = await observerService.getContext();
        setUserContext(response.data.context);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching user context:', error);
        // Fall back to demo data even if not explicitly in demo mode
        setUserContext({
          username: user?.username || localStorage.getItem('username') || 'Observer User',
          role: 'CompanyObserver'
        });
        setError('Unable to connect to the server. Using demo mode.');
        setLoading(false);
      }
    };
    
    fetchUserContext();
  }, [user]);

  const menuItems = [
    { label: t('menu.dashboard'), translationKey: 'menu.dashboard', path: '/observer', icon: <DashboardIcon /> },
    { label: t('menu.companies'), translationKey: 'menu.companies', path: '/observer/companies', icon: <BusinessIcon /> },
    { label: t('menu.transactions'), translationKey: 'menu.transactions', path: '/observer/transactions', icon: <TrendingUpIcon /> },
    { label: t('menu.analytics'), translationKey: 'menu.analytics', path: '/observer/analytics', icon: <AnalyticsIcon /> },
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <DashboardLayout title={t('dashboard.observer')} menuItems={menuItems}>
      {error && (
        <Paper sx={{ p: 2, mb: 3, bgcolor: 'warning.light' }}>
          <Typography color="warning.dark">{error}</Typography>
        </Paper>
      )}
      <Routes>
        <Route path="/" element={<ObserverHome />} />
        <Route path="/companies" element={<ObserverCompanies />} />
        <Route path="/transactions" element={<ObserverTransactions />} />
        <Route path="/analytics" element={<ObserverAnalytics />} />
        <Route path="*" element={<Navigate to="/observer" replace />} />
      </Routes>
    </DashboardLayout>
  );
};

export default ObserverDashboard;
