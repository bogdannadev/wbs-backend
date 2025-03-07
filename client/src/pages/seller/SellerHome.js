import React, { useState, useEffect } from 'react';
import {
  Grid,
  Paper,
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button,
  Skeleton,
  Divider,
  List,
  ListItem,
  ListItemText,
  ListItemIcon,
  Chip,
  useTheme
} from '@mui/material';
import {
  ArrowUpward as EarnedIcon,
  ArrowDownward as SpentIcon,
  Store as StoreIcon,
  BarChart as ChartIcon,
  AssignmentTurnedIn as AssignmentIcon,
  Person as PersonIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { sellerService } from '../../services/api';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';

const SellerHome = ({ userContext }) => {
  const [storeData, setStoreData] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();
  const theme = useTheme();

  useEffect(() => {
    const fetchStoreData = async () => {
      try {
        setLoading(true);
        // In a real implementation, this would be a real API call
        // For demo purposes, we'll use mock data
        
        // Simulate API delay
        await new Promise(resolve => setTimeout(resolve, 1000));
        
        const mockData = {
          storeName: "Downtown Store",
          storeId: "store1",
          dailyTransactions: 28,
          dailyEarned: 345,
          dailySpent: 120,
          bonusBalance: 12450,
          recentCustomers: [
            { id: "11111111-1111-1111-1111-111111111111", name: "John Doe", timestamp: new Date().toISOString() },
            { id: "22222222-2222-2222-2222-222222222222", name: "Jane Smith", timestamp: new Date(Date.now() - 30 * 60000).toISOString() },
            { id: "33333333-3333-3333-3333-333333333333", name: "Bob Johnson", timestamp: new Date(Date.now() - 60 * 60000).toISOString() }
          ],
          weeklyData: [
            { day: 'Mon', earned: 420, spent: 180 },
            { day: 'Tue', earned: 380, spent: 240 },
            { day: 'Wed', earned: 450, spent: 150 },
            { day: 'Thu', earned: 520, spent: 290 },
            { day: 'Fri', earned: 600, spent: 320 },
            { day: 'Sat', earned: 750, spent: 400 },
            { day: 'Sun', earned: 580, spent: 250 }
          ]
        };
        
        setStoreData(mockData);
        setLoading(false);
      } catch (error) {
        console.error('Error fetching store data:', error);
        setLoading(false);
      }
    };
    
    fetchStoreData();
  }, []);

  const handleNavigateToTransactions = () => {
    navigate('/seller/transactions');
  };

  const handleNavigateToProcessTransaction = () => {
    navigate('/seller/transaction');
  };

  return (
    <Grid container spacing={3}>
      {/* Welcome Card */}
      <Grid item xs={12}>
        <Paper elevation={2} sx={{ p: 3, mb: 1 }}>
          <Typography variant="h4" gutterBottom>
            Welcome, {userContext?.username || 'Seller'}
          </Typography>
          <Typography variant="body1">
            Manage transactions and view store performance from this dashboard.
          </Typography>
        </Paper>
      </Grid>
      
      {/* Quick Action Cards */}
      <Grid item xs={12} md={6}>
        <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
          <CardContent sx={{ flexGrow: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <AssignmentIcon sx={{ color: theme.palette.primary.main, mr: 1 }} />
              <Typography variant="h6">
                Process Transaction
              </Typography>
            </Box>
            <Divider sx={{ mb: 2 }} />
            <Typography variant="body2" color="text.secondary" paragraph>
              Process new transactions by scanning customer QR codes.
              You can process both earning and redemption of bonus points.
            </Typography>
          </CardContent>
          <CardActions>
            <Button 
              size="large" 
              variant="contained" 
              fullWidth
              onClick={handleNavigateToProcessTransaction}
            >
              Process New Transaction
            </Button>
          </CardActions>
        </Card>
      </Grid>
      
      <Grid item xs={12} md={6}>
        <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
          <CardContent sx={{ flexGrow: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <StoreIcon sx={{ color: theme.palette.primary.main, mr: 1 }} />
              <Typography variant="h6">
                Store Status
              </Typography>
            </Box>
            <Divider sx={{ mb: 2 }} />
            
            {loading ? (
              <Box>
                <Skeleton variant="text" height={30} />
                <Skeleton variant="text" height={30} />
                <Skeleton variant="text" height={30} />
              </Box>
            ) : (
              <>
                <Typography variant="body1" sx={{ fontWeight: 'bold', mb: 1 }}>
                  {storeData?.storeName || 'Store Name'}
                </Typography>
                
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2" color="text.secondary">
                    Daily Transactions:
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {storeData?.dailyTransactions || 0}
                  </Typography>
                </Box>
                
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2" color="text.secondary">
                    Points Earned Today:
                  </Typography>
                  <Typography variant="body2" fontWeight="bold" color="success.main">
                    +{storeData?.dailyEarned || 0}
                  </Typography>
                </Box>
                
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2" color="text.secondary">
                    Points Spent Today:
                  </Typography>
                  <Typography variant="body2" fontWeight="bold" color="error.main">
                    -{storeData?.dailySpent || 0}
                  </Typography>
                </Box>
                
                <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                  <Typography variant="body2" color="text.secondary">
                    Store Bonus Balance:
                  </Typography>
                  <Typography variant="body2" fontWeight="bold">
                    {storeData?.bonusBalance?.toLocaleString() || 0}
                  </Typography>
                </Box>
              </>
            )}
          </CardContent>
          <CardActions>
            <Button 
              size="large" 
              variant="outlined" 
              fullWidth
              onClick={handleNavigateToTransactions}
            >
              View Transactions
            </Button>
          </CardActions>
        </Card>
      </Grid>
      
      {/* Weekly Stats Chart */}
      <Grid item xs={12}>
        <Card>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <ChartIcon sx={{ color: theme.palette.primary.main, mr: 1 }} />
              <Typography variant="h6">
                Weekly Performance
              </Typography>
            </Box>
            <Divider sx={{ mb: 2 }} />
            
            {loading ? (
              <Box sx={{ height: 300, display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                <Skeleton variant="rectangular" width="100%" height={300} />
              </Box>
            ) : (
              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={storeData?.weeklyData || []}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="day" />
                    <YAxis />
                    <Tooltip />
                    <Legend />
                    <Bar dataKey="earned" name="Points Earned" fill={theme.palette.primary.main} />
                    <Bar dataKey="spent" name="Points Spent" fill={theme.palette.secondary.main} />
                  </BarChart>
                </ResponsiveContainer>
              </Box>
            )}
          </CardContent>
        </Card>
      </Grid>
      
      {/* Recent Customers */}
      <Grid item xs={12}>
        <Card>
          <CardContent>
            <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
              <PersonIcon sx={{ color: theme.palette.primary.main, mr: 1 }} />
              <Typography variant="h6">
                Recent Customers
              </Typography>
            </Box>
            <Divider sx={{ mb: 2 }} />
            
            {loading ? (
              <Box>
                <Skeleton variant="rectangular" height={50} sx={{ mb: 1 }} />
                <Skeleton variant="rectangular" height={50} sx={{ mb: 1 }} />
                <Skeleton variant="rectangular" height={50} />
              </Box>
            ) : (
              <List>
                {storeData?.recentCustomers?.length ? (
                  storeData.recentCustomers.map((customer) => (
                    <ListItem key={customer.id} divider>
                      <ListItemIcon>
                        <PersonIcon />
                      </ListItemIcon>
                      <ListItemText
                        primary={customer.name}
                        secondary={`Last seen: ${new Date(customer.timestamp).toLocaleTimeString()}`}
                      />
                      <Button variant="outlined" size="small">
                        View History
                      </Button>
                    </ListItem>
                  ))
                ) : (
                  <ListItem>
                    <ListItemText primary="No recent customers" />
                  </ListItem>
                )}
              </List>
            )}
          </CardContent>
        </Card>
      </Grid>
    </Grid>
  );
};

export default SellerHome;
