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
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  Button,
  CircularProgress
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  Business as BusinessIcon,
  Store as StoreIcon,
  Notifications as NotificationsIcon,
  BarChart as BarChartIcon,
  People as PeopleIcon
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';
import AdminCompanies from './AdminCompanies';
import AdminUsers from './AdminUsers';
import { adminService } from '../../services/api';

// ... [rest of the existing code remains the same]

// Main Admin Dashboard component
const AdminDashboard = () => {
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
            username: 'Admin User',
            role: 'SystemAdmin'
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
    { label: 'Dashboard', path: '/admin', icon: <DashboardIcon /> },
    { label: 'Companies', path: '/admin/companies', icon: <BusinessIcon /> },
    { label: 'Stores', path: '/admin/stores', icon: <StoreIcon /> },
    { label: 'Users', path: '/admin/users', icon: <PeopleIcon /> },
    { label: 'Notifications', path: '/admin/notifications', icon: <NotificationsIcon /> },
    { label: 'Reports', path: '/admin/reports', icon: <BarChartIcon /> },
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
    <DashboardLayout title="Admin Dashboard" menuItems={menuItems}>
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
