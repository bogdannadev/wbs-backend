import React, { useState, useEffect } from 'react';
import {
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  Grid,
  Divider,
  Button,
  TextField,
  InputAdornment,
  IconButton,
  CircularProgress,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Snackbar,
  Alert
} from '@mui/material';
import {
  Search as SearchIcon,
  Close as CloseIcon,
  LocationOn as LocationIcon,
  ArrowForward as ArrowForwardIcon,
  CheckCircle as CheckCircleIcon,
  Schedule as ScheduleIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';
import { sellerService } from '../../services/api';

const SellerStores = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  
  // Store state
  const [stores, setStores] = useState([
    { 
      id: 1, 
      name: 'Global Retail - Downtown', 
      address: '123 Main St, Downtown', 
      status: 'Active', 
      isCurrentlySelected: false, 
      lastActive: '2024-03-05', 
      storeHours: '9:00 AM - 9:00 PM',
      company: 'Global Retail Inc.'
    },
    { 
      id: 2, 
      name: 'Global Retail - Uptown', 
      address: '456 High St, Uptown', 
      status: 'Active', 
      isCurrentlySelected: true, 
      lastActive: '2024-03-06', 
      storeHours: '8:00 AM - 10:00 PM',
      company: 'Global Retail Inc.'
    },
    { 
      id: 3, 
      name: 'Metro Grocery - West', 
      address: '789 West Ave, Westside', 
      status: 'Inactive', 
      isCurrentlySelected: false, 
      lastActive: '2024-02-28', 
      storeHours: '7:00 AM - 11:00 PM',
      company: 'Metro Grocery Chain'
    }
  ]);
  
  // UI state
  const [loading, setLoading] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [selectedStore, setSelectedStore] = useState(null);
  const [openDetailDialog, setOpenDetailDialog] = useState(false);
  const [notification, setNotification] = useState({ open: false, message: '', severity: 'success' });
  
  // Load store data
  useEffect(() => {
    const fetchStores = async () => {
      try {
        setLoading(true);
        // This would be an API call in the real implementation
        // const response = await sellerService.getStores();
        // setStores(response.data);
        
        // Simulate API call with timeout
        setTimeout(() => {
          setLoading(false);
        }, 1000);
      } catch (error) {
        console.error('Error fetching stores:', error);
        setLoading(false);
      }
    };
    
    fetchStores();
  }, []);
  
  // Filter stores based on search query
  const filteredStores = stores.filter(store => 
    store.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    store.address.toLowerCase().includes(searchQuery.toLowerCase())
  );
  
  // Select a store
  const handleSelectStore = async (storeId) => {
    try {
      setLoading(true);
      
      // In a real implementation, this would call the API
      // await sellerService.selectStore(storeId);
      
      // Update store selection locally
      const updatedStores = stores.map(store => ({
        ...store,
        isCurrentlySelected: store.id === storeId
      }));
      
      setStores(updatedStores);
      
      setNotification({
        open: true,
        message: 'Store selection updated successfully',
        severity: 'success'
      });
      
      // After selecting a store, navigate to seller dashboard
      setTimeout(() => {
        navigate('/seller');
      }, 1500);
      
      setLoading(false);
    } catch (error) {
      console.error('Error selecting store:', error);
      
      setNotification({
        open: true,
        message: 'Failed to update store selection. Please try again.',
        severity: 'error'
      });
      
      setLoading(false);
    }
  };
  
  // Open store detail dialog
  const handleOpenDetailDialog = (store) => {
    setSelectedStore(store);
    setOpenDetailDialog(true);
  };
  
  // Close store detail dialog
  const handleCloseDetailDialog = () => {
    setOpenDetailDialog(false);
    setSelectedStore(null);
  };
  
  // Close notification
  const handleCloseNotification = () => {
    setNotification({ ...notification, open: false });
  };
  
  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Store Selection
        </Typography>
        <Typography variant="body1">
          Select the store location you are currently working at.
        </Typography>
      </Paper>
      
      <Box sx={{ mb: 3 }}>
        <TextField
          fullWidth
          placeholder="Search stores"
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
      </Box>
      
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      ) : filteredStores.length > 0 ? (
        <Grid container spacing={3}>
          {filteredStores.map((store) => (
            <Grid item xs={12} md={6} lg={4} key={store.id}>
              <Card 
                sx={{ 
                  height: '100%',
                  borderColor: store.isCurrentlySelected ? 'primary.main' : 'divider',
                  borderWidth: store.isCurrentlySelected ? 2 : 1,
                  borderStyle: 'solid'
                }}
              >
                <CardContent sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                  <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                    <Typography variant="h6" gutterBottom>
                      {store.name}
                    </Typography>
                    <Chip
                      label={store.status}
                      color={store.status === 'Active' ? 'success' : 'error'}
                      size="small"
                    />
                  </Box>
                  
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <LocationIcon fontSize="small" color="action" sx={{ mr: 1 }} />
                    <Typography variant="body2">{store.address}</Typography>
                  </Box>
                  
                  <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                    <ScheduleIcon fontSize="small" color="action" sx={{ mr: 1 }} />
                    <Typography variant="body2">{store.storeHours}</Typography>
                  </Box>
                  
                  <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                    {store.company}
                  </Typography>
                  
                  <Divider sx={{ my: 2 }} />
                  
                  <Box sx={{ mt: 'auto', display: 'flex', justifyContent: 'space-between' }}>
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={() => handleOpenDetailDialog(store)}
                    >
                      Details
                    </Button>
                    
                    {store.isCurrentlySelected ? (
                      <Button
                        variant="contained"
                        color="success"
                        size="small"
                        startIcon={<CheckCircleIcon />}
                        disabled
                      >
                        Selected
                      </Button>
                    ) : (
                      <Button
                        variant="contained"
                        size="small"
                        endIcon={<ArrowForwardIcon />}
                        onClick={() => handleSelectStore(store.id)}
                        disabled={store.status !== 'Active'}
                      >
                        Select Store
                      </Button>
                    )}
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          ))}
        </Grid>
      ) : (
        <Paper sx={{ p: 4, textAlign: 'center' }}>
          <Typography variant="body1" color="text.secondary">
            No stores found matching your search.
          </Typography>
        </Paper>
      )}
      
      {/* Store Detail Dialog */}
      {selectedStore && (
        <Dialog open={openDetailDialog} onClose={handleCloseDetailDialog} maxWidth="sm" fullWidth>
          <DialogTitle>
            <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
              <Typography variant="h6">{selectedStore.name}</Typography>
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
                    Address
                  </Typography>
                  <Typography variant="body1">
                    {selectedStore.address}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Company
                  </Typography>
                  <Typography variant="body1">
                    {selectedStore.company}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Status
                  </Typography>
                  <Chip
                    label={selectedStore.status}
                    color={selectedStore.status === 'Active' ? 'success' : 'error'}
                    size="small"
                  />
                </Box>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Store Hours
                  </Typography>
                  <Typography variant="body1">
                    {selectedStore.storeHours}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12} sm={6}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Last Active
                  </Typography>
                  <Typography variant="body1">
                    {selectedStore.lastActive}
                  </Typography>
                </Box>
              </Grid>
              <Grid item xs={12}>
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Currently Selected
                  </Typography>
                  <Typography variant="body1">
                    {selectedStore.isCurrentlySelected ? 'Yes' : 'No'}
                  </Typography>
                </Box>
              </Grid>
            </Grid>
          </DialogContent>
          <DialogActions>
            {!selectedStore.isCurrentlySelected && selectedStore.status === 'Active' && (
              <Button 
                variant="contained" 
                onClick={() => {
                  handleCloseDetailDialog();
                  handleSelectStore(selectedStore.id);
                }}
              >
                Select This Store
              </Button>
            )}
            <Button onClick={handleCloseDetailDialog}>Close</Button>
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

export default SellerStores;
