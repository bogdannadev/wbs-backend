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
  Dialog,
  DialogActions,
  DialogContent,
  DialogContentText,
  DialogTitle,
  Snackbar,
  Alert,
  CircularProgress
} from '@mui/material';
import { 
  ArrowUpward as EarnedIcon,
  ArrowDownward as SpentIcon,
  Undo as CancelIcon,
  WatchLater as PendingIcon,
  CheckCircle as CompletedIcon,
  Cancel as RejectedIcon
 } from '@mui/icons-material';
import { buyerService } from '../../services/api';

const BuyerTransactions = () => {
  const [transactions, setTransactions] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [transactionToCancel, setTransactionToCancel] = useState(null);
  const [cancellingTransaction, setCancellingTransaction] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');

  useEffect(() => {
    const fetchTransactions = async () => {
      try {
        setLoading(true);
        const response = await buyerService.getTransactions();
        setTransactions(response.data);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching transactions:', error);
        setError('Failed to load transactions. Please try again later.');
        setLoading(false);
      }
    };
    
    fetchTransactions();
  }, []);

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (event) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  const handleOpenCancelDialog = (transaction) => {
    setTransactionToCancel(transaction);
    setCancelDialogOpen(true);
  };

  const handleCloseCancelDialog = () => {
    setCancelDialogOpen(false);
    setTransactionToCancel(null);
  };

  const handleCancelTransaction = async () => {
    if (!transactionToCancel) return;
    
    setCancellingTransaction(true);
    
    try {
      await buyerService.cancelTransaction(transactionToCancel.id);
      
      // Update the transaction status in the local state
      setTransactions(prevTransactions => 
        prevTransactions.map(transaction => 
          transaction.id === transactionToCancel.id 
            ? { ...transaction, status: 'Reversed' } 
            : transaction
        )
      );
      
      setSuccessMessage('Transaction cancelled successfully');
    } catch (error) {
      console.error('Error cancelling transaction:', error);
      setError('Failed to cancel transaction. Please try again later.');
    } finally {
      setCancellingTransaction(false);
      handleCloseCancelDialog();
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
        return <Chip icon={<CancelIcon />} label="Cancelled" color="error" size="small" />;
      case 'Failed':
        return <Chip icon={<RejectedIcon />} label="Failed" color="error" size="small" />;
      default:
        return <Chip label={status} size="small" />;
    }
  };

  // Function to check if transaction can be cancelled
  const canCancelTransaction = (transaction) => {
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
          View your bonus point transactions and cancel recent ones if needed.
        </Typography>
      </Paper>

      <Paper sx={{ width: '100%', overflow: 'hidden' }}>
        {loading ? (
          <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
            <CircularProgress />
          </Box>
        ) : error ? (
          <Box sx={{ p: 3 }}>
            <Typography color="error">{error}</Typography>
          </Box>
        ) : (
          <>
            <TableContainer sx={{ maxHeight: 440 }}>
              <Table stickyHeader aria-label="transactions table">
                <TableHead>
                  <TableRow>
                    <TableCell>Type</TableCell>
                    <TableCell>Date</TableCell>
                    <TableCell>Amount</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {transactions.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={6} align="center">No transactions found</TableCell>
                    </TableRow>
                  ) : (
                    transactions
                      .slice(page * rowsPerPage, page * rowsPerPage + rowsPerPage)
                      .map((transaction) => (
                        <TableRow hover key={transaction.id}>
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
                          <TableCell sx={{ 
                            fontWeight: 'bold',
                            color: transaction.type === 'Earn' ? 'success.main' : 'error.main'
                          }}>
                            {transaction.type === 'Earn' ? '+' : '-'}{transaction.amount.toFixed(2)}
                          </TableCell>
                          <TableCell>{transaction.description || 'No description'}</TableCell>
                          <TableCell>{getStatusChip(transaction.status)}</TableCell>
                          <TableCell>
                            {canCancelTransaction(transaction) ? (
                              <Button 
                                size="small" 
                                variant="outlined" 
                                color="error"
                                startIcon={<CancelIcon />}
                                onClick={() => handleOpenCancelDialog(transaction)}
                              >
                                Cancel
                              </Button>
                            ) : (
                              <Button 
                                size="small" 
                                variant="outlined" 
                                disabled
                              >
                                {transaction.status === 'Reversed' ? 'Cancelled' : 'Cannot Cancel'}
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

      {/* Cancel Transaction Dialog */}
      <Dialog
        open={cancelDialogOpen}
        onClose={handleCloseCancelDialog}
        aria-labelledby="cancel-transaction-dialog-title"
      >
        <DialogTitle id="cancel-transaction-dialog-title">
          Cancel Transaction
        </DialogTitle>
        <DialogContent>
          <DialogContentText>
            Are you sure you want to cancel this transaction? This action cannot be undone.
          </DialogContentText>
          {transactionToCancel && (
            <Box sx={{ mt: 2 }}>
              <Typography variant="body2">
                <strong>Type:</strong> {transactionToCancel.type}
              </Typography>
              <Typography variant="body2">
                <strong>Amount:</strong> {transactionToCancel.amount.toFixed(2)}
              </Typography>
              <Typography variant="body2">
                <strong>Date:</strong> {new Date(transactionToCancel.timestamp).toLocaleString()}
              </Typography>
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseCancelDialog} disabled={cancellingTransaction}>
            No, Keep It
          </Button>
          <Button 
            onClick={handleCancelTransaction} 
            color="error" 
            disabled={cancellingTransaction}
            autoFocus
          >
            {cancellingTransaction ? 'Cancelling...' : 'Yes, Cancel Transaction'}
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

export default BuyerTransactions;
