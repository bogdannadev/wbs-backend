import axios from 'axios';

const API_URL = process.env.REACT_APP_API_URL || '';

// Helper function to determine if we're in demo mode
export const isDemoMode = () => {
  return true; // For prototype, we're always in demo mode
};

const api = axios.create({
  baseURL: API_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add JWT token to all requests
api.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Handle 401 unauthorized responses
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response && error.response.status === 401) {
      // Clear token and redirect to login page
      localStorage.removeItem('token');
      localStorage.removeItem('userId');
      localStorage.removeItem('role');
      window.location.href = '/';
    }
    return Promise.reject(error);
  }
);

// Authentication services
export const authService = {
  login: (credentials) => api.post('/auth/login', credentials),
  register: (userData) => api.post('/auth/register', userData),
};

// Buyer services
export const buyerService = {
  getContext: () => api.get('/api/buyers/context'),
  getBalance: () => api.get('/api/buyers/balance'),
  getTransactions: () => api.get('/api/buyers/transactions'),
  generateQrCode: () => api.get('/api/buyers/qrcode'),
  cancelTransaction: (id) => api.post(`/api/buyers/transactions/${id}/cancel`),
  findStores: (category) => api.get('/api/buyers/stores', { params: { category } }),
};

// Seller services
export const sellerService = {
  getContext: () => api.get('/api/sellers/context'),
  processTransaction: (transaction) => api.post('/api/sellers/transactions', transaction),
  confirmReturn: (id) => api.post(`/api/sellers/transactions/${id}/return`),
  getBuyerBalance: (buyerId) => api.get(`/api/sellers/buyers/${buyerId}/balance`),
  getStoreBalance: (storeId) => api.get(`/api/sellers/stores/${storeId}/balance`),
  getTransactions: (storeId) => api.get(`/api/sellers/stores/${storeId}/transactions`),
};

// Admin services
export const adminService = {
  getContext: () => api.get('/api/admin/context'),
  registerCompany: (company) => api.post('/api/admin/companies', company),
  updateCompanyStatus: (id, status) => api.put(`/api/admin/companies/${id}/status`, { status }),
  moderateStore: (id, approve) => api.put(`/api/admin/stores/${id}/moderate`, { approved: approve }),
  creditCompany: (id, amount) => api.post(`/api/admin/companies/${id}/credit`, { amount }),
  getTransactions: (params) => api.get('/api/admin/transactions', { params }),
  sendNotification: (data) => api.post('/api/admin/notifications', data),
};

// Observer services
export const observerService = {
  getContext: () => api.get('/api/observers/context'),
  getStatistics: (params) => api.get('/api/observers/statistics', { params }),
  getTransactionSummary: (companyId) => api.get('/api/observers/transactions/summary', { params: { companyId } }),
  getCompaniesOverview: () => api.get('/api/observers/companies'),
};

export default api;
