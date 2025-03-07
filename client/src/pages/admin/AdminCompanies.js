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
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Snackbar,
  Alert,
  Chip,
  IconButton,
  Tooltip,
  MenuItem,
  Menu,
  Select,
  FormControl,
  InputLabel,
  Grid
} from '@mui/material';
import {
  Add as AddIcon,
  Edit as EditIcon,
  MoreVert as MoreVertIcon,
  Delete as DeleteIcon,
  CreditCard as CreditCardIcon
} from '@mui/icons-material';
import { adminService } from '../../services/api';

const AdminCompanies = () => {
  // State for companies data
  const [companies, setCompanies] = useState([
    { id: 1, name: 'Global Retail Inc.', contactEmail: 'contact@globalretail.com', contactPhone: '(555) 123-4567', bonusBalance: 250000, originalBonusBalance: 200000, status: 'Active', storeCount: 12, createdAt: '2024-01-15' },
    { id: 2, name: 'Metro Grocery Chain', contactEmail: 'info@metrogrocery.com', contactPhone: '(555) 987-6543', bonusBalance: 180000, originalBonusBalance: 180000, status: 'Active', storeCount: 8, createdAt: '2024-01-20' },
    { id: 3, name: 'Fashion Outlets Ltd', contactEmail: 'support@fashionoutlets.com', contactPhone: '(555) 765-4321', bonusBalance: 0, originalBonusBalance: 0, status: 'Pending', storeCount: 3, createdAt: '2024-02-10' },
    { id: 4, name: 'Tech Gadgets Corp', contactEmail: 'help@techgadgets.com', contactPhone: '(555) 456-7890', bonusBalance: 45000, originalBonusBalance: 75000, status: 'Suspended', storeCount: 5, createdAt: '2024-02-05' },
  ]);
  
  // Dialog states
  const [openAddDialog, setOpenAddDialog] = useState(false);
  const [openCreditDialog, setOpenCreditDialog] = useState(false);
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [creditAmount, setCreditAmount] = useState('');
  const [newCompany, setNewCompany] = useState({
    name: '',
    contactEmail: '',
    contactPhone: '',
    initialBonusBalance: ''
  });
  
  // Menu state
  const [anchorEl, setAnchorEl] = useState(null);
  const [menuCompanyId, setMenuCompanyId] = useState(null);
  
  // Notification state
  const [notification, setNotification] = useState({ open: false, message: '', severity: 'success' });
  
  // Handle menu opening
  const handleMenuOpen = (event, companyId) => {
    setAnchorEl(event.currentTarget);
    setMenuCompanyId(companyId);
  };
  
  // Handle menu closing
  const handleMenuClose = () => {
    setAnchorEl(null);
    setMenuCompanyId(null);
  };
  
  // Open add company dialog
  const handleOpenAddDialog = () => {
    setOpenAddDialog(true);
  };
  
  // Close add company dialog
  const handleCloseAddDialog = () => {
    setOpenAddDialog(false);
    setNewCompany({
      name: '',
      contactEmail: '',
      contactPhone: '',
      initialBonusBalance: ''
    });
  };
  
  // Add new company
  const handleAddCompany = () => {
    // Form validation
    if (!newCompany.name || !newCompany.contactEmail || !newCompany.contactPhone) {
      setNotification({
        open: true,
        message: 'Please fill in all required fields',
        severity: 'error'
      });
      return;
    }
    
    // In a real implementation, this would call the API
    const newId = Math.max(...companies.map(c => c.id)) + 1;
    const balance = newCompany.initialBonusBalance ? parseFloat(newCompany.initialBonusBalance) : 0;
    
    const companyToAdd = {
      id: newId,
      name: newCompany.name,
      contactEmail: newCompany.contactEmail,
      contactPhone: newCompany.contactPhone,
      bonusBalance: balance,
      originalBonusBalance: balance,
      status: 'Pending',
      storeCount: 0,
      createdAt: new Date().toISOString().split('T')[0]
    };
    
    setCompanies([...companies, companyToAdd]);
    
    setNotification({
      open: true,
      message: 'Company added successfully',
      severity: 'success'
    });
    
    handleCloseAddDialog();
  };
  
  // Open credit dialog
  const handleOpenCreditDialog = (company) => {
    setSelectedCompany(company);
    setOpenCreditDialog(true);
    handleMenuClose();
  };
  
  // Close credit dialog
  const handleCloseCreditDialog = () => {
    setOpenCreditDialog(false);
    setSelectedCompany(null);
    setCreditAmount('');
  };
  
  // Credit bonus balance
  const handleCreditBalance = () => {
    // Validate input
    if (!creditAmount || isNaN(parseFloat(creditAmount))) {
      setNotification({
        open: true,
        message: 'Please enter a valid credit amount',
        severity: 'error'
      });
      return;
    }
    
    // In a real implementation, this would call the API
    const updatedCompanies = companies.map(company => {
      if (company.id === selectedCompany.id) {
        const newBalance = company.bonusBalance + parseFloat(creditAmount);
        const newOriginalBalance = company.originalBonusBalance + parseFloat(creditAmount);
        
        return {
          ...company,
          bonusBalance: newBalance,
          originalBonusBalance: newOriginalBalance
        };
      }
      return company;
    });
    
    setCompanies(updatedCompanies);
    
    setNotification({
      open: true,
      message: `Successfully credited ${creditAmount} bonus points to ${selectedCompany.name}`,
      severity: 'success'
    });
    
    handleCloseCreditDialog();
  };
  
  // Change company status
  const handleStatusChange = (companyId, newStatus) => {
    const updatedCompanies = companies.map(company => {
      if (company.id === companyId) {
        return {
          ...company,
          status: newStatus
        };
      }
      return company;
    });
    
    setCompanies(updatedCompanies);
    
    setNotification({
      open: true,
      message: `Company status updated to ${newStatus}`,
      severity: 'success'
    });
    
    handleMenuClose();
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
            Company Management
          </Typography>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleOpenAddDialog}
          >
            Add Company
          </Button>
        </Box>
        <Typography variant="body1">
          Manage companies, update their status, and adjust bonus balances.
        </Typography>
      </Paper>

      <Card>
        <CardContent>
          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>Company Name</TableCell>
                  <TableCell>Contact Information</TableCell>
                  <TableCell>Bonus Balance</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Stores</TableCell>
                  <TableCell>Joined</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {companies.map((company) => (
                  <TableRow key={company.id} hover>
                    <TableCell>{company.name}</TableCell>
                    <TableCell>
                      <Typography variant="body2">{company.contactEmail}</Typography>
                      <Typography variant="body2" color="textSecondary">{company.contactPhone}</Typography>
                    </TableCell>
                    <TableCell>
                      <Typography variant="body2">{company.bonusBalance.toLocaleString()} points</Typography>
                      <Typography variant="caption" color="textSecondary">
                        Original: {company.originalBonusBalance.toLocaleString()} points
                      </Typography>
                    </TableCell>
                    <TableCell>
                      <Chip
                        label={company.status}
                        color={
                          company.status === 'Active' ? 'success' :
                          company.status === 'Pending' ? 'warning' : 'error'
                        }
                        size="small"
                      />
                    </TableCell>
                    <TableCell>{company.storeCount}</TableCell>
                    <TableCell>{company.createdAt}</TableCell>
                    <TableCell>
                      <IconButton size="small" onClick={(e) => handleMenuOpen(e, company.id)}>
                        <MoreVertIcon />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          
          {/* Company actions menu */}
          <Menu
            anchorEl={anchorEl}
            open={Boolean(anchorEl)}
            onClose={handleMenuClose}
          >
            <MenuItem onClick={() => handleOpenCreditDialog(companies.find(c => c.id === menuCompanyId))}>
              <CreditCardIcon sx={{ mr: 1 }} fontSize="small" />
              Credit Balance
            </MenuItem>
            <MenuItem onClick={() => handleStatusChange(menuCompanyId, 'Active')}>
              <Chip label="Set Active" color="success" size="small" sx={{ mr: 1 }} />
            </MenuItem>
            <MenuItem onClick={() => handleStatusChange(menuCompanyId, 'Suspended')}>
              <Chip label="Set Suspended" color="error" size="small" sx={{ mr: 1 }} />
            </MenuItem>
          </Menu>
        </CardContent>
      </Card>
      
      {/* Add Company Dialog */}
      <Dialog open={openAddDialog} onClose={handleCloseAddDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Add New Company</DialogTitle>
        <DialogContent>
          <Box sx={{ pt: 1 }}>
            <TextField
              autoFocus
              margin="dense"
              label="Company Name"
              fullWidth
              variant="outlined"
              value={newCompany.name}
              onChange={(e) => setNewCompany({ ...newCompany, name: e.target.value })}
              sx={{ mb: 2 }}
            />
            <TextField
              margin="dense"
              label="Contact Email"
              type="email"
              fullWidth
              variant="outlined"
              value={newCompany.contactEmail}
              onChange={(e) => setNewCompany({ ...newCompany, contactEmail: e.target.value })}
              sx={{ mb: 2 }}
            />
            <TextField
              margin="dense"
              label="Contact Phone"
              fullWidth
              variant="outlined"
              value={newCompany.contactPhone}
              onChange={(e) => setNewCompany({ ...newCompany, contactPhone: e.target.value })}
              sx={{ mb: 2 }}
            />
            <TextField
              margin="dense"
              label="Initial Bonus Balance"
              type="number"
              fullWidth
              variant="outlined"
              value={newCompany.initialBonusBalance}
              onChange={(e) => setNewCompany({ ...newCompany, initialBonusBalance: e.target.value })}
              sx={{ mb: 2 }}
            />
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseAddDialog}>Cancel</Button>
          <Button onClick={handleAddCompany} variant="contained">Add Company</Button>
        </DialogActions>
      </Dialog>
      
      {/* Credit Balance Dialog */}
      <Dialog open={openCreditDialog} onClose={handleCloseCreditDialog} maxWidth="sm" fullWidth>
        <DialogTitle>Credit Bonus Balance</DialogTitle>
        <DialogContent>
          {selectedCompany && (
            <Box sx={{ pt: 1 }}>
              <Typography variant="subtitle1" gutterBottom>
                {selectedCompany.name}
              </Typography>
              <Typography variant="body2" gutterBottom>
                Current Balance: {selectedCompany.bonusBalance.toLocaleString()} points
              </Typography>
              <TextField
                autoFocus
                margin="dense"
                label="Credit Amount"
                type="number"
                fullWidth
                variant="outlined"
                value={creditAmount}
                onChange={(e) => setCreditAmount(e.target.value)}
                sx={{ mt: 2 }}
              />
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleCloseCreditDialog}>Cancel</Button>
          <Button onClick={handleCreditBalance} variant="contained">Credit Balance</Button>
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

export default AdminCompanies;
