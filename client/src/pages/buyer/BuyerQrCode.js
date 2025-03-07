import React, { useState, useEffect } from 'react';
import { 
  Paper, 
  Typography, 
  Box, 
  Card, 
  CardContent, 
  CircularProgress, 
  Button, 
  Snackbar, 
  Alert,
  Divider,
  useTheme
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import { QRCodeCanvas } from 'qrcode.react';
import { buyerService } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';

const BuyerQrCode = () => {
  const [qrCode, setQrCode] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [refreshing, setRefreshing] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const { user } = useAuth();
  const theme = useTheme();

  const fetchQrCode = async () => {
    try {
      setLoading(true);
      setError('');
      const response = await buyerService.generateQrCode();
      setQrCode(response.data.qrCode);
      setLoading(false);
    } catch (error) {
      setError('Failed to generate QR code. Please try again.');
      setLoading(false);
      console.error('Error generating QR code:', error);
    }
  };

  useEffect(() => {
    fetchQrCode();
  }, []);

  const handleRefresh = async () => {
    setRefreshing(true);
    await fetchQrCode();
    setRefreshing(false);
    setSuccessMessage('QR code refreshed successfully');
  };

  const handleCloseSnackbar = () => {
    setSuccessMessage('');
    setError('');
  };

  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Your Bonus QR Code
        </Typography>
        <Typography variant="body1">
          Present this QR code to sellers to earn or redeem bonus points during transactions.
        </Typography>
      </Paper>

      <Card>
        <CardContent>
          <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center', p: 2 }}>
            <Typography variant="h6" gutterBottom>
              Your Unique QR Code
            </Typography>
            <Divider sx={{ width: '100%', mb: 3 }} />
            
            {loading ? (
              <CircularProgress sx={{ my: 5 }} />
            ) : (
              <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                <Box 
                  sx={{ 
                    bgcolor: 'white', 
                    p: 3, 
                    borderRadius: 2, 
                    border: `1px solid ${theme.palette.divider}`,
                    boxShadow: 1,
                    mb: 2
                  }}
                >
                  <QRCodeCanvas 
                    value={qrCode || `BONUS-USER-${user?.id || 'DEMO'}`} 
                    size={250}
                    level="H"
                    includeMargin={true}
                  />
                </Box>
                
                <Typography variant="body2" color="text.secondary" sx={{ mb: 3, textAlign: 'center' }}>
                  Seller will scan this code to process bonus points <br />
                  Code ID: {qrCode ? qrCode.substring(qrCode.length - 8) : 'Loading...'}
                </Typography>
                
                <Button 
                  variant="outlined" 
                  color="primary" 
                  startIcon={<RefreshIcon />}
                  onClick={handleRefresh}
                  disabled={refreshing}
                >
                  {refreshing ? 'Refreshing...' : 'Refresh QR Code'}
                </Button>
              </Box>
            )}
          </Box>
        </CardContent>
      </Card>

      <Card sx={{ mt: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            How to Use
          </Typography>
          <Divider sx={{ mb: 2 }} />
          <Typography variant="body1" paragraph>
            1. When making a purchase, show this QR code to the seller.
          </Typography>
          <Typography variant="body1" paragraph>
            2. The seller will scan your code and enter the transaction details.
          </Typography>
          <Typography variant="body1" paragraph>
            3. You can earn points on purchases or redeem points for discounts.
          </Typography>
          <Typography variant="body1">
            4. Your transaction will appear in your history once completed.
          </Typography>
        </CardContent>
      </Card>

      <Snackbar 
        open={!!successMessage} 
        autoHideDuration={4000} 
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleCloseSnackbar} severity="success" sx={{ width: '100%' }}>
          {successMessage}
        </Alert>
      </Snackbar>

      <Snackbar 
        open={!!error} 
        autoHideDuration={4000} 
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleCloseSnackbar} severity="error" sx={{ width: '100%' }}>
          {error}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default BuyerQrCode;
