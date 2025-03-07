import React, { useState, useEffect } from 'react';
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
  Chip,
  Divider,
  Grid,
  Button,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  CircularProgress
} from '@mui/material';
import { 
  AreaChart,
  Area,
  BarChart, 
  Bar, 
  XAxis, 
  YAxis, 
  CartesianGrid, 
  Tooltip, 
  Legend, 
  ResponsiveContainer
} from 'recharts';
import { observerService } from '../../services/api';

const ObserverCompanies = () => {
  const [companies, setCompanies] = useState([
    { 
      id: 1, 
      name: 'Global Retail Inc.', 
      status: 'Active', 
      bonusBalance: 250000, 
      originalBonusBalance: 200000,
      transactionVolume: 1250000,
      storeCount: 12,
      userCount: 45000
    },
    { 
      id: 2, 
      name: 'Metro Grocery Chain', 
      status: 'Active', 
      bonusBalance: 180000, 
      originalBonusBalance: 180000,
      transactionVolume: 980000,
      storeCount: 8,
      userCount: 32000
    },
    { 
      id: 3, 
      name: 'Fashion Outlets Ltd', 
      status: 'Active', 
      bonusBalance: 120000, 
      originalBonusBalance: 100000,
      transactionVolume: 750000,
      storeCount: 5,
      userCount: 27000
    },
    { 
      id: 4, 
      name: 'Tech Gadgets Corp', 
      status: 'Suspended', 
      bonusBalance: 45000, 
      originalBonusBalance: 75000,
      transactionVolume: 320000,
      storeCount: 3,
      userCount: 12000
    },
  ]);
  
  const [selectedCompany, setSelectedCompany] = useState(null);
  const [timeRange, setTimeRange] = useState('last6months');
  const [loading, setLoading] = useState(false);
  
  // Mock monthly transaction data for company details
  const generateMockTransactionData = (companyId) => {
    const multiplier = companyId * 0.4; // Generate different data for each company
    return [
      { month: 'Jan', earned: 4000 * multiplier, spent: 2400 * multiplier },
      { month: 'Feb', earned: 3000 * multiplier, spent: 1398 * multiplier },
      { month: 'Mar', earned: 2000 * multiplier, spent: 9800 * multiplier },
      { month: 'Apr', earned: 2780 * multiplier, spent: 3908 * multiplier },
      { month: 'May', earned: 1890 * multiplier, spent: 4800 * multiplier },
      { month: 'Jun', earned: 2390 * multiplier, spent: 3800 * multiplier },
    ];
  };
  
  // Mock user growth data
  const generateMockUserGrowthData = (companyId) => {
    const baseValue = companyId * 3000;
    const growth = 1.1 + (companyId * 0.05); // Different growth rate per company
    return [
      { month: 'Jan', users: baseValue },
      { month: 'Feb', users: Math.round(baseValue * growth) },
      { month: 'Mar', users: Math.round(baseValue * growth * growth) },
      { month: 'Apr', users: Math.round(baseValue * growth * growth * growth) },
      { month: 'May', users: Math.round(baseValue * growth * growth * growth * growth) },
      { month: 'Jun', users: Math.round(baseValue * growth * growth * growth * growth * growth) },
    ];
  };
  
  const handleCompanySelect = (companyId) => {
    // Find the selected company
    const company = companies.find(c => c.id === companyId);
    
    // Add transaction and user data
    const companyWithData = {
      ...company,
      transactionData: generateMockTransactionData(companyId),
      userGrowthData: generateMockUserGrowthData(companyId)
    };
    
    setSelectedCompany(companyWithData);
  };
  
  const handleTimeRangeChange = (event) => {
    setTimeRange(event.target.value);
  };
  
  // Dashboard for all companies (shown when no company is selected)
  const CompaniesDashboard = () => (
    <>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Typography variant="h5" gutterBottom>
          Company Analytics
        </Typography>
        <Typography variant="body1">
          View detailed performance metrics for each company in the bonus system.
        </Typography>
      </Paper>
      
      <Grid container spacing={3}>
        {/* Summary statistics */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Companies Overview
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Grid container spacing={2}>
                <Grid item xs={12} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {companies.length}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Active Companies
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {companies.reduce((sum, company) => sum + company.storeCount, 0)}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Total Stores
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {companies.reduce((sum, company) => sum + company.bonusBalance, 0).toLocaleString()}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Total Bonus Points
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {companies.reduce((sum, company) => sum + company.userCount, 0).toLocaleString()}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Total Users
                    </Typography>
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>
        
        {/* Companies table */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Companies Performance
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <TableContainer>
                <Table>
                  <TableHead>
                    <TableRow>
                      <TableCell>Company Name</TableCell>
                      <TableCell>Bonus Balance</TableCell>
                      <TableCell>Transaction Volume</TableCell>
                      <TableCell>Stores</TableCell>
                      <TableCell>Users</TableCell>
                      <TableCell>Status</TableCell>
                      <TableCell>Actions</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {companies.map((company) => (
                      <TableRow key={company.id} hover>
                        <TableCell>{company.name}</TableCell>
                        <TableCell>{company.bonusBalance.toLocaleString()}</TableCell>
                        <TableCell>{company.transactionVolume.toLocaleString()}</TableCell>
                        <TableCell>{company.storeCount}</TableCell>
                        <TableCell>{company.userCount.toLocaleString()}</TableCell>
                        <TableCell>
                          <Chip
                            label={company.status}
                            color={company.status === 'Active' ? 'success' : 'error'}
                            size="small"
                          />
                        </TableCell>
                        <TableCell>
                          <Button 
                            variant="outlined" 
                            size="small"
                            onClick={() => handleCompanySelect(company.id)}
                          >
                            View Details
                          </Button>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </>
  );
  
  // Company detail view (shown when a company is selected)
  const CompanyDetailView = ({ company }) => (
    <>
      <Paper elevation={2} sx={{ p: 3, mb: 3 }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
          <Typography variant="h5">
            {company.name}
          </Typography>
          <Button 
            variant="outlined" 
            size="small"
            onClick={() => setSelectedCompany(null)}
          >
            Back to All Companies
          </Button>
        </Box>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
          <Typography variant="body1">
            Status: 
          </Typography>
          <Chip
            label={company.status}
            color={company.status === 'Active' ? 'success' : 'error'}
          />
        </Box>
      </Paper>
      
      <Box sx={{ mb: 3 }}>
        <FormControl fullWidth size="small">
          <InputLabel id="time-range-label">Time Range</InputLabel>
          <Select
            labelId="time-range-label"
            id="time-range"
            value={timeRange}
            label="Time Range"
            onChange={handleTimeRangeChange}
          >
            <MenuItem value="last30days">Last 30 Days</MenuItem>
            <MenuItem value="last3months">Last 3 Months</MenuItem>
            <MenuItem value="last6months">Last 6 Months</MenuItem>
            <MenuItem value="lastyear">Last Year</MenuItem>
            <MenuItem value="alltime">All Time</MenuItem>
          </Select>
        </FormControl>
      </Box>
      
      <Grid container spacing={3}>
        {/* Summary statistics */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Company Overview
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Grid container spacing={2}>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {company.bonusBalance.toLocaleString()}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Current Bonus Balance
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {company.transactionVolume.toLocaleString()}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Transaction Volume
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {company.storeCount}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Total Stores
                    </Typography>
                  </Box>
                </Grid>
                <Grid item xs={12} sm={6} md={3}>
                  <Box sx={{ textAlign: 'center', p: 2 }}>
                    <Typography variant="h4" color="primary">
                      {company.userCount.toLocaleString()}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      Total Users
                    </Typography>
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>
        
        {/* Transaction Chart */}
        <Grid item xs={12} md={6}>
          <Card sx={{ height: '100%' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Bonus Points Transactions
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <BarChart
                    data={company.transactionData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="month" />
                    <YAxis />
                    <Tooltip formatter={(value) => value.toLocaleString()} />
                    <Legend />
                    <Bar dataKey="earned" name="Points Earned" fill="#8884d8" />
                    <Bar dataKey="spent" name="Points Spent" fill="#82ca9d" />
                  </BarChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        
        {/* User Growth Chart */}
        <Grid item xs={12} md={6}>
          <Card sx={{ height: '100%' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                User Growth
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Box sx={{ height: 300 }}>
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart
                    data={company.userGrowthData}
                    margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
                  >
                    <CartesianGrid strokeDasharray="3 3" />
                    <XAxis dataKey="month" />
                    <YAxis />
                    <Tooltip formatter={(value) => value.toLocaleString()} />
                    <Legend />
                    <Area type="monotone" dataKey="users" name="Active Users" fill="#8884d8" stroke="#8884d8" />
                  </AreaChart>
                </ResponsiveContainer>
              </Box>
            </CardContent>
          </Card>
        </Grid>
        
        {/* Store Performance */}
        <Grid item xs={12}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Store Performance
              </Typography>
              <Divider sx={{ mb: 2 }} />
              
              <Typography variant="body1" color="text.secondary" sx={{ textAlign: 'center', py: 4 }}>
                Detailed store performance metrics will be available in the full version.
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </>
  );
  
  return (
    <Box>
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', p: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          {selectedCompany ? (
            <CompanyDetailView company={selectedCompany} />
          ) : (
            <CompaniesDashboard />
          )}
        </>
      )}
    </Box>
  );
};

export default ObserverCompanies;
