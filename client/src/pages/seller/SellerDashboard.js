import React, { useState, useEffect } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
import { 
  Dashboard as DashboardIcon, 
  History as HistoryIcon, 
  QrCode as QrCodeIcon, 
  Store as StoreIcon,
  ShoppingCart as ShoppingCartIcon
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import SellerHome from './SellerHome';
import SellerTransactions from './SellerTransactions';
import SellerTransactionForm from './SellerTransactionForm';
import SellerStores from './SellerStores';
import { sellerService } from '../../services/api';
import { Paper, Typography, Box, CircularProgress } from '@mui/material';

const SellerDashboard = () => {
  const [userContext, setUserContext] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUserContext = async () => {
      try {
        setLoading(true);
        const response = await sellerService.getContext();
        setUserContext(response.data.context);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching user context:', error);
        setError('Failed to load user data. Please try again later.');
        setLoading(false);
      }
    };
    
    // In a real implementation, we would call the API
    // However, for the demo, we'll mock the response
    setTimeout(() => {
      setUserContext({
        username: 'Sarah Seller',
        role: 'Seller',
        selectedStore: {
          id: 2,
          name: 'Global Retail - Uptown',
          company: 'Global Retail Inc.'
        }
      });
      setLoading(false);
    }, 1000);
  }, []);

  const menuItems = [
    { label: 'Dashboard', path: '/seller', icon: <DashboardIcon /> },
    { label: 'New Transaction', path: '/seller/transaction', icon: <ShoppingCartIcon /> },
    { label: 'Transaction History', path: '/seller/transactions', icon: <HistoryIcon /> },
    { label: 'Store Selection', path: '/seller/stores', icon: <StoreIcon /> }
  ];

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
        <Paper sx={{ p: 3, maxWidth: 500 }}>
          <Typography variant="h6" color="error" gutterBottom>
            Error
          </Typography>
          <Typography>{error}</Typography>
        </Paper>
      </Box>
    );
  }

  return (
    <DashboardLayout title="Seller Dashboard" menuItems={menuItems}>
      <Routes>
        <Route path="/" element={<SellerHome userContext={userContext} />} />
        <Route path="/transaction" element={<SellerTransactionForm />} />
        <Route path="/transactions" element={<SellerTransactions />} />
        <Route path="/stores" element={<SellerStores />} />
      </Routes>
    </DashboardLayout>
  );
};

export default SellerDashboard;
