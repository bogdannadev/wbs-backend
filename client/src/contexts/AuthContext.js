import React, { createContext, useState, useContext, useEffect } from 'react';
import { authService } from '../services/api';
import { useNavigate } from 'react-router-dom';

// Demo accounts data for mocking the authentication
const demoAccounts = [
  {
    email: 'buyer@example.com',
    password: 'demo123',
    userId: '11111111-1111-1111-1111-111111111111',
    role: 'Buyer',
    username: 'john.buyer',
    bonusBalance: 450
  },
  {
    email: 'seller@example.com',
    password: 'demo123',
    userId: '22222222-2222-2222-2222-222222222222',
    role: 'Seller',
    username: 'sarah.seller',
    selectedStore: {
      id: 2,
      name: 'Global Retail - Uptown',
      company: 'Global Retail Inc.'
    }
  },
  {
    email: 'admin@example.com',
    password: 'demo123',
    userId: '33333333-3333-3333-3333-333333333333',
    role: 'SystemAdmin',
    username: 'admin.user'
  },
  {
    email: 'observer@example.com',
    password: 'demo123',
    userId: '44444444-4444-4444-4444-444444444444',
    role: 'CompanyObserver',
    username: 'observer.user'
  }
];

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Check if user is already logged in
  useEffect(() => {
    const token = localStorage.getItem('token');
    const userId = localStorage.getItem('userId');
    const role = localStorage.getItem('role');
    const username = localStorage.getItem('username');

    if (token && userId && role) {
      setUser({
        id: userId,
        role,
        username: username || 'User',
        // Add additional user data if available in localStorage
        ...(role === 'Buyer' && { 
          bonusBalance: parseInt(localStorage.getItem('bonusBalance') || '0') 
        }),
        ...(role === 'Seller' && { 
          selectedStore: JSON.parse(localStorage.getItem('selectedStore') || 'null')
        })
      });
    }
    
    setLoading(false);
  }, []);

  // Login method with demo account support
  const login = async (email, password) => {
    try {
      setLoading(true);
      
      // Check for demo accounts first
      const demoAccount = demoAccounts.find(
        account => account.email === email && account.password === password
      );
      
      if (demoAccount) {
        // Handle demo account login
        const { userId, role, username, bonusBalance, selectedStore } = demoAccount;
        
        // Mock token for demo purposes
        const token = `demo-token-${role.toLowerCase()}-${Date.now()}`;
        
        localStorage.setItem('token', token);
        localStorage.setItem('userId', userId);
        localStorage.setItem('role', role);
        localStorage.setItem('username', username);
        
        // Store additional role-specific data
        if (role === 'Buyer' && bonusBalance !== undefined) {
          localStorage.setItem('bonusBalance', bonusBalance.toString());
        }
        
        if (role === 'Seller' && selectedStore) {
          localStorage.setItem('selectedStore', JSON.stringify(selectedStore));
        }
        
        setUser({
          id: userId,
          role,
          username,
          ...(role === 'Buyer' && { bonusBalance }),
          ...(role === 'Seller' && { selectedStore })
        });
        
        // Redirect based on role
        navigateByRole(role);
        
        setLoading(false);
        return true;
      }
      
      // If not a demo account, try the real API
      const response = await authService.login({ email, password });
      const { userId, token, role } = response.data;
      
      localStorage.setItem('token', token);
      localStorage.setItem('userId', userId);
      localStorage.setItem('role', role);
      
      setUser({
        id: userId,
        role,
      });
      
      // Redirect based on role
      navigateByRole(role);
      
      setLoading(false);
      return true;
    } catch (error) {
      console.error('Login error:', error);
      setLoading(false);
      return false;
    }
  };

  // Register method
  const register = async (userData) => {
    try {
      setLoading(true);
      const response = await authService.register(userData);
      const { userId, token, role } = response.data;
      
      localStorage.setItem('token', token);
      localStorage.setItem('userId', userId);
      localStorage.setItem('role', role);
      
      setUser({
        id: userId,
        role,
      });
      
      // Redirect based on role
      navigateByRole(role);
      
      setLoading(false);
      return true;
    } catch (error) {
      console.error('Registration error:', error);
      setLoading(false);
      return false;
    }
  };

  // Logout method
  const logout = () => {
    // Clear all auth-related data from localStorage
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('role');
    localStorage.removeItem('username');
    localStorage.removeItem('bonusBalance');
    localStorage.removeItem('selectedStore');
    
    setUser(null);
    navigate('/');
  };

  // Helper function to navigate based on role
  const navigateByRole = (role) => {
    switch (role) {
      case 'Buyer':
        navigate('/buyer');
        break;
      case 'Seller':
        navigate('/seller');
        break;
      case 'SystemAdmin':
        navigate('/admin');
        break;
      case 'CompanyObserver':
      case 'SystemObserver':
        navigate('/observer');
        break;
      default:
        navigate('/');
    }
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        login,
        register,
        logout,
        loading,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
