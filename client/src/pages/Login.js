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
  Paper
} from '@mui/material';
import { useAuth } from '../contexts/AuthContext';

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

  const handleDemoLogin = async (demoType) => {
    setLoading(true);
    setError('');

    let demoEmail = '';
    let demoPassword = 'Password123!';

    switch (demoType) {
      case 'buyer':
        demoEmail = 'buyer1@example.com';
        break;
      case 'seller':
        demoEmail = 'seller1@example.com';
        break;
      case 'admin':
        demoEmail = 'admin1@example.com';
        break;
      case 'observer':
        demoEmail = 'observer1@example.com';
        break;
      default:
        demoEmail = 'buyer1@example.com';
    }

    try {
      const success = await login(demoEmail, demoPassword);
      if (!success) {
        setError(`Demo login failed for ${demoType} account`);
      }
    } catch (error) {
      setError('An error occurred during demo login');
      console.error('Demo login error:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <Container component="main" maxWidth="md">
      <Box
        sx={{
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
          justifyContent: 'center',
          minHeight: '100vh',
        }}
      >
        <Paper elevation={3} sx={{ width: '100%', p: 4, borderRadius: 2 }}>
          <Typography variant="h4" component="h1" align="center" gutterBottom>
            BonusSystem
          </Typography>
          <Typography variant="h6" align="center" color="textSecondary" gutterBottom>
            Bonus Tracking and Management Platform
          </Typography>

          <Box sx={{ mt: 4, display: 'flex', flexDirection: { xs: 'column', md: 'row' }, gap: 4 }}>
            {/* Login Form */}
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Login
                </Typography>
                <Box component="form" onSubmit={handleSubmit} sx={{ mt: 2 }}>
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

            {/* Demo Accounts */}
            <Card sx={{ flex: 1 }}>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Demo Accounts
                </Typography>
                <Typography variant="body2" color="textSecondary" paragraph>
                  Use these demo accounts to explore different user roles
                </Typography>
                <Box sx={{ display: 'flex', flexDirection: 'column', gap: 2, mt: 2 }}>
                  <Button 
                    variant="outlined" 
                    color="primary" 
                    fullWidth
                    onClick={() => handleDemoLogin('buyer')}
                    disabled={loading}
                  >
                    Login as Buyer
                  </Button>
                  <Button 
                    variant="outlined" 
                    color="secondary" 
                    fullWidth
                    onClick={() => handleDemoLogin('seller')}
                    disabled={loading}
                  >
                    Login as Seller
                  </Button>
                  <Button 
                    variant="outlined" 
                    color="error" 
                    fullWidth
                    onClick={() => handleDemoLogin('admin')}
                    disabled={loading}
                  >
                    Login as Admin
                  </Button>
                  <Button 
                    variant="outlined" 
                    color="info" 
                    fullWidth
                    onClick={() => handleDemoLogin('observer')}
                    disabled={loading}
                  >
                    Login as Observer
                  </Button>
                </Box>
              </CardContent>
            </Card>
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
