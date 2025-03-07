import React, { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  TextField,
  Button,
  Snackbar,
  Alert,
  Card,
  CardContent,
  Divider,
  Paper,
  Grid,
  Avatar,
  Chip,
  List,
  ListItem,
  ListItemAvatar,
  ListItemText,
  ListItemButton
} from '@mui/material';
import {
  Person as PersonIcon,
  ShoppingCart as ShoppingCartIcon,
  Store as StoreIcon,
  AdminPanelSettings as AdminIcon,
  BarChart as ChartIcon
} from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';

// Demo accounts for quick login
const demoAccounts = [
  {
    id: 1,
    role: 'Buyer',
    username: 'john.buyer',
    email: 'buyer@example.com',
    password: 'demo123',
    avatar: <ShoppingCartIcon />,
    description: 'Regular user who earns and spends bonus points',
    color: 'primary.main'
  },
  {
    id: 2,
    role: 'Seller',
    username: 'sarah.seller',
    email: 'seller@example.com',
    password: 'demo123',
    avatar: <StoreIcon />,
    description: 'Retail employee who processes bonus transactions',
    color: 'secondary.main'
  },
  {
    id: 3,
    role: 'SystemAdmin',
    username: 'admin.user',
    email: 'admin@example.com',
    password: 'demo123',
    avatar: <AdminIcon />,
    description: 'Administrator who manages the entire bonus system',
    color: 'error.main'
  },
  {
    id: 4,
    role: 'CompanyObserver',
    username: 'observer.user',
    email: 'observer@example.com',
    password: 'demo123',
    avatar: <ChartIcon />,
    description: 'Analytics user who monitors system performance',
    color: 'info.main'
  }
];

const Login = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError('');

    try {
      const success = await login(email, password);
      if (!success) {
        setError('Invalid credentials');
      }
    } catch (error) {
      setError('An error occurred during login');
      console.error('Login error:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleDemoLogin = async (account) => {
    setLoading(true);
    setError('');
    
    try {
      // Pre-fill form fields with demo account details
      setEmail(account.email);
      setPassword(account.password);
      
      // Perform login
      const success = await login(account.email, account.password);
      if (!success) {
        setError(`Demo login failed for ${account.role} account`);
      }
    } catch (error) {
      setError('An error occurred during demo login');
      console.error('Demo login error:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container component="main" maxWidth="lg">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
          py: 4
        }}
      >
        <Paper elevation={3} sx={{ width: '100%', p: 4, borderRadius: 2 }}>
          <Box sx={{ textAlign: 'center', mb: 4 }}>
            <Typography variant="h3" component="h1" gutterBottom fontWeight="bold" color="primary">
              BonusSystem
            </Typography>
            <Typography variant="h6" color="text.secondary">
              Comprehensive Bonus Tracking and Management Platform
            </Typography>
            <Chip 
              label="Demo Version" 
              color="primary" 
              size="small" 
              sx={{ mt: 1 }} 
            />
          </Box>

          <Grid container spacing={4}>
            {/* Login Form */}
            <Grid item xs={12} md={5}>
              <Card sx={{ height: '100%' }}>
                <CardContent>
                  <Typography variant="h5" gutterBottom>
                    Login
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                  
                  <Box component="form" onSubmit={handleSubmit}>
                    <TextField
                      margin="normal"
                      required
                      fullWidth
                      id="email"
                      label="Email Address"
                      name="email"
                      autoComplete="email"
                      autoFocus
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                    />
                    <TextField
                      margin="normal"
                      required
                      fullWidth
                      name="password"
                      label="Password"
                      type="password"
                      id="password"
                      autoComplete="current-password"
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                    />
                    <Button
                      type="submit"
                      fullWidth
                      variant="contained"
                      sx={{ mt: 3, mb: 2 }}
                      disabled={loading}
                    >
                      {loading ? 'Logging in...' : 'Login'}
                    </Button>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
            
            {/* Demo Accounts */}
            <Grid item xs={12} md={7}>
              <Card sx={{ height: '100%' }}>
                <CardContent>
                  <Typography variant="h5" gutterBottom>
                    Demo Accounts
                  </Typography>
                  <Divider sx={{ mb: 3 }} />
                  
                  <Typography variant="body2" paragraph>
                    Click on any of these demo accounts to quickly explore different user roles in the system:
                  </Typography>
                  
                  <List>
                    {demoAccounts.map(account => (
                      <ListItem 
                        key={account.id} 
                        disablePadding
                        sx={{ mb: 1 }}
                      >
                        <ListItemButton 
                          onClick={() => handleDemoLogin(account)}
                          sx={{ 
                            border: 1, 
                            borderColor: 'divider',
                            borderRadius: 1,
                            '&:hover': {
                              borderColor: account.color,
                              bgcolor: 'background.paper'
                            }
                          }}
                        >
                          <ListItemAvatar>
                            <Avatar sx={{ bgcolor: account.color }}>
                              {account.avatar}
                            </Avatar>
                          </ListItemAvatar>
                          <ListItemText 
                            primary={
                              <Box sx={{ display: 'flex', alignItems: 'center' }}>
                                <Typography variant="subtitle1">
                                  {account.username}
                                </Typography>
                                <Chip
                                  label={account.role}
                                  size="small"
                                  sx={{ ml: 1, bgcolor: account.color, color: 'white' }}
                                />
                              </Box>
                            }
                            secondary={account.description}
                          />
                        </ListItemButton>
                      </ListItem>
                    ))}
                  </List>
                  
                  <Box sx={{ mt: 2 }}>
                    <Alert severity="info">
                      <Typography variant="body2">
                        All demo accounts use password: <strong>demo123</strong>
                      </Typography>
                    </Alert>
                  </Box>
                </CardContent>
              </Card>
            </Grid>
          </Grid>
          
          <Box sx={{ mt: 4, textAlign: 'center' }}>
            <Typography variant="body2" color="text.secondary">
              BonusSystem Prototype - A bonus tracking and management platform designed for companies and their customers.
            </Typography>
          </Box>
        </Paper>

        <Snackbar open={!!error} autoHideDuration={6000} onClose={() => setError('')}>
          <Alert severity="error" onClose={() => setError('')}>
            {error}
          </Alert>
        </Snackbar>
      </Box>
    </Container>
  );
};

export default Login;
