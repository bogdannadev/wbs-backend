import React from 'react';
import { 
  Paper, 
  Typography, 
  Box, 
  Card, 
  CardContent, 
  Button, 
  TextField, 
  Grid, 
  Divider,
  Alert,
  AlertTitle
} from '@mui/material';
import {
  QrCodeScanner,
  PointOfSale,
  ReceiptLong,
  AccountBalance
} from '@mui/icons-material';
import DashboardLayout from '../../components/common/DashboardLayout';

const SellerDashboard = () => {
  const menuItems = [
    { label: 'Dashboard', path: '/seller', icon: <PointOfSale /> },
    { label: 'Scan', path: '/seller/scan', icon: <QrCodeScanner /> },
    { label: 'Transactions', path: '/seller/transactions', icon: <ReceiptLong /> },
    { label: 'Store Balances', path: '/seller/balances', icon: <AccountBalance /> },
  ];

  return (
    <DashboardLayout title="Seller Dashboard" menuItems={menuItems}>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Welcome to Seller Dashboard
        </Typography>
        <Typography variant="body1">
          Process bonus transactions for customers by scanning their QR codes.
        </Typography>
      </Paper>

      <Alert severity="info" sx={{ mb: 3 }}>
        <AlertTitle>Demo Version</AlertTitle>
        This is a simplified seller interface for demonstration purposes. Full functionality will be available in the production version.
      </Alert>

      <Grid container spacing={3}>
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Process Transaction
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box component="form" sx={{ mt: 2 }}>
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  label="Customer QR Code"
                  placeholder="Scan or enter customer code"
                  variant="outlined"
                />
                
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  label="Transaction Amount"
                  type="number"
                  InputProps={{
                    startAdornment: <Typography sx={{ mr: 1 }}>$</Typography>,
                  }}
                  variant="outlined"
                />
                
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  label="Bonus Points"
                  type="number"
                  variant="outlined"
                  helperText="Points to earn or redeem (negative for redemption)"
                />
                
                <Grid container spacing={2} sx={{ mt: 1 }}>
                  <Grid item xs={6}>
                    <Button
                      fullWidth
                      variant="contained"
                      color="primary"
                    >
                      Process Sale
                    </Button>
                  </Grid>
                  <Grid item xs={6}>
                    <Button
                      fullWidth
                      variant="outlined"
                      color="secondary"
                    >
                      Process Return
                    </Button>
                  </Grid>
                </Grid>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={6}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Recent Transactions
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box sx={{ p: 2, textAlign: 'center' }}>
                <Typography variant="body1" color="text.secondary">
                  Process a transaction to see it appear here.
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </DashboardLayout>
  );
};

export default SellerDashboard;
