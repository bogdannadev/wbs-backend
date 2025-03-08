import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { 
  Dashboard as DashboardIcon, 
  History as HistoryIcon, 
  QrCode as QrCodeIcon, 
  Store as StoreIcon 
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import BuyerHome from './BuyerHome';
import BuyerTransactions from './BuyerTransactions';
import BuyerQrCode from './BuyerQrCode';
import BuyerStores from './BuyerStores';
import { buyerService, isDemoMode } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';
import { Paper, Typography, Box, CircularProgress } from '@mui/material';
import { useTranslation } from 'react-i18next';

const BuyerDashboard = () => {
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
            username: user?.username || localStorage.getItem('username') || 'Demo User',
            role: 'Buyer',
            bonusBalance: user?.bonusBalance || parseInt(localStorage.getItem('bonusBalance') || '450')
          });
          setLoading(false);
          return;
        }
        
        // Try the real API
        const response = await buyerService.getContext();
        setUserContext(response.data.context);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching user context:', error);
        // Fall back to demo data even if not explicitly in demo mode
        setUserContext({
          username: user?.username || localStorage.getItem('username') || 'User',
          role: 'Buyer',
          bonusBalance: parseInt(localStorage.getItem('bonusBalance') || '450')
        });
        setError('Unable to connect to the server. Using demo mode.');
        setLoading(false);
      }
    };
    
    fetchUserContext();
  }, [user]);

  const menuItems = [
    { label: t('menu.dashboard'), translationKey: 'menu.dashboard', path: '/buyer', icon: <DashboardIcon /> },
    { label: t('menu.transactionHistory'), translationKey: 'menu.transactionHistory', path: '/buyer/transactions', icon: <HistoryIcon /> },
    { label: t('menu.qrCode'), translationKey: 'menu.qrCode', path: '/buyer/qrcode', icon: <QrCodeIcon /> },
    { label: t('menu.findStores'), translationKey: 'menu.findStores', path: '/buyer/stores', icon: <StoreIcon /> },
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <DashboardLayout title={t('dashboard.buyer')} menuItems={menuItems}>
      {error && (
        <Paper sx={{ p: 2, mb: 3, bgcolor: 'warning.light' }}>
          <Typography color="warning.dark">{error}</Typography>
        </Paper>
      )}
      <Routes>
        <Route path="/" element={<BuyerHome userContext={userContext} />} />
        <Route path="/transactions" element={<BuyerTransactions />} />
        <Route path="/qrcode" element={<BuyerQrCode />} />
        <Route path="/stores" element={<BuyerStores />} />
        <Route path="*" element={<Navigate to="/buyer" replace />} />
      </Routes>
    </DashboardLayout>
  );
};

export default BuyerDashboard;
