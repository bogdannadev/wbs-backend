import React, { useState } from 'react';
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
  Button,
  Chip,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Tabs,
  Tab,
  IconButton,
  Tooltip,
  Avatar,
  Snackbar,
  Alert
} from '@mui/material';
import {
  Add as AddIcon,
  Delete as DeleteIcon,
  Edit as EditIcon,
  Block as BlockIcon,
  LockReset as LockResetIcon,
  Search as SearchIcon
} from '@mui/icons-material';
import { adminService } from '../../services/api';

const AdminUsers = () => {
  // State for users data and filtering
  const [users, setUsers] = useState([
    { id: 1, username: 'john.doe', email: 'john.doe@example.com', role: 'Buyer', bonusBalance: 450, status: 'Active', lastLogin: '2024-02-28', registeredAt: '2023-10-15' },
    { id: 2, username: 'jane.smith', email: 'jane.smith@example.com', role: 'Buyer', bonusBalance: 720, status: 'Active', lastLogin: '2024-03-02', registeredAt: '2023-11-05' },
    { id: 3, username: 'sales.rep1', email: 'sales.rep1@globalretail.com', role: 'Seller', bonusBalance: 0, status: 'Active', lastLogin: '2024-03-01', registeredAt: '2023-09-22' },
    { id: 4, username: 'store.manager', email: 'manager@metrogrocery.com', role: 'StoreAdmin', bonusBalance: 0, status: 'Active', lastLogin: '2024-03-02', registeredAt: '2023-08-10' },
    { id: 5, username: 'finance.observer', email: 'finance@techgadgets.com', role: 'CompanyObserver', bonusBalance: 0, status: 'Active', lastLogin: '2024-02-25', registeredAt: '2023-12-17' },
    { id: 6, username: 'blocked.user', email: 'blocked@example.com', role: 'Buyer', bonusBalance: 50, status: 'Blocked', lastLogin: '2024-01-15', registeredAt: '2023-07-20' },
  ]);
  
  // State for dialog and actions
  const [openNewUserDialog, setOpenNewUserDialog] = useState(false);
  const [selectedUser, setSelectedUser] = useState(null);
  const [newUser, setNewUser] = useState({
    username: '',
    email: '',
    role: '',
    initialBalance: '0'
  });
  
  // State for filtering and tabs
  const [searchQuery, setSearchQuery] = useState('');
  const [roleFilter, setRoleFilter] = useState('All');
  const [currentTab, setCurrentTab] = useState(0);
  
  // State for notifications
  const [notification, setNotification] = useState({ open: false, message: '', severity: 'success' });
  
  // Tab labels and corresponding status filters
  const tabs = ['All Users', 'Active', 'Blocked'];
  
  // Filter users based on search, role, and tab
  const filteredUsers = users.filter(user => {
    // Filter by search query
    const matchesSearch = 
      user.username.toLowerCase().includes(searchQuery.toLowerCase()) ||
      user.email.toLowerCase().includes(searchQuery.toLowerCase());
    
    // Filter by role
    const matchesRole = roleFilter === 'All' || user.role === roleFilter;
    
    // Filter by tab (status)
    const matchesTab = 
      (currentTab === 0) || // All users
      (currentTab === 1 && user.status === 'Active') || // Active users
      (currentTab === 2 && user.status === 'Blocked'); // Blocked users
    
    return matchesSearch && matchesRole && matchesTab;
  });
  
  // Open new user dialog
  const handleOpenNewUserDialog = () => {
    setOpenNewUserDialog(true);
  };
  
  // Close new user dialog
  const handleCloseNewUserDialog = () => {
    setOpenNewUserDialog(false);
    setNewUser({
      username: '',
      email: '',
      role: '',
      initialBalance: '0'
    });
  };
  
  // Add new user
  const handleAddUser = () => {
    // Form validation
    if (!newUser.username || !newUser.email || !newUser.role) {
      setNotification({
        open: true,
        message: 'Please fill in all required fields',
        severity: 'error'
      });
      return;
    }
    
    // In a real implementation, this would call the API
    const newId = Math.max(...users.map(u => u.id)) + 1;
    const balance = newUser.role === 'Buyer' ? parseFloat(newUser.initialBalance) : 0;
    
    const userToAdd = {
      id: newId,
      username: newUser.username,
      email: newUser.email,
      role: newUser.role,
      bonusBalance: balance,
      status: 'Active',
      lastLogin: '-',
      registeredAt: new Date().toISOString().split('T')[0]
    };
    
    setUsers([...users, userToAdd]);
    
    setNotification({
      open: true,
      message: 'User added successfully',
      severity: 'success'
    });
    
    handleCloseNewUserDialog();
  };
  
  // Change user status (block/unblock)
  const handleToggleUserStatus = (userId) => {
    const updatedUsers = users.map(user => {
      if (user.id === userId) {
        const newStatus = user.status === 'Active' ? 'Blocked' : 'Active';
        return {
          ...user,
          status: newStatus
        };
      }
      return user;
    });
    
    setUsers(updatedUsers);
    
    const user = users.find(u => u.id === userId);
    const action = user.status === 'Active' ? 'blocked' : 'unblocked';
    
    setNotification({
      open: true,
      message: `User ${user.username} has been ${action}`,
      severity: 'success'
    });
  };
  
  // Reset password (mock function)
  const handleResetPassword = (userId) => {
    const user = users.find(u => u.id === userId);
    
    setNotification({
      open: true,
      message: `Password reset link sent to ${user.email}`,
      severity: 'success'
    });
  };
  
  // Handle tab change
  const handleTabChange = (event, newValue) => {
    setCurrentTab(newValue);
  };
  
  // Close notification
  const handleCloseNotification = () => {
    setNotification({ ...notification, open: false });
  };
  
  return (
    <Box>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <Typography variant="h5" gutterBottom>
            User Management
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleOpenNewUserDialog}
          >
            Add User
          </Button>
        </Box>
        <Typography variant="body1">
          Manage system users, assign roles, and control access.
        </Typography>
      </Paper>
      
      <Card sx={{ mb: 3 }}>
        <Tabs
          value={currentTab}
          onChange={handleTabChange}
          indicatorColor="primary"
          textColor="primary"
          variant="fullWidth"
        >
          {tabs.map((tab, index) => (
            <Tab key={index} label={tab} />
          ))}
        </Tabs>
      </Card>
      
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
            <TextField
              label="Search Users"
              variant="outlined"
              size="small"
              fullWidth
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              InputProps={{
                startAdornment: <SearchIcon color="action" sx={{ mr: 1 }} />,
              }}
            />
            <FormControl variant="outlined" size="small" sx={{ minWidth: 150 }}>
              <InputLabel id="role-filter-label">Role</InputLabel>
              <Select
                labelId="role-filter-label"
                id="role-filter"
                value={roleFilter}
                onChange={(e) => setRoleFilter(e.target.value)}
                label="Role"
              >
                <MenuItem value="All">All Roles</MenuItem>
                <MenuItem value="Buyer">Buyers</MenuItem>
                <MenuItem value="Seller">Sellers</MenuItem>
                <MenuItem value="StoreAdmin">Store Admins</MenuItem>
                <MenuItem value="SystemAdmin">System Admins</MenuItem>
                <MenuItem value="CompanyObserver">Company Observers</MenuItem>
                <MenuItem value="SystemObserver">System Observers</MenuItem>
              </Select>
            </FormControl>
          </Box>
          
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>User</TableCell>
                  <TableCell>Role</TableCell>
                  <TableCell>Bonus Balance</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Last Login</TableCell>
                  <TableCell>Registered On</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {filteredUsers.length > 0 ? (
                  filteredUsers.map(user => (
                    <TableRow key={user.id} hover>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          <Avatar sx={{ mr: 2, bgcolor: user.status === 'Blocked' ? 'grey.500' : 'primary.main' }}>
                            {user.username.charAt(0).toUpperCase()}
                          </Avatar>
                          <Box>
                            <Typography variant="body2" fontWeight="medium">
                              {user.username}
                            </Typography>
                            <Typography variant="caption" color="text.secondary">
                              {user.email}
                            </Typography>
                          </Box>
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Chip 
                          label={user.role} 
                          color={
                            user.role === 'Buyer' ? 'primary' :
                            user.role === 'Seller' ? 'secondary' :
                            user.role === 'SystemAdmin' ? 'error' :
                            'default'
                          }
                          size="small"
                        />
                      </TableCell>
                      <TableCell>
                        {user.role === 'Buyer' ? `${user.bonusBalance} points` : '-'}
                      </TableCell>
                      <TableCell>
                        <Chip 
                          label={user.status} 
                          color={user.status === 'Active' ? 'success' : 'error'}
                          size="small"
                        />
                      </TableCell>
                      <TableCell>{user.lastLogin}</TableCell>
                      <TableCell>{user.registeredAt}</TableCell>
                      <TableCell>
                        <Box sx={{ display: 'flex' }}>
                          <Tooltip title={user.status === 'Active' ? 'Block User' : 'Unblock User'}>
                            <IconButton 
                              size="small" 
                              color={user.status === 'Active' ? 'error' : 'success'}
                              onClick={() => handleToggleUserStatus(user.id)}
                            >
                              <BlockIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                          <Tooltip title="Reset Password">
                            <IconButton 
                              size="small" 
                              color="primary" 
                              onClick={() => handleResetPassword(user.id)}
                            >
                              <LockResetIcon fontSize="small" />
                            </IconButton>
                          </Tooltip>
                        </Box>
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={7} align="center">
                      <Typography variant="body2" color="text.secondary" sx={{ py: 2 }}>
                        No users found matching the current filters.
                      </Typography>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        </CardContent>
      </Card>
      
      {/* Add User Dialog */}
      <Dialog open={openNewUserDialog} onClose={handleCloseNewUserDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Add New User</DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              autoFocus
              margin="dense"
              label="Username"
              fullWidth
              variant="outlined"
              value={newUser.username}
              onChange={(e) => setNewUser({ ...newUser, username: e.target.value })}
              sx={{ mb: 2 }}
            />
            <TextField
              margin="dense"
              label="Email Address"
              type="email"
              fullWidth
              variant="outlined"
              value={newUser.email}
              onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
              sx={{ mb: 2 }}
            />
            <FormControl fullWidth margin="dense" sx={{ mb: 2 }}>
              <InputLabel id="role-select-label">Role</InputLabel>
              <Select
                labelId="role-select-label"
                id="role-select"
                value={newUser.role}
                label="Role"
                onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
              >
                <MenuItem value="Buyer">Buyer</MenuItem>
                <MenuItem value="Seller">Seller</MenuItem>
                <MenuItem value="StoreAdmin">Store Admin</MenuItem>
                <MenuItem value="CompanyObserver">Company Observer</MenuItem>
                <MenuItem value="SystemObserver">System Observer</MenuItem>
              </Select>
            </FormControl>
            {newUser.role === 'Buyer' && (
              <TextField
                margin="dense"
                label="Initial Bonus Balance"
                type="number"
                fullWidth
                variant="outlined"
                value={newUser.initialBalance}
                onChange={(e) => setNewUser({ ...newUser, initialBalance: e.target.value })}
                sx={{ mb: 2 }}
              />
            )}
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseNewUserDialog}>Cancel</Button>
          <Button onClick={handleAddUser} variant="contained">Add User</Button>
        </DialogActions>
      </Dialog>
      
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

export default AdminUsers;
