import React, { useState, useEffect } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { 
  Paper, 
  Typography, 
  Box, 
  Card, 
  CardContent, 
  Grid, 
  Divider,
  Alert,
  AlertTitle,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  Business as BusinessIcon,
  TrendingUp as TrendingUpIcon,
  Analytics as AnalyticsIcon
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import ObserverCompanies from './ObserverCompanies';
import { 
  BarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  Legend, 
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell
} from 'recharts';
import { observerService } from '../../services/api';

// [Rest of the existing code remains the same]

// Main Observer Dashboard component
const ObserverDashboard = () => {
  const [userContext, setUserContext] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUserContext = async () => {
      try {
        setLoading(true);
        // In a real implementation, this would call the API
        // For demo, we're just simulating the response
        setTimeout(() => {
          setUserContext({
            username: 'Observer User',
            role: 'CompanyObserver'
          });
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching user context:', error);
        setError('Failed to load user data. Please try again later.');
        setLoading(false);
      }
    };
    
    fetchUserContext();
  }, []);

  const menuItems = [
    { label: 'Dashboard', path: '/observer', icon: <DashboardIcon /> },
    { label: 'Companies', path: '/observer/companies', icon: <BusinessIcon /> },
    { label: 'Transactions', path: '/observer/transactions', icon: <TrendingUpIcon /> },
    { label: 'Analytics', path: '/observer/analytics', icon: <AnalyticsIcon /> },
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
    <DashboardLayout title="Observer Dashboard" menuItems={menuItems}>
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
