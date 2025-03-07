import React, { useState } from 'react';
import { 
  Paper, 
  Typography, 
  Box, 
  TextField, 
  Button, 
  Grid, 
  Card, 
  CardContent, 
  CardActions,
  Chip,
  MenuItem,
  InputAdornment,
  CircularProgress,
  Divider,
  Alert,
  FormControl,
  InputLabel,
  Select
} from '@mui/material';
import {
  Search as SearchIcon,
  Store as StoreIcon,
  LocationOn as LocationIcon,
  Phone as PhoneIcon
} from '@mui/icons-material';
import { buyerService } from '../../services/api';

const BuyerStores = () => {
  const [searchTerm, setSearchTerm] = useState('');
  const [searchResults, setSearchResults] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [searched, setSearched] = useState(false);
  const [selectedCategory, setSelectedCategory] = useState('All');

  const categories = [
    'All',
    'Restaurant',
    'Retail',
    'Grocery',
    'Electronics',
    'Fashion',
    'Beauty',
    'Health',
    'Home Goods',
    'Sports',
    'Entertainment'
  ];

  const handleSearch = async () => {
    if (!searchTerm && selectedCategory === 'All') {
      setError('Please enter a search term or select a category');
      return;
    }

    setLoading(true);
    setError('');
    
    try {
      // Prepare search query based on input and selected category
      const category = selectedCategory === 'All' ? searchTerm : selectedCategory;
      const response = await buyerService.findStores(category);
      setSearchResults(response.data);
      setSearched(true);
    } catch (error) {
      console.error('Error searching stores:', error);
      setError('Failed to search stores. Please try again later.');
    } finally {
      setLoading(false);
    }
  };

  // Demo data for prototype showcase (when backend is not available)
  const getDemoStores = () => {
    return [
      {
        id: '1',
        name: 'Metro Grocery',
        location: 'Downtown',
        address: '123 Main St, City Center',
        contactPhone: '(555) 123-4567',
        status: 'Active'
      },
      {
        id: '2',
        name: 'Fashion Outlet',
        location: 'Shopping Mall',
        address: '456 Market Ave, Fashion District',
        contactPhone: '(555) 987-6543',
        status: 'Active'
      },
      {
        id: '3',
        name: 'Tech Gadgets',
        location: 'Tech Park',
        address: '789 Innovation Way, Tech District',
        contactPhone: '(555) 456-7890',
        status: 'Active'
      },
      {
        id: '4',
        name: 'Sunny Cafe',
        location: 'Riverside',
        address: '321 River View, Waterfront',
        contactPhone: '(555) 234-5678',
        status: 'Active'
      }
    ];
  };

  const handleKeyPress = (e) => {
    if (e.key === 'Enter') {
      handleSearch();
    }
  };

  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Find Stores
        </Typography>
        <Typography variant="body1">
          Search for stores where you can earn and redeem bonus points.
        </Typography>
      </Paper>

      <Paper sx={{ p: 3, mb: 3 }}>
        <Grid container spacing={2} alignItems="center">
          <Grid item xs={12} md={5}>
            <TextField
              fullWidth
              label="Search Stores"
              variant="outlined"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyPress={handleKeyPress}
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              }}
              placeholder="Enter store name or location"
            />
          </Grid>
          <Grid item xs={12} md={5}>
            <FormControl fullWidth>
              <InputLabel id="category-select-label">Category</InputLabel>
              <Select
                labelId="category-select-label"
                id="category-select"
                value={selectedCategory}
                label="Category"
                onChange={(e) => setSelectedCategory(e.target.value)}
              >
                {categories.map((category) => (
                  <MenuItem key={category} value={category}>
                    {category}
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
          </Grid>
          <Grid item xs={12} md={2}>
            <Button
              variant="contained"
              color="primary"
              fullWidth
              onClick={handleSearch}
              disabled={loading}
              startIcon={loading ? <CircularProgress size={20} color="inherit" /> : null}
            >
              {loading ? 'Searching...' : 'Search'}
            </Button>
          </Grid>
          {error && (
            <Grid item xs={12}>
              <Alert severity="error">{error}</Alert>
            </Grid>
          )}
        </Grid>
      </Paper>

      <Box>
        {!searched && !loading && (
          <Paper sx={{ p: 3, mb: 3, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary">
              Search for stores by name, location, or select a category to see results.
            </Typography>
          </Paper>
        )}

        {searched && !loading && searchResults.length === 0 ? (
          <Paper sx={{ p: 3, mb: 3, textAlign: 'center' }}>
            <Typography variant="body1" color="text.secondary">
              No stores found matching your search criteria.
            </Typography>
          </Paper>
        ) : (
          <Grid container spacing={3}>
            {/* Use demo data if no results but searched flag is true (for prototype) */}
            {(searchResults.length > 0 ? searchResults : (searched ? getDemoStores() : [])).map((store) => (
              <Grid item xs={12} sm={6} md={4} key={store.id}>
                <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
                  <CardContent sx={{ flexGrow: 1 }}>
                    <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 2 }}>
                      <Typography variant="h6" component="h2" gutterBottom>
                        {store.name}
                      </Typography>
                      <Chip 
                        label={store.status} 
                        color={store.status === 'Active' ? 'success' : 'default'} 
                        size="small"
                      />
                    </Box>
                    <Divider sx={{ mb: 2 }} />
                    <Box sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
                      <StoreIcon sx={{ color: 'text.secondary', mr: 1 }} />
                      <Typography variant="body2" color="text.secondary">
                        {store.location}
                      </Typography>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'flex-start', mb: 1 }}>
                      <LocationIcon sx={{ color: 'text.secondary', mr: 1, mt: 0.5 }} />
                      <Typography variant="body2">
                        {store.address}
                      </Typography>
                    </Box>
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <PhoneIcon sx={{ color: 'text.secondary', mr: 1 }} />
                      <Typography variant="body2">
                        {store.contactPhone}
                      </Typography>
                    </Box>
                  </CardContent>
                  <CardActions>
                    <Button size="small" color="primary">
                      View Details
                    </Button>
                    <Button size="small" color="secondary">
                      Get Directions
                    </Button>
                  </CardActions>
                </Card>
              </Grid>
            ))}
          </Grid>
        )}
      </Box>
    </Box>
  );
};

export default BuyerStores;
