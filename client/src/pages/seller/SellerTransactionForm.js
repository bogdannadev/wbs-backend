import React, { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Divider,
  Grid,
  FormControl,
  FormControlLabel,
  Radio,
  RadioGroup,
  Alert,
  Snackbar,
  CircularProgress,
  Paper,
  Chip
} from '@mui/material';
import {
  QrCodeScanner as QrCodeScannerIcon,
  CreditCard as CreditCardIcon,
  CheckCircle as CheckCircleIcon
} from '@mui/icons-material';
import { sellerService } from '../../services/api';

const SellerTransactionForm = () => {
  const [buyerCode, setBuyerCode] = useState('');
  const [transactionAmount, setTransactionAmount] = useState('');
  const [bonusPoints, setBonusPoints] = useState('');
  const [transactionType, setTransactionType] = useState('earn');
  const [loading, setLoading] = useState(false);
  const [scanningQrCode, setScanningQrCode] = useState(false);
  const [buyerInfo, setBuyerInfo] = useState(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loadingBuyerInfo, setLoadingBuyerInfo] = useState(false);

  const handleStartScanning = () => {
    setScanningQrCode(true);
    // In a real implementation, this would activate the device camera
    // For the demo, we'll just simulate it
    setTimeout(() => {
      setBuyerCode('BONUS-USER-11111111-1111-1111-1111-111111111111');
      setScanningQrCode(false);
      fetchBuyerInfo('11111111-1111-1111-1111-111111111111');
    }, 2000);
  };

  const fetchBuyerInfo = async (buyerId) => {
    setLoadingBuyerInfo(true);
    try {
      // In a real implementation, we would parse the buyerId from the QR code
      // For the demo, we're using a hardcoded ID
      const response = await sellerService.getBuyerBalance(buyerId);
      setBuyerInfo({
        id: buyerId,
        bonusBalance: response.data,
        name: 'John Doe' // This would come from the API in a real implementation
      });
    } catch (error) {
      console.error('Error fetching buyer info:', error);
      setError('Failed to fetch buyer information. Please try again.');
    } finally {
      setLoadingBuyerInfo(false);
    }
  };

  const calculateMaxPoints = () => {
    if (!transactionAmount || !buyerInfo) return 0;
    
    const amount = parseFloat(transactionAmount);
    // For demo purposes, let's say 1 point for every $10 spent
    return Math.floor(amount / 10);
  };

  const calculateMaxRedemption = () => {
    if (!buyerInfo) return 0;
    return buyerInfo.bonusBalance;
  };

  const handleTransactionSubmit = async () => {
    if (!buyerCode || !transactionAmount || !bonusPoints) {
      setError('Please fill in all required fields');
      return;
    }

    setLoading(true);
    setError('');

    try {
      // Extract buyer ID from QR code
      const buyerId = buyerCode.split('-').slice(2).join('-');
      
      // Create transaction request
      const transaction = {
        buyerId,
        amount: parseFloat(transactionAmount),
        type: transactionType,
        bonusPoints: transactionType === 'earn' ? 
          parseFloat(bonusPoints) : 
          -parseFloat(bonusPoints), // Negative for redemption
        storeId: '12345678-1234-1234-1234-123456789012' // Hardcoded for demo
      };

      // Submit transaction
      const response = await sellerService.processTransaction(transaction);
      
      // Handle success
      setSuccess('Transaction processed successfully!');
      
      // Reset form
      setBuyerCode('');
      setTransactionAmount('');
      setBonusPoints('');
      setBuyerInfo(null);
      
    } catch (error) {
      console.error('Error processing transaction:', error);
      setError('Failed to process transaction. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  const handleSnackbarClose = (event, reason) => {
    if (reason === 'clickaway') {
      return;
    }
    setError('');
    setSuccess('');
  };

  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Process Transaction
        </Typography>
        <Typography variant="body1">
          Scan customer QR code and process bonus transactions.
        </Typography>
      </Paper>

      <Card>
        <CardContent>
          <Grid container spacing={3}>
            {/* Step 1: Scan QR Code */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom>
                Step 1: Identify Customer
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box sx={{ mb: 3 }}>
                <TextField
                  fullWidth
                  label="Customer QR Code"
                  variant="outlined"
                  value={buyerCode}
                  onChange={(e) => setBuyerCode(e.target.value)}
                  placeholder="Scan or enter customer code"
                  disabled={scanningQrCode || loadingBuyerInfo}
                  sx={{ mb: 2 }}
                />
                
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Button
                    variant="outlined"
                    startIcon={<QrCodeScannerIcon />}
                    onClick={handleStartScanning}
                    disabled={scanningQrCode || loadingBuyerInfo}
                  >
                    {scanningQrCode ? 'Scanning...' : 'Scan QR Code'}
                  </Button>
                  
                  {buyerInfo && (
                    <Chip 
                      icon={<CheckCircleIcon />} 
                      color="success" 
                      label={`${buyerInfo.name}: ${buyerInfo.bonusBalance} bonus points available`} 
                    />
                  )}
                </Box>
              </Box>
            </Grid>
            
            {/* Step 2: Transaction Details */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom>
                Step 2: Transaction Details
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box sx={{ mb: 3 }}>
                <TextField
                  fullWidth
                  label="Transaction Amount"
                  type="number"
                  variant="outlined"
                  value={transactionAmount}
                  onChange={(e) => setTransactionAmount(e.target.value)}
                  disabled={!buyerInfo || loading}
                  InputProps={{
                    startAdornment: <Typography sx={{ mr: 1 }}>$</Typography>,
                  }}
                  sx={{ mb: 2 }}
                />
                
                <FormControl component="fieldset" sx={{ mb: 2 }}>
                  <RadioGroup
                    row
                    name="transaction-type"
                    value={transactionType}
                    onChange={(e) => setTransactionType(e.target.value)}
                  >
                    <FormControlLabel value="earn" control={<Radio />} label="Earn Points" />
                    <FormControlLabel value="spend" control={<Radio />} label="Redeem Points" />
                  </RadioGroup>
                </FormControl>
                
                <TextField
                  fullWidth
                  label="Bonus Points"
                  type="number"
                  variant="outlined"
                  value={bonusPoints}
                  onChange={(e) => setBonusPoints(e.target.value)}
                  disabled={!buyerInfo || !transactionAmount || loading}
                  helperText={
                    transactionType === 'earn' 
                      ? `Maximum points to award: ${calculateMaxPoints()}` 
                      : `Maximum points to redeem: ${calculateMaxRedemption()}`
                  }
                />
              </Box>
            </Grid>
            
            {/* Step 3: Submit */}
            <Grid item xs={12}>
              <Typography variant="h6" gutterBottom>
                Step 3: Process Transaction
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box sx={{ display: 'flex', justifyContent: 'center' }}>
                <Button
                  variant="contained"
                  size="large"
                  startIcon={<CreditCardIcon />}
                  onClick={handleTransactionSubmit}
                  disabled={!buyerInfo || !transactionAmount || !bonusPoints || loading}
                  sx={{ minWidth: 200 }}
                >
                  {loading ? (
                    <CircularProgress size={24} color="inherit" />
                  ) : (
                    'Process Transaction'
                  )}
                </Button>
              </Box>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      <Snackbar
        open={!!error}
        autoHideDuration={6000}
        onClose={handleSnackbarClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleSnackbarClose} severity="error">
          {error}
        </Alert>
      </Snackbar>

      <Snackbar
        open={!!success}
        autoHideDuration={6000}
        onClose={handleSnackbarClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleSnackbarClose} severity="success">
          {success}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default SellerTransactionForm;
