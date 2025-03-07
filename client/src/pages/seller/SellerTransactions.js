import React, { useState, useEffect } from 'react';
import {
  Paper,
  Typography,
  Box,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  Chip,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Snackbar,
  Alert,
  CircularProgress,
  Grid
} from '@mui/material';
import {
  ArrowUpward as EarnedIcon,
  ArrowDownward as SpentIcon,
  Undo as ReturnIcon,
  WatchLater as PendingIcon,
  CheckCircle as CompletedIcon,
  Cancel as RejectedIcon
} from '@mui/icons-material';
import { sellerService } from '../../services/api';

const SellerTransactions = () => {
  const [transactions, setTransactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [returnDialogOpen, setReturnDialogOpen] = useState(false);
  const [transactionToReturn, setTransactionToReturn] = useState(null);
  const [processingReturn, setProcessingReturn] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const [selectedStore, setSelectedStore] = useState('');
  const [stores, setStores] = useState([
    { id: 'store1', name: 'Downtown Store' },
    { id: 'store2', name: 'Mall Location' },
    { id: 'store3', name: 'East Side Store' },
  ]);

  useEffect(() => {
    if (selectedStore) {
      fetchTransactions();
    }
  }, [selectedStore]);

  const fetchTransactions = async () => {
    try {
      setLoading(true);
      const response = await sellerService.getTransactions(selectedStore);
      
      // For demo purposes, let's create some mock data
      const mockTransactions = [
        {
          id: "tx1",
          buyerId: "11111111-1111-1111-1111-111111111111",
          buyerName: "John Doe",
          amount: 120.50,
          bonusPoints: 12,
          type: "Earn",
          timestamp: new Date(Date.now() - 1000 * 60 * 30).toISOString(), // 30 minutes ago
          status: "Completed",
          description: "Purchase of electronics"
        },
        {
          id: "tx2",
          buyerId: "11111111-1111-1111-1111-111111111111",
          buyerName: "John Doe",
          amount: 50.25,
          bonusPoints: 5,
          type: "Spend",
          timestamp: new Date(Date.now() - 1000 * 60 * 60).toISOString(), // 1 hour ago
          status: "Completed",
          description: "Redemption for discount"
        },
        {
          id: "tx3",
          buyerId: "22222222-2222-2222-2222-222222222222",
          buyerName: "Jane Smith",
          amount: 75.00,
          bonusPoints: 7,
          type: "Earn",
          timestamp: new Date(Date.now() - 1000 * 60 * 90).toISOString(), // 1.5 hours ago
          status: "Pending",
          description: "Clothing purchase"
        }
      ];
      
      setTransactions(mockTransactions);
      setLoading(false);
    } catch (error) {
      console.error('Error fetching transactions:', error);
      setError('Failed to load transactions. Please try again later.');
      setLoading(false);
    }
  };

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleOpenReturnDialog = (transaction) => {
    setTransactionToReturn(transaction);
    setReturnDialogOpen(true);
  };

  const handleCloseReturnDialog = () => {
    setReturnDialogOpen(false);
    setTransactionToReturn(null);
  };

  const handleProcessReturn = async () => {
    if (!transactionToReturn) return;
    
    setProcessingReturn(true);
    
    try {
      await sellerService.confirmReturn(transactionToReturn.id);
      
      // Update the transaction status in the local state
      setTransactions(prevTransactions => 
        prevTransactions.map(transaction => 
          transaction.id === transactionToReturn.id 
            ? { ...transaction, status: 'Reversed' } 
            : transaction
        )
      );
      
      setSuccessMessage('Return processed successfully');
    } catch (error) {
      console.error('Error processing return:', error);
      setError('Failed to process return. Please try again later.');
    } finally {
      setProcessingReturn(false);
      handleCloseReturnDialog();
    }
  };

  const handleCloseSnackbar = () => {
    setSuccessMessage('');
    setError('');
  };

  // Function to get status chip style
  const getStatusChip = (status) => {
    switch (status) {
      case 'Completed':
        return <Chip icon={<CompletedIcon />} label="Completed" color="success" size="small" />;
      case 'Pending':
        return <Chip icon={<PendingIcon />} label="Pending" color="warning" size="small" />;
      case 'Reversed':
        return <Chip icon={<ReturnIcon />} label="Returned" color="error" size="small" />;
      case 'Failed':
        return <Chip icon={<RejectedIcon />} label="Failed" color="error" size="small" />;
      default:
        return <Chip label={status} size="small" />;
    }
  };

  // Function to check if transaction can be returned
  const canReturnTransaction = (transaction) => {
    // Check if transaction is recent (within 24 hours) and completed
    const isRecent = new Date(transaction.timestamp) > new Date(Date.now() - 24 * 60 * 60 * 1000);
    return isRecent && transaction.status === 'Completed';
  };

  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Transaction History
        </Typography>
        <Typography variant="body1">
          View and manage transaction history for your stores.
        </Typography>
      </Paper>

      <Grid container spacing={3} sx={{ mb: 3 }}>
        <Grid item xs={12} md={6}>
          <FormControl fullWidth>
            <InputLabel>Select Store</InputLabel>
            <Select
              value={selectedStore}
              label="Select Store"
              onChange={(e) => setSelectedStore(e.target.value)}
            >
              {stores.map((store) => (
                <MenuItem key={store.id} value={store.id}>
                  {store.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Grid>
      </Grid>

      <Paper sx={{ width: '100%', overflow: 'hidden' }}>
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : error ? (
          <Box sx={{ p: 3 }}>
            <Typography color="error">{error}</Typography>
          </Box>
        ) : !selectedStore ? (
          <Box sx={{ p: 3, textAlign: 'center' }}>
            <Typography>Please select a store to view transactions</Typography>
          </Box>
        ) : (
          <>
            <TableContainer sx={{ maxHeight: 440 }}>
              <Table stickyHeader aria-label="transactions table">
                <TableHead>
                  <TableRow>
                    <TableCell>Customer</TableCell>
                    <TableCell>Type</TableCell>
                    <TableCell>Date</TableCell>
                    <TableCell>Amount</TableCell>
                    <TableCell>Bonus Points</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {transactions.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={7} align="center">No transactions found</TableCell>
                    </TableRow>
                  ) : (
                    transactions
                      .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                      .map((transaction) => (
                        <TableRow hover key={transaction.id}>
                          <TableCell>{transaction.buyerName}</TableCell>
                          <TableCell>
                            {transaction.type === 'Earn' ? (
                              <Chip 
                                icon={<EarnedIcon />} 
                                label="Earned" 
                                color="primary" 
                                size="small"
                              />
                            ) : (
                              <Chip 
                                icon={<SpentIcon />} 
                                label="Spent" 
                                color="secondary" 
                                size="small"
                              />
                            )}
                          </TableCell>
                          <TableCell>
                            {new Date(transaction.timestamp).toLocaleString()}
                          </TableCell>
                          <TableCell>
                            ${transaction.amount.toFixed(2)}
                          </TableCell>
                          <TableCell sx={{ 
                            fontWeight: 'bold',
                            color: transaction.type === 'Earn' ? 'success.main' : 'error.main'
                          }}>
                            {transaction.type === 'Earn' ? '+' : '-'}{transaction.bonusPoints}
                          </TableCell>
                          <TableCell>{getStatusChip(transaction.status)}</TableCell>
                          <TableCell>
                            {canReturnTransaction(transaction) ? (
                              <Button 
                                size="small" 
                                variant="outlined" 
                                color="error"
                                startIcon={<ReturnIcon />}
                                onClick={() => handleOpenReturnDialog(transaction)}
                              >
                                Process Return
                              </Button>
                            ) : (
                              <Button 
                                size="small" 
                                variant="outlined" 
                                disabled
                              >
                                {transaction.status === 'Reversed' ? 'Returned' : 'Cannot Return'}
                              </Button>
                            )}
                          </TableCell>
                        </TableRow>
                      ))
                  )}
                </TableBody>
              </Table>
            </TableContainer>
            <TablePagination
              rowsPerPageOptions={[5, 10, 25]}
              component="div"
              count={transactions.length}
              rowsPerPage={rowsPerPage}
              page={page}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
            />
          </>
        )}
      </Paper>

      {/* Return Transaction Dialog */}
      <Dialog
        open={returnDialogOpen}
        onClose={handleCloseReturnDialog}
        aria-labelledby="return-transaction-dialog-title"
      >
        <DialogTitle id="return-transaction-dialog-title">
          Process Return
        </DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to process a return for this transaction? This will reverse the bonus points and mark the transaction as returned.
          </DialogContentText>
          {transactionToReturn && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2">
                <strong>Customer:</strong> {transactionToReturn.buyerName}
              </Typography>
              <Typography variant="body2">
                <strong>Amount:</strong> ${transactionToReturn.amount.toFixed(2)}
              </Typography>
              <Typography variant="body2">
                <strong>Bonus Points:</strong> {transactionToReturn.bonusPoints}
              </Typography>
              <Typography variant="body2">
                <strong>Date:</strong> {new Date(transactionToReturn.timestamp).toLocaleString()}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseReturnDialog} disabled={processingReturn}>
            Cancel
          </Button>
          <Button 
            onClick={handleProcessReturn} 
            color="error" 
            disabled={processingReturn}
            autoFocus
          >
            {processingReturn ? 'Processing...' : 'Confirm Return'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* Success Snackbar */}
      <Snackbar
        open={!!successMessage}
        autoHideDuration={6000}
        onClose={handleCloseSnackbar}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleCloseSnackbar} severity="success" sx={{ width: '100%' }}>
          {successMessage}
        </Alert>
      </Snackbar>

      {/* Error Snackbar */}
      <Snackbar
        open={!!error}
        autoHideDuration={6000}
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

export default SellerTransactions;
