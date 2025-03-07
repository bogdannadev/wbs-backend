import React, { useState, useEffect } from 'react';
import {
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Chip,
  TextField,
  InputAdornment,
  IconButton,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress,
  Divider,
  Tabs,
  Tab,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Snackbar,
  Alert
} from '@mui/material';
import {
  Search as SearchIcon,
  Close as CloseIcon,
  FilterList as FilterListIcon,
  Info as InfoIcon,
  Cancel as CancelIcon
} from '@mui/icons-material';
import { buyerService } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';

const BuyerTransactions = () => {
  const { user } = useAuth();
  
  // Transaction history state
  const [transactions, setTransactions] = useState([
    { id: 1, timestamp: '2024-03-05 14:30', type: 'Earn', amount: 50, storeName: 'Global Retail - Downtown', description: 'Purchase: Grocery items', status: 'Completed' },
    { id: 2, timestamp: '2024-03-03 11:15', type: 'Spend', amount: 20, storeName: 'Fashion Outlets - Mall', description: 'Discount applied: Clothing', status: 'Completed' },
    { id: 3, timestamp: '2024-02-28 16:45', type: 'Earn', amount: 35, storeName: 'Tech Gadgets - Main St', description: 'Purchase: Electronics', status: 'Completed' },
    { id: 4, timestamp: '2024-02-25 12:30', type: 'Spend', amount: 15, storeName: 'Metro Grocery - West', description: 'Discount applied: Produce', status: 'Completed' },
    { id: 5, timestamp: '2024-02-20 09:20', type: 'Earn', amount: 25, storeName: 'Global Retail - Uptown', description: 'Purchase: Home goods', status: 'Completed' },
    { id: 6, timestamp: '2024-02-18 17:10', type: 'Earn', amount: 30, storeName: 'Metro Grocery - Central', description: 'Purchase: Grocery items', status: 'Completed' },
    { id: 7, timestamp: '2024-02-15 15:40', type: 'Spend', amount: 25, storeName: 'Tech Gadgets - Main St', description: 'Discount applied: Accessories', status: 'Completed' },
    { id: 8, timestamp: '2024-03-06 10:25', type: 'Earn', amount: 40, storeName: 'Global Retail - Downtown', description: 'Purchase: Household items', status: 'Pending' },
  ]);
  
  // UI state
  const [loading, setLoading] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedTab, setSelectedTab] = useState(0);
  const [openFilters, setOpenFilters] = useState(false);
  const [selectedTransaction, setSelectedTransaction] = useState(null);
  const [openDetailDialog, setOpenDetailDialog] = useState(false);
  const [openCancelDialog, setOpenCancelDialog] = useState(false);
  const [notification, setNotification] = useState({ open: false, message: '', severity: 'success' });
  
  // Filter state
  const [filters, setFilters] = useState({
    dateFrom: '',
    dateTo: '',
    type: 'All',
    status: 'All',
    store: 'All'
  });
  
  // Get all unique stores for filter dropdown
  const uniqueStores = [...new Set(transactions.map(t => t.storeName))];
  
  // Load transaction data
  useEffect(() => {
    const fetchTransactions = async () => {
      try {
        setLoading(true);
        // This would be an API call in the real implementation
        // const response = await buyerService.getTransactions();
        // setTransactions(response.data);
        
        // Simulate API call with timeout
        setTimeout(() => {
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching transactions:', error);
        setLoading(false);
      }
    };
    
    fetchTransactions();
  }, []);
  
  // Filter transactions based on search query, tab selection and filters
  const filteredTransactions = transactions.filter(transaction => {
    // Filter by search query
    const matchesSearch = 
      transaction.storeName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      transaction.description.toLowerCase().includes(searchQuery.toLowerCase());
    
    // Filter by tab (transaction type)
    const matchesTab = 
      selectedTab === 0 || // All
      (selectedTab === 1 && transaction.type === 'Earn') || // Earned
      (selectedTab === 2 && transaction.type === 'Spend'); // Spent
    
    // Filter by date range
    const transactionDate = new Date(transaction.timestamp);
    const matchesDateFrom = filters.dateFrom ? new Date(filters.dateFrom) <= transactionDate : true;
    const matchesDateTo = filters.dateTo ? new Date(filters.dateTo) >= transactionDate : true;
    
    // Filter by transaction type
    const matchesType = filters.type === 'All' || transaction.type === filters.type;
    
    // Filter by status
    const matchesStatus = filters.status === 'All' || transaction.status === filters.status;
    
    // Filter by store
    const matchesStore = filters.store === 'All' || transaction.storeName === filters.store;
    
    return matchesSearch && matchesTab && matchesDateFrom && matchesDateTo && 
           matchesType && matchesStatus && matchesStore;
  });
  
  // Handle tab change
  const handleTabChange = (event, newValue) => {
    setSelectedTab(newValue);
  };
  
  // Open the filter dialog
  const handleOpenFilters = () => {
    setOpenFilters(true);
  };
  
  // Close the filter dialog
  const handleCloseFilters = () => {
    setOpenFilters(false);
  };
  
  // Reset all filters
  const handleResetFilters = () => {
    setFilters({
      dateFrom: '',
      dateTo: '',
      type: 'All',
      status: 'All',
      store: 'All'
    });
  };
  
  // Apply filters and close dialog
  const handleApplyFilters = () => {
    handleCloseFilters();
  };
  
  // Open transaction detail dialog
  const handleOpenDetailDialog = (transaction) => {
    setSelectedTransaction(transaction);
    setOpenDetailDialog(true);
  };
  
  // Close transaction detail dialog
  const handleCloseDetailDialog = () => {
    setOpenDetailDialog(false);
    setSelectedTransaction(null);
  };
  
  // Open cancel transaction dialog
  const handleOpenCancelDialog = (transaction) => {
    setSelectedTransaction(transaction);
    setOpenCancelDialog(true);
    handleCloseDetailDialog();
  };
  
  // Close cancel transaction dialog
  const handleCloseCancelDialog = () => {
    setOpenCancelDialog(false);
    setSelectedTransaction(null);
  };
  
  // Cancel transaction
  const handleCancelTransaction = async () => {
    try {
      setLoading(true);
      
      // In a real implementation, this would call the API
      // await buyerService.cancelTransaction(selectedTransaction.id);
      
      // Update transaction status locally
      const updatedTransactions = transactions.map(t => {
        if (t.id === selectedTransaction.id) {
          return { ...t, status: 'Cancelled' };
        }
        return t;
      });
      
      setTransactions(updatedTransactions);
      
      setNotification({
        open: true,
        message: 'Transaction has been cancelled successfully',
        severity: 'success'
      });
      
      handleCloseCancelDialog();
      setLoading(false);
    } catch (error) {
      console.error('Error cancelling transaction:', error);
      
      setNotification({
        open: true,
        message: 'Failed to cancel transaction. Please try again later.',
        severity: 'error'
      });
      
      setLoading(false);
    }
  };
  
  // Close notification
  const handleCloseNotification = () => {
    setNotification({ ...notification, open: false });
  };
  
  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Transaction History
        </Typography>
        <Typography variant="body1">
          View and manage your bonus point transactions.
        </Typography>
      </Paper>
      
      <Card sx={{ mb: 3 }}>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs value={selectedTab} onChange={handleTabChange} aria-label="transaction history tabs">
            <Tab label="All Transactions" />
            <Tab label="Points Earned" />
            <Tab label="Points Spent" />
          </Tabs>
        </Box>
      </Card>
      
      <Card>
        <CardContent>
          <Box sx={{ display: 'flex', mb: 3, gap: 2 }}>
            <TextField
              fullWidth
              placeholder="Search transactions"
              variant="outlined"
              size="small"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
                endAdornment: searchQuery && (
                  <InputAdornment position="end">
                    <IconButton size="small" onClick={() => setSearchQuery('')}>
                      <CloseIcon fontSize="small" />
                    </IconButton>
                  </InputAdornment>
                )
              }}
            />
            <Button
              variant="outlined"
              startIcon={<FilterListIcon />}
              onClick={handleOpenFilters}
            >
              Filters
            </Button>
          </Box>
          
          {loading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
              <CircularProgress />
            </Box>
          ) : filteredTransactions.length > 0 ? (
            <TableContainer>
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Date & Time</TableCell>
                    <TableCell>Store</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell>Points</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {filteredTransactions.map((transaction) => (
                    <TableRow 
                      key={transaction.id} 
                      hover
                      sx={{ 
                        opacity: transaction.status === 'Cancelled' ? 0.6 : 1,
                        textDecoration: transaction.status === 'Cancelled' ? 'line-through' : 'none'
                      }}
                    >
                      <TableCell>{transaction.timestamp}</TableCell>
                      <TableCell>{transaction.storeName}</TableCell>
                      <TableCell>{transaction.description}</TableCell>
                      <TableCell>
                        <Typography 
                          variant="body2" 
                          color={transaction.type === 'Earn' ? 'success.main' : 'error.main'}
                          fontWeight="medium"
                        >
                          {transaction.type === 'Earn' ? '+' : '-'}{transaction.amount}
                        </Typography>
                      </TableCell>
                      <TableCell>
                        <Chip 
                          label={transaction.status} 
                          color={
                            transaction.status === 'Completed' ? 'success' :
                            transaction.status === 'Pending' ? 'warning' :
                            transaction.status === 'Cancelled' ? 'error' : 'default'
                          }
                          size="small"
                        />
                      </TableCell>
                      <TableCell>
                        <Button 
                          size="small" 
                          variant="outlined"
                          onClick={() => handleOpenDetailDialog(transaction)}
                        >
                          Details
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          ) : (
            <Box sx={{ textAlign: 'center', py: 4 }}>
              <Typography variant="body1" color="text.secondary">
                No matching transactions found.
              </Typography>
            </Box>
          )}
        </CardContent>
      </Card>
      
      {/* Filter Dialog */}
      <Dialog open={openFilters} onClose={handleCloseFilters} maxWidth="sm" fullWidth>
        <DialogTitle>
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
            <Typography variant="h6">Filter Transactions</Typography>
            <IconButton size="small" onClick={handleCloseFilters}>
              <CloseIcon fontSize="small" />
            </IconButton>
          </Box>
        </DialogTitle>
        <DialogContent dividers>
          <Grid container spacing={2}>
            <Grid item xs={12} sm={6}>
              <TextField
                label="From Date"
                type="date"
                fullWidth
                InputLabelProps={{ shrink: true }}
                value={filters.dateFrom}
                onChange={(e) => setFilters({ ...filters, dateFrom: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <TextField
                label="To Date"
                type="date"
                fullWidth
                InputLabelProps={{ shrink: true }}
                value={filters.dateTo}
                onChange={(e) => setFilters({ ...filters, dateTo: e.target.value })}
              />
            </Grid>
            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel id="transaction-type-label">Transaction Type</InputLabel>
                <Select
                  labelId="transaction-type-label"
                  id="transaction-type"
                  value={filters.type}
                  label="Transaction Type"
                  onChange={(e) => setFilters({ ...filters, type: e.target.value })}
                >
                  <MenuItem value="All">All Types</MenuItem>
                  <MenuItem value="Earn">Earned Points</MenuItem>
                  <MenuItem value="Spend">Spent Points</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12} sm={6}>
              <FormControl fullWidth>
                <InputLabel id="status-label">Status</InputLabel>
                <Select
                  labelId="status-label"
                  id="status"
                  value={filters.status}
                  label="Status"
                  onChange={(e) => setFilters({ ...filters, status: e.target.value })}
                >
                  <MenuItem value="All">All Statuses</MenuItem>
                  <MenuItem value="Completed">Completed</MenuItem>
                  <MenuItem value="Pending">Pending</MenuItem>
                  <MenuItem value="Cancelled">Cancelled</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12}>
              <FormControl fullWidth>
                <InputLabel id="store-label">Store</InputLabel>
                <Select
                  labelId="store-label"
                  id="store"
                  value={filters.store}
                  label="Store"
                  onChange={(e) => setFilters({ ...filters, store: e.target.value })}
                >
                  <MenuItem value="All">All Stores</MenuItem>
                  {uniqueStores.map((store, index) => (
                    <MenuItem key={index} value={store}>{store}</MenuItem>
                  ))}
                </Select>
              </FormControl>
            </Grid>
          </Grid>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleResetFilters} color="inherit">
            Reset Filters
          </Button>
          <Button onClick={handleApplyFilters} variant="contained">
            Apply Filters
          </Button>
        </DialogActions>
      </Dialog>
      
      {/* Transaction Detail Dialog */}
      {selectedTransaction && (
        <Dialog open={openDetailDialog} onClose={handleCloseDetailDialog} maxWidth="sm" fullWidth>
          <DialogTitle>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Typography variant="h6">Transaction Details</Typography>
              <IconButton size="small" onClick={handleCloseDetailDialog}>
                <CloseIcon fontSize="small" />
              </IconButton>
            </Box>
          </DialogTitle>
          <DialogContent dividers>
            <Grid container spacing={2}>
              <Grid item xs={12}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Transaction ID
                  </Typography>
                  <Typography variant="body1">
                    {selectedTransaction.id}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Date & Time
                  </Typography>
                  <Typography variant="body1">
                    {selectedTransaction.timestamp}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Status
                  </Typography>
                  <Chip 
                    label={selectedTransaction.status} 
                    color={
                      selectedTransaction.status === 'Completed' ? 'success' :
                      selectedTransaction.status === 'Pending' ? 'warning' :
                      selectedTransaction.status === 'Cancelled' ? 'error' : 'default'
                    }
                    size="small"
                  />
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Store
                  </Typography>
                  <Typography variant="body1">
                    {selectedTransaction.storeName}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Description
                  </Typography>
                  <Typography variant="body1">
                    {selectedTransaction.description}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Points
                  </Typography>
                  <Typography 
                    variant="body1" 
                    color={selectedTransaction.type === 'Earn' ? 'success.main' : 'error.main'}
                    fontWeight="medium"
                  >
                    {selectedTransaction.type === 'Earn' ? '+' : '-'}{selectedTransaction.amount}
                  </Typography>
                </Box>
              </Grid>
              {selectedTransaction.status === 'Pending' && (
                <Grid item xs={12}>
                  <Alert severity="info" icon={<InfoIcon />}>
                    This transaction is currently being processed and will be completed shortly.
                  </Alert>
                </Grid>
              )}
            </Grid>
          </DialogContent>
          <DialogActions>
            {selectedTransaction.status !== 'Cancelled' && (
              <Button 
                startIcon={<CancelIcon />} 
                color="error" 
                onClick={() => handleOpenCancelDialog(selectedTransaction)}
                disabled={selectedTransaction.status === 'Completed' && new Date(selectedTransaction.timestamp) < new Date(Date.now() - 24 * 60 * 60 * 1000)}
              >
                Cancel Transaction
              </Button>
            )}
            <Button onClick={handleCloseDetailDialog}>Close</Button>
          </DialogActions>
        </Dialog>
      )}
      
      {/* Cancel Transaction Confirmation Dialog */}
      {selectedTransaction && (
        <Dialog open={openCancelDialog} onClose={handleCloseCancelDialog} maxWidth="xs" fullWidth>
          <DialogTitle>
            Cancel Transaction
          </DialogTitle>
          <DialogContent>
            <Typography variant="body1" gutterBottom>
              Are you sure you want to cancel this transaction?
            </Typography>
            {selectedTransaction.type === 'Earn' ? (
              <Typography variant="body2" color="error">
                This will remove {selectedTransaction.amount} bonus points from your account.
              </Typography>
            ) : (
              <Typography variant="body2" color="success.main">
                This will return {selectedTransaction.amount} bonus points to your account.
              </Typography>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseCancelDialog}>No, Keep It</Button>
            <Button 
              color="error" 
              variant="contained" 
              onClick={handleCancelTransaction}
              disabled={loading}
            >
              {loading ? <CircularProgress size={24} /> : 'Yes, Cancel It'}
            </Button>
          </DialogActions>
        </Dialog>
      )}
      
      {/* Notifications */}
      <Snackbar
        open={notification.open}
        autoHideDuration={6000}
        onClose={handleCloseNotification}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
      >
        <Alert onClose={handleCloseNotification} severity={notification.severity}>
          {notification.message}
        </Alert>
      </Snackbar>
    </Box>
  );
};

export default BuyerTransactions;
