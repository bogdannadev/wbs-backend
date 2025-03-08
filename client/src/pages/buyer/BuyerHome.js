import React, { useState, useEffect } from 'react';
import { Grid, Paper, Typography, Box, Skeleton, Divider, List, ListItem, ListItemText, Card, CardContent } from '@mui/material';
import { buyerService, isDemoMode } from '../../services/api';
import { PieChart, Pie, Cell, ResponsiveContainer, Tooltip } from 'recharts';
import { useAuth } from '../../contexts/AuthContext';

const BuyerHome = ({ userContext }) => {
  const [bonusSummary, setBonusSummary] = useState(null);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();
  
  useEffect(() => {
    const fetchBonusSummary = async () => {
      try {
        setLoading(true);
        
        // Check if in demo mode by looking for stored balance
        const demoMode = isDemoMode();
        const storedBonusBalance = localStorage.getItem('bonusBalance');
        
        if (demoMode && storedBonusBalance) {
          // Create mock bonus summary for demo mode
          const bonusBalance = parseInt(storedBonusBalance, 10);
          const expiringAmount = Math.floor(bonusBalance * 0.2); // 20% expiring
          
          // Mock data
          setBonusSummary({
            currentBalance: bonusBalance,
            expiringNextQuarter: expiringAmount,
            recentTransactions: [
              {
                id: '1',
                type: 'Earn',
                amount: 50,
                timestamp: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000).toISOString(), // 2 days ago
                description: 'Purchase at Global Retail Store'
              },
              {
                id: '2',
                type: 'Spend',
                amount: 25,
                timestamp: new Date(Date.now() - 5 * 24 * 60 * 60 * 1000).toISOString(), // 5 days ago
                description: 'Discount on Electronics'
              },
              {
                id: '3',
                type: 'Earn',
                amount: 30,
                timestamp: new Date(Date.now() - 7 * 24 * 60 * 60 * 1000).toISOString(), // 7 days ago
                description: 'Purchase at Fashion Outlet'
              }
            ]
          });
        } else {
          // Real API call for non-demo mode
          const response = await buyerService.getBalance();
          setBonusSummary(response.data);
        }
        
        setLoading(false);
      } catch (error) {
        console.error('Error fetching bonus summary:', error);
        
        // Fallback to mock data if API call fails
        const fallbackBalance = user?.bonusBalance || 450;
        setBonusSummary({
          currentBalance: fallbackBalance,
          expiringNextQuarter: Math.floor(fallbackBalance * 0.2),
          recentTransactions: []
        });
        
        setLoading(false);
      }
    };
    
    fetchBonusSummary();
  }, [user]);
  
  // Prepare data for PieChart
  const preparePieData = () => {
    if (!bonusSummary) return [];
    
    return [
      { name: 'Available', value: bonusSummary.currentBalance - bonusSummary.expiringNextQuarter },
      { name: 'Expiring Soon', value: bonusSummary.expiringNextQuarter },
    ];
  };
  
  const COLORS = ['#0088FE', '#FF8042'];

  return (
    <Grid container spacing={3}>
      {/* Welcome Card */}
      <Grid item xs={12}>
        <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
          <Typography variant="h4" gutterBottom>
            Welcome, {userContext?.username || user?.username || 'User'}
          </Typography>
          <Typography variant="body1">
            Manage your bonus points and transactions from this dashboard.
          </Typography>
        </Paper>
      </Grid>
      
      {/* Current Balance */}
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Bonus Balance
            </Typography>
            <Divider />
            {loading ? (
              <Box sx={{ mt: 2 }}>
                <Skeleton variant="text" height={60} />
                <Skeleton variant="text" />
                <Skeleton variant="text" />
              </Box>
            ) : (
              <>
                <Typography variant="h3" sx={{ mt: 2, color: 'primary.main' }}>
                  {bonusSummary?.currentBalance?.toFixed(2) || 0}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Total Available Bonus Points
                </Typography>
                
                <Box sx={{ mt: 2 }}>
                  <Typography variant="body2" color="error">
                    {bonusSummary?.expiringNextQuarter?.toFixed(2) || 0} points expiring next quarter
                  </Typography>
                </Box>
              </>
            )}
          </CardContent>
        </Card>
      </Grid>
      
      {/* Bonus Distribution */}
      <Grid item xs={12} md={6}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Bonus Distribution
            </Typography>
            <Divider />
            {loading ? (
              <Box sx={{ mt: 2, height: 200 }}>
                <Skeleton variant="circular" width={200} height={200} sx={{ mx: 'auto' }} />
              </Box>
            ) : (
              <Box sx={{ height: 200, mt: 2 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={preparePieData()}
                      cx="50%"
                      cy="50%"
                      innerRadius={60}
                      outerRadius={80}
                      paddingAngle={5}
                      dataKey="value"
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                    >
                      {preparePieData().map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <Tooltip formatter={(value) => value.toFixed(2)} />
                  </PieChart>
                </ResponsiveContainer>
              </Box>
            )}
          </CardContent>
        </Card>
      </Grid>
      
      {/* Recent Transactions */}
      <Grid item xs={12}>
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Recent Transactions
            </Typography>
            <Divider />
            {loading ? (
              <Box sx={{ mt: 2 }}>
                <Skeleton variant="text" height={40} />
                <Skeleton variant="text" height={40} />
                <Skeleton variant="text" height={40} />
                <Skeleton variant="text" height={40} />
              </Box>
            ) : (
              <List>
                {bonusSummary?.recentTransactions?.length ? (
                  bonusSummary.recentTransactions.map((transaction) => (
                    <ListItem key={transaction.id} divider>
                      <ListItemText
                        primary={`${transaction.type === 'Earn' ? 'Earned' : 'Spent'} ${transaction.amount.toFixed(2)} points`}
                        secondary={`${new Date(transaction.timestamp).toLocaleDateString()} - ${transaction.description || 'No description'}`}
                      />
                      <Typography 
                        variant="body2" 
                        sx={{ 
                          color: transaction.type === 'Earn' ? 'success.main' : 'error.main',
                          fontWeight: 'bold'
                        }}
                      >
                        {transaction.type === 'Earn' ? '+' : '-'}{transaction.amount.toFixed(2)}
                      </Typography>
                    </ListItem>
                  ))
                ) : (
                  <ListItem>
                    <ListItemText primary="No recent transactions found" />
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

export default BuyerHome;
