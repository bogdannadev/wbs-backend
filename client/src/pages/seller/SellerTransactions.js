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
  Alert,
  Avatar
} from '@mui/material';
import {
  Search as SearchIcon,
  Close as CloseIcon,
  FilterList as FilterListIcon,
  Cancel as CancelIcon,
  Undo as UndoIcon
} from '@mui/icons-material';
import { sellerService } from '../../services/api';
import { useAuth } from '../../contexts/AuthContext';

const SellerTransactions = () => {
  const { user } = useAuth();
  
  // Transaction history state
  const [transactions, setTransactions] = useState([
    { 
      id: 1, 
      timestamp: '2024-03-06 14:30', 
      type: 'Earn', 
      amount: 50, 
      buyerName: 'John Doe',
      buyerId: '11111111-1111-1111-1111-111111111111',
      description: 'Purchase: Grocery items', 
      status: 'Completed',
      processingTime: '2.3 seconds'
    },
    { 
      id: 2, 
      timestamp: '2024-03-06 11:15', 
      type: 'Spend', 
      amount: 20, 
      buyerName: 'Jane Smith',
      buyerId: '22222222-2222-2222-2222-222222222222',
      description: 'Discount applied: Clothing', 
      status: 'Completed',
      processingTime: '1.8 seconds'
    },
    { 
      id: 3, 
      timestamp: '2024-03-05 16:45', 
      type: 'Earn', 
      amount: 35, 
      buyerName: 'Robert Johnson',
      buyerId: '33333333-3333-3333-3333-333333333333',
      description: 'Purchase: Electronics', 
      status: 'Completed',
      processingTime: '2.1 seconds'
    },
    { 
      id: 4, 
      timestamp: '2024-03-05 12:30', 
      type: 'Spend', 
      amount: 15, 
      buyerName: 'Sarah Williams',
      buyerId: '44444444-4444-4444-4444-444444444444',
      description: 'Discount applied: Produce', 
      status: 'Completed',
      processingTime: '1.5 seconds'
    },
    { 
      id: 5, 
      timestamp: '2024-03-05 09:20', 
      type: 'Earn', 
      amount: 25, 
      buyerName: 'Michael Brown',
      buyerId: '55555555-5555-5555-5555-555555555555',
      description: 'Purchase: Home goods', 
      status: 'Completed',
      processingTime: '2.7 seconds'
    },
    { 
      id: 6, 
      timestamp: '2024-03-04 17:10', 
      type: 'Earn', 
      amount: 30, 
      buyerName: 'Emily Davis',
      buyerId: '66666666-6666-6666-6666-666666666666',
      description: 'Purchase: Grocery items', 
      status: 'Completed',
      processingTime: '2.2 seconds'
    },
    { 
      id: 7, 
      timestamp: '2024-03-04 15:40', 
      type: 'Spend', 
      amount: 25, 
      buyerName: 'David Miller',
      buyerId: '77777777-7777-7777-7777-777777777777',
      description: 'Discount applied: Accessories', 
      status: 'Completed',
      processingTime: '1.9 seconds'
    },
    { 
      id: 8, 
      timestamp: '2024-03-06 16:25', 
      type: 'Earn', 
      amount: 40, 
      buyerName: 'John Doe',
      buyerId: '11111111-1111-1111-1111-111111111111',
      description: 'Purchase: Household items', 
      status: 'Pending',
      processingTime: 'In progress'
    },
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
    buyer: 'All'
  });
  
  // Get all unique buyers for filter dropdown
  const uniqueBuyers = [...new Set(transactions.map(t => t.buyerName))];
  
  // Load transaction data
  useEffect(() => {
    const fetchTransactions = async () => {
      try {
        setLoading(true);
        // This would be an API call in the real implementation
        // const response = await sellerService.getTransactions();
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
      transaction.buyerName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      transaction.description.toLowerCase().includes(searchQuery.toLowerCase());
    
    // Filter by tab (today, yesterday, all)
    const today = new Date().toISOString().split('T')[0];
    const yesterday = new Date(Date.now() - 86400000).toISOString().split('T')[0];
    const transactionDate = transaction.timestamp.split(' ')[0];
    
    const matchesTab = 
      (selectedTab === 0) || // All
      (selectedTab === 1 && transactionDate === today) || // Today
      (selectedTab === 2 && transactionDate === yesterday); // Yesterday
    
    // Filter by date range
    const transactionDateTime = new Date(transaction.timestamp);
    const matchesDateFrom = filters.dateFrom ? new Date(filters.dateFrom) <= transactionDateTime : true;
    const matchesDateTo = filters.dateTo ? new Date(filters.dateTo) >= transactionDateTime : true;
    
    // Filter by transaction type
    const matchesType = filters.type === 'All' || transaction.type === filters.type;
    
    // Filter by status
    const matchesStatus = filters.status === 'All' || transaction.status === filters.status;
    
    // Filter by buyer
    const matchesBuyer = filters.buyer === 'All' || transaction.buyerName === filters.buyer;
    
    return matchesSearch && matchesTab && matchesDateFrom && matchesDateTo && 
           matchesType && matchesStatus && matchesBuyer;
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
      buyer: 'All'
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
  
  // Process transaction return/cancellation
  const handleProcessReturn = async () => {
    try {
      setLoading(true);
      
      // In a real implementation, this would call the API
      // await sellerService.confirmReturn(selectedTransaction.id);
      
      // Update transaction status locally
      const updatedTransactions = transactions.map(t => {
        if (t.id === selectedTransaction.id) {
          return { ...t, status: 'Returned' };
        }
        return t;
      });
      
      setTransactions(updatedTransactions);
      
      setNotification({
        open: true,
        message: 'Transaction return has been processed successfully',
        severity: 'success'
      });
      
      handleCloseCancelDialog();
      setLoading(false);
    } catch (error) {
      console.error('Error processing return:', error);
      
      setNotification({
        open: true,
        message: 'Failed to process return. Please try again later.',
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
          View and manage your processed bonus transactions.
        </Typography>
      </Paper>
      
      <Card sx={{ mb: 3 }}>
        <Box sx={{ borderBottom: 1, borderColor: 'divider' }}>
          <Tabs value={selectedTab} onChange={handleTabChange} aria-label="transaction history tabs">
            <Tab label="All Transactions" />
            <Tab label="Today" />
            <Tab label="Yesterday" />
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
                    <TableCell>Buyer</TableCell>
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
                        opacity: transaction.status === 'Returned' ? 0.6 : 1,
                        textDecoration: transaction.status === 'Returned' ? 'line-through' : 'none'
                      }}
                    >
                      <TableCell>{transaction.timestamp}</TableCell>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          <Avatar sx={{ width: 28, height: 28, mr: 1, bgcolor: 'primary.main' }}>
                            {transaction.buyerName.charAt(0)}
                          </Avatar>
                          {transaction.buyerName}
                        </Box>
                      </TableCell>
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
                            transaction.status === 'Returned' ? 'error' : 'default'
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
                  <MenuItem value="Returned">Returned</MenuItem>
                </Select>
              </FormControl>
            </Grid>
            <Grid item xs={12}>
              <FormControl fullWidth>
                <InputLabel id="buyer-label">Buyer</InputLabel>
                <Select
                  labelId="buyer-label"
                  id="buyer"
                  value={filters.buyer}
                  label="Buyer"
                  onChange={(e) => setFilters({ ...filters, buyer: e.target.value })}
                >
                  <MenuItem value="All">All Buyers</MenuItem>
                  {uniqueBuyers.map((buyer, index) => (
                    <MenuItem key={index} value={buyer}>{buyer}</MenuItem>
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
                      selectedTransaction.status === 'Returned' ? 'error' : 'default'
                    }
                    size="small"
                  />
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Buyer
                  </Typography>
                  <Typography variant="body1">
                    {selectedTransaction.buyerName}
                  </Typography>
                  <Typography variant="caption" color="text.secondary">
                    ID: {selectedTransaction.buyerId}
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
              <Grid item xs={12} sm={6}>
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
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Processing Time
                  </Typography>
                  <Typography variant="body1">
                    {selectedTransaction.processingTime}
                  </Typography>
                </Box>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            {selectedTransaction.status === 'Completed' && (
              <Button 
                startIcon={<UndoIcon />} 
                color="error" 
                onClick={() => handleOpenCancelDialog(selectedTransaction)}
              >
                Process Return
              </Button>
            )}
            <Button onClick={handleCloseDetailDialog}>Close</Button>
          </DialogActions>
        </Dialog>
      )}
      
      {/* Process Return Confirmation Dialog */}
      {selectedTransaction && (
        <Dialog open={openCancelDialog} onClose={handleCloseCancelDialog} maxWidth="xs" fullWidth>
          <DialogTitle>
            Process Transaction Return
          </DialogTitle>
          <DialogContent>
            <Typography variant="body1" gutterBottom>
              Are you sure you want to process a return for this transaction?
            </Typography>
            {selectedTransaction.type === 'Earn' ? (
              <Typography variant="body2" color="error">
                This will remove {selectedTransaction.amount} bonus points from the customer's account.
              </Typography>
            ) : (
              <Typography variant="body2" color="success.main">
                This will return {selectedTransaction.amount} bonus points to the customer's account.
              </Typography>
            )}
          </DialogContent>
          <DialogActions>
            <Button onClick={handleCloseCancelDialog}>Cancel</Button>
            <Button 
              color="error" 
              variant="contained" 
              onClick={handleProcessReturn}
              disabled={loading}
            >
              {loading ? <CircularProgress size={24} /> : 'Process Return'}
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

export default SellerTransactions;
