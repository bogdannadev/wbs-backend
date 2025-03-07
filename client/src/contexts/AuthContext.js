import React, { createContext, useState, useContext, useEffect } from 'react';
import { authService } from '../services/api';
import { useNavigate } from 'react-router-dom';

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

    if (token && userId && role) {
      setUser({
        id: userId,
        role,
      });
    }
    
    setLoading(false);
  }, []);

  // Login method
  const login = async (email, password) => {
    try {
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
      
      return true;
    } catch (error) {
      console.error('Login error:', error);
      return false;
    }
  };

  // Register method
  const register = async (userData) => {
    try {
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
      
      return true;
    } catch (error) {
      console.error('Registration error:', error);
      return false;
    }
  };

  // Logout method
  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('userId');
    localStorage.removeItem('role');
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
