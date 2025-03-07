import React, { useState, useEffect } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
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
import { buyerService } from '../../services/api';
import { Paper, Typography, Box, CircularProgress } from '@mui/material';

const BuyerDashboard = () => {
  const [userContext, setUserContext] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchUserContext = async () => {
      try {
        setLoading(true);
        const response = await buyerService.getContext();
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
    { label: 'Dashboard', path: '/buyer', icon: <DashboardIcon /> },
    { label: 'Transactions', path: '/buyer/transactions', icon: <HistoryIcon /> },
    { label: 'QR Code', path: '/buyer/qrcode', icon: <QrCodeIcon /> },
    { label: 'Find Stores', path: '/buyer/stores', icon: <StoreIcon /> },
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
    <DashboardLayout title="Buyer Dashboard" menuItems={menuItems}>
      <Routes>
        <Route path="/" element={<BuyerHome userContext={userContext} />} />
        <Route path="/transactions" element={<BuyerTransactions />} />
        <Route path="/qrcode" element={<BuyerQrCode />} />
        <Route path="/stores" element={<BuyerStores />} />
      </Routes>
    </DashboardLayout>
  );
};

export default BuyerDashboard;
