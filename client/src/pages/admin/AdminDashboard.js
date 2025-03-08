import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { 
  Dashboard as DashboardIcon, 
  Business as BusinessIcon, 
  Store as StoreIcon, 
  Notifications as NotificationsIcon, 
  BarChart as BarChartIcon, 
  People as PeopleIcon
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import AdminHome from './AdminHome';
import AdminCompanies from './AdminCompanies';
import AdminUsers from './AdminUsers';
import { adminService, isDemoMode } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';
import { Paper, Typography, Box, CircularProgress } from '@mui/material';
import { useTranslation } from 'react-i18next';

// Placeholder components for future implementation
const AdminStores = () => {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>Store Management</Typography>
      <Typography>This feature will be available in the complete version.</Typography>
    </Paper>
  );
};

const AdminNotifications = () => {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>{t('menu.notifications')}</Typography>
      <Typography>This feature will be available in the complete version.</Typography>
    </Paper>
  );
};

const AdminReports = () => {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>System Reports</Typography>
      <Typography>This feature will be available in the complete version.</Typography>
    </Paper>
  );
};

const AdminDashboard = () => {
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
            username: user?.username || localStorage.getItem('username') || 'Admin User',
            role: 'SystemAdmin'
          });
          setLoading(false);
          return;
        }
        
        // Try the real API
        const response = await adminService.getContext();
        setUserContext(response.data.context);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching user context:', error);
        // Fall back to demo data even if not explicitly in demo mode
        setUserContext({
          username: user?.username || localStorage.getItem('username') || 'Admin User',
          role: 'SystemAdmin'
        });
        setError('Unable to connect to the server. Using demo mode.');
        setLoading(false);
      }
    };
    
    fetchUserContext();
  }, [user]);

  const menuItems = [
    { label: t('menu.dashboard'), translationKey: 'menu.dashboard', path: '/admin', icon: <DashboardIcon /> },
    { label: t('menu.companies'), translationKey: 'menu.companies', path: '/admin/companies', icon: <BusinessIcon /> },
    { label: t('menu.stores'), translationKey: 'menu.stores', path: '/admin/stores', icon: <StoreIcon /> },
    { label: t('menu.users'), translationKey: 'menu.users', path: '/admin/users', icon: <PeopleIcon /> },
    { label: t('menu.notifications'), translationKey: 'menu.notifications', path: '/admin/notifications', icon: <NotificationsIcon /> },
    { label: t('menu.reports'), translationKey: 'menu.reports', path: '/admin/reports', icon: <BarChartIcon /> },
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <DashboardLayout title={t('dashboard.admin')} menuItems={menuItems}>
      {error && (
        <Paper sx={{ p: 2, mb: 3, bgcolor: 'warning.light' }}>
          <Typography color="warning.dark">{error}</Typography>
        </Paper>
      )}
      <Routes>
        <Route path="/" element={<AdminHome />} />
        <Route path="/companies" element={<AdminCompanies />} />
        <Route path="/stores" element={<AdminStores />} />
        <Route path="/users" element={<AdminUsers />} />
        <Route path="/notifications" element={<AdminNotifications />} />
        <Route path="/reports" element={<AdminReports />} />
        <Route path="*" element={<Navigate to="/admin" replace />} />
      </Routes>
    </DashboardLayout>
  );
};

export default AdminDashboard;
