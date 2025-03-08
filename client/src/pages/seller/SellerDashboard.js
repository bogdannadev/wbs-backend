import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { 
  Dashboard as DashboardIcon, 
  History as HistoryIcon, 
  Store as StoreIcon,
  ShoppingCart as ShoppingCartIcon
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import SellerHome from './SellerHome';
import SellerTransactions from './SellerTransactions';
import SellerTransactionForm from './SellerTransactionForm';
import SellerStores from './SellerStores';
import { sellerService, isDemoMode } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';
import { Paper, Typography, Box, CircularProgress } from '@mui/material';
import { useTranslation } from 'react-i18next';

const SellerDashboard = () => {
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
          const selectedStore = user?.selectedStore || 
                                JSON.parse(localStorage.getItem('selectedStore') || 'null') || 
                                { id: 2, name: 'Demo Store', company: 'Demo Company' };
          
          setUserContext({
            username: user?.username || localStorage.getItem('username') || 'Demo Seller',
            role: 'Seller',
            selectedStore
          });
          setLoading(false);
          return;
        }
        
        // Try the real API
        const response = await sellerService.getContext();
        setUserContext(response.data.context);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching user context:', error);
        // Fall back to demo data even if not explicitly in demo mode
        const selectedStore = user?.selectedStore || 
                            JSON.parse(localStorage.getItem('selectedStore') || 'null') || 
                            { id: 2, name: 'Demo Store', company: 'Demo Company' };
        
        setUserContext({
          username: user?.username || localStorage.getItem('username') || 'Demo Seller',
          role: 'Seller',
          selectedStore
        });
        setError('Unable to connect to the server. Using demo mode.');
        setLoading(false);
      }
    };
    
    fetchUserContext();
  }, [user]);

  const menuItems = [
    { label: t('menu.dashboard'), translationKey: 'menu.dashboard', path: '/seller', icon: <DashboardIcon /> },
    { label: t('menu.newTransaction'), translationKey: 'menu.newTransaction', path: '/seller/transaction', icon: <ShoppingCartIcon /> },
    { label: t('menu.transactionHistory'), translationKey: 'menu.transactionHistory', path: '/seller/transactions', icon: <HistoryIcon /> },
    { label: t('menu.storeSelection'), translationKey: 'menu.storeSelection', path: '/seller/stores', icon: <StoreIcon /> }
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <DashboardLayout title={t('dashboard.seller')} menuItems={menuItems}>
      {error && (
        <Paper sx={{ p: 2, mb: 3, bgcolor: 'warning.light' }}>
          <Typography color="warning.dark">{error}</Typography>
        </Paper>
      )}
      <Routes>
        <Route path="/" element={<SellerHome userContext={userContext} />} />
        <Route path="/transaction" element={<SellerTransactionForm />} />
        <Route path="/transactions" element={<SellerTransactions />} />
        <Route path="/stores" element={<SellerStores />} />
        <Route path="*" element={<Navigate to="/seller" replace />} />
      </Routes>
    </DashboardLayout>
  );
};

export default SellerDashboard;
