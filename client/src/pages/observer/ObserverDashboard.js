import React, { useState, useEffect } from 'react';
import { Routes, Route, useNavigate } from 'react-router-dom';
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

// Observer Home component
const ObserverHome = () => {
  // Mock data for charts
  const transactionData = [
    { month: 'Jan', earned: 4000, spent: 2400 },
    { month: 'Feb', earned: 3000, spent: 1398 },
    { month: 'Mar', earned: 2000, spent: 9800 },
    { month: 'Apr', earned: 2780, spent: 3908 },
    { month: 'May', earned: 1890, spent: 4800 },
    { month: 'Jun', earned: 2390, spent: 3800 },
  ];

  const companyDistributionData = [
    { name: 'Global Retail Inc.', value: 250000 },
    { name: 'Metro Grocery Chain', value: 180000 },
    { name: 'Fashion Outlets Ltd', value: 120000 },
    { name: 'Tech Gadgets Corp', value: 45000 },
  ];

  const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042'];

  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          System Analytics
        </Typography>
        <Typography variant="body1">
          View transaction statistics and bonus distribution across the system.
        </Typography>
      </Paper>

      <Alert severity="info" sx={{ mb: 3 }}>
        <AlertTitle>Demo Version</AlertTitle>
        This is a simplified observer interface for demonstration purposes, showing sample data. Full analytical functionality will be available in the production version.
      </Alert>

      <Box sx={{ mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} md={6}>
            <FormControl fullWidth size="small">
              <InputLabel id="time-period-label">Time Period</InputLabel>
              <Select
                labelId="time-period-label"
                id="time-period"
                value="last6months"
                label="Time Period"
              >
                <MenuItem value="last30days">Last 30 Days</MenuItem>
                <MenuItem value="last3months">Last 3 Months</MenuItem>
                <MenuItem value="last6months">Last 6 Months</MenuItem>
                <MenuItem value="lastyear">Last Year</MenuItem>
                <MenuItem value="alltime">All Time</MenuItem>
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={6}>
            <FormControl fullWidth size="small">
              <InputLabel id="company-filter-label">Company</InputLabel>
              <Select
                labelId="company-filter-label"
                id="company-filter"
                value="all"
                label="Company"
              >
                <MenuItem value="all">All Companies</MenuItem>
                <MenuItem value="global">Global Retail Inc.</MenuItem>
                <MenuItem value="metro">Metro Grocery Chain</MenuItem>
                <MenuItem value="fashion">Fashion Outlets Ltd</MenuItem>
                <MenuItem value="tech">Tech Gadgets Corp</MenuItem>
              </Select>
            </FormControl>
          </Grid>
        </Grid>
      </Box>

      <Grid container spacing={3}>
        {/* System Overview */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                System Overview
              </Typography>
              <Divider sx={{ mb: 2 }} />

              <Grid container spacing={3}>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">595,000</Typography>
                    <Typography variant="body2" color="text.secondary">Total Bonus Points</Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">412,450</Typography>
                    <Typography variant="body2" color="text.secondary">Points Earned</Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">182,550</Typography>
                    <Typography variant="body2" color="text.secondary">Points Spent</Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">2,456</Typography>
                    <Typography variant="body2" color="text.secondary">Total Transactions</Typography>
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>

        {/* Transaction Chart */}
        <Grid item xs={12} md={8}>
          <Card sx={{ height: '100%' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Bonus Points Transaction History
              </Typography>
              <Divider sx={{ mb: 2 }} />

              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={transactionData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="month" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="earned" name="Points Earned" fill="#8884d8" />
                    <Bar dataKey="spent" name="Points Spent" fill="#82ca9d" />
                  </BarChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Distribution Chart */}
        <Grid item xs={12} md={4}>
          <Card sx={{ height: '100%' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Bonus Points Distribution
              </Typography>
              <Divider sx={{ mb: 2 }} />

              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={companyDistributionData}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                    >
                      {companyDistributionData.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value) => `${value.toLocaleString()} points`} />
                  </PieChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

// Placeholder components for future implementation
const ObserverTransactions = () => (
  <Paper sx={{ p: 3 }}>
    <Typography variant="h5" gutterBottom>Transaction Analytics</Typography>
    <Typography>This feature will be available in the complete version.</Typography>
  </Paper>
);

const ObserverAnalytics = () => (
  <Paper sx={{ p: 3 }}>
    <Typography variant="h5" gutterBottom>Advanced Analytics</Typography>
    <Typography>This feature will be available in the complete version.</Typography>
  </Paper>
);

// Main Observer Dashboard component
const ObserverDashboard = () => {
  const [userContext, setUserContext] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

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
      </Routes>
    </DashboardLayout>
  );
};

export default ObserverDashboard;
