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
import { adminService } from '../../services/api';

// Home component for the Admin dashboard
const AdminHome = () => {
  const [loading, setLoading] = useState(false);

  // Mock data for companies
  const companies = [
    { id: 1, name: 'Global Retail Inc.', status: 'Active', balance: 250000, storeCount: 12 },
    { id: 2, name: 'Metro Grocery Chain', status: 'Active', balance: 180000, storeCount: 8 },
    { id: 3, name: 'Fashion Outlets Ltd', status: 'Pending', balance: 0, storeCount: 3 },
    { id: 4, name: 'Tech Gadgets Corp', status: 'Suspended', balance: 45000, storeCount: 5 },
  ];

  // Mock data for stores pending approval
  const pendingStores = [
    { id: 1, name: 'Metro Grocery - Westside', company: 'Metro Grocery Chain', location: 'Westside Mall' },
    { id: 2, name: 'Fashion Outlet - Downtown', company: 'Fashion Outlets Ltd', location: 'Downtown' },
    { id: 3, name: 'Tech Gadgets - Central', company: 'Tech Gadgets Corp', location: 'Central Square' },
  ];

  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          System Administration
        </Typography>
        <Typography variant="body1">
          Manage companies, stores, and bonus system settings.
        </Typography>
      </Paper>

      <Alert severity="info" sx={{ mb: 3 }}>
        <AlertTitle>Demo Version</AlertTitle>
        This is a simplified admin interface for demonstration purposes. Full administrative functionality will be available in the production version.
      </Alert>

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
                    <Typography variant="h4" color="primary">4</Typography>
                    <Typography variant="body2" color="text.secondary">Companies</Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">28</Typography>
                    <Typography variant="body2" color="text.secondary">Stores</Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">150</Typography>
                    <Typography variant="body2" color="text.secondary">Users</Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">475,000</Typography>
                    <Typography variant="body2" color="text.secondary">Total Bonus Points</Typography>
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>

        {/* Companies */}
        <Grid item xs={12} md={7}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Companies
              </Typography>
              <Divider sx={{ mb: 2 }} />

              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Name</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell align="right">Balance</TableCell>
                      <TableCell align="right">Stores</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {companies.map((company) => (
                      <TableRow key={company.id} hover>
                        <TableCell>{company.name}</TableCell>
                        <TableCell>
                          <Chip 
                            label={company.status} 
                            color={
                              company.status === 'Active' ? 'success' : 
                              company.status === 'Pending' ? 'warning' : 'error'
                            }
                            size="small"
                          />
                        </TableCell>
                        <TableCell align="right">{company.balance.toLocaleString()}</TableCell>
                        <TableCell align="right">{company.storeCount}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
              
              <Box sx={{ mt: 2, textAlign: 'right' }}>
                <Button variant="outlined" size="small" href="/admin/companies">View All Companies</Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Pending Approvals */}
        <Grid item xs={12} md={5}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Pending Store Approvals
              </Typography>
              <Divider sx={{ mb: 2 }} />

              <TableContainer>
                <Table size="small">
                  <TableHead>
                    <TableRow>
                      <TableCell>Store Name</TableCell>
                      <TableCell>Company</TableCell>
                      <TableCell>Action</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {pendingStores.map((store) => (
                      <TableRow key={store.id} hover>
                        <TableCell>{store.name}</TableCell>
                        <TableCell>{store.company}</TableCell>
                        <TableCell>
                          <Button 
                            size="small" 
                            variant="outlined" 
                            color="success" 
                            sx={{ mr: 1 }}
                          >
                            Approve
                          </Button>
                          <Button 
                            size="small" 
                            variant="outlined" 
                            color="error"
                          >
                            Reject
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
              
              <Box sx={{ mt: 2, textAlign: 'right' }}>
                <Button variant="outlined" size="small" href="/admin/stores">View All Pending</Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
};

// Placeholder components for future implementation
const AdminStores = () => (
  <Paper sx={{ p: 3 }}>
    <Typography variant="h5" gutterBottom>Store Management</Typography>
    <Typography>This feature will be available in the complete version.</Typography>
  </Paper>
);

const AdminUsers = () => (
  <Paper sx={{ p: 3 }}>
    <Typography variant="h5" gutterBottom>User Management</Typography>
    <Typography>This feature will be available in the complete version.</Typography>
  </Paper>
);

const AdminNotifications = () => (
  <Paper sx={{ p: 3 }}>
    <Typography variant="h5" gutterBottom>Notification Management</Typography>
    <Typography>This feature will be available in the complete version.</Typography>
  </Paper>
);

const AdminReports = () => (
  <Paper sx={{ p: 3 }}>
    <Typography variant="h5" gutterBottom>System Reports</Typography>
    <Typography>This feature will be available in the complete version.</Typography>
  </Paper>
);

// Main Admin Dashboard component
const AdminDashboard = () => {
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
      </Routes>
    </DashboardLayout>
  );
};

export default AdminDashboard;
