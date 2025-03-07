import React, { useState, useEffect } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
import { 
  Dashboard as DashboardIcon, 
  QrCodeScanner as QrCodeScannerIcon, 
  History as HistoryIcon, 
  AccountBalance as AccountBalanceIcon 
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import SellerHome from './SellerHome';
import SellerTransactionForm from './SellerTransactionForm';
import SellerTransactions from './SellerTransactions';
import SellerBalances from './SellerBalances';
import { sellerService } from '../../services/api';
import { CircularProgress, Box, Paper, Typography } from '@mui/material';

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
    
    fetchUserContext();
  }, []);

  const menuItems = [
    { label: 'Dashboard', path: '/seller', icon: <DashboardIcon /> },
    { label: 'Process Transaction', path: '/seller/transaction', icon: <QrCodeScannerIcon /> },
    { label: 'Transaction History', path: '/seller/transactions', icon: <HistoryIcon /> },
    { label: 'Store Balances', path: '/seller/balances', icon: <AccountBalanceIcon /> },
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

  // For the demo, let's create a mock user context if API call fails
  const demoUserContext = userContext || {
    userId: '22222222-2222-2222-2222-222222222222',
    username: 'Seller Demo',
    role: 'Seller'
  };

  return (
    <DashboardLayout title="Seller Dashboard" menuItems={menuItems}>
      <Routes>
        <Route path="/" element={<SellerHome userContext={demoUserContext} />} />
        <Route path="/transaction" element={<SellerTransactionForm />} />
        <Route path="/transactions" element={<SellerTransactions />} />
        <Route path="/balances" element={<SellerBalances />} />
      </Routes>
    </DashboardLayout>
  );
};

export default SellerDashboard;
