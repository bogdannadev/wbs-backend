# BonusSystem Frontend

This is the React-based frontend for the BonusSystem prototype.

## Overview

BonusSystem is a comprehensive bonus tracking and management platform designed for companies and their customers. The system allows:

- **Buyers** to earn and spend bonus points at participating stores
- **Sellers** to process bonus point transactions
- **Administrators** to manage companies, stores, and users
- **Observers** to analyze system performance and metrics

## Technology Stack

- React 18
- Material UI
- React Router for navigation
- Axios for API communication
- Recharts for data visualization
- QR Code generation/scanning

## Getting Started

### Prerequisites

- Node.js 16+ and npm

### Installation

1. Clone the repository
2. Navigate to the client directory
```
cd client
```
3. Install dependencies
```
npm install
```
4. Start the development server
```
npm start
```

The application will run at [http://localhost:3000](http://localhost:3000)

## Demo Accounts

The application comes with pre-configured demo accounts for each role:

| Role | Email | Password | Description |
|------|-------|----------|-------------|
| Buyer | buyer@example.com | demo123 | Regular user who earns and spends bonus points |
| Seller | seller@example.com | demo123 | Retail employee who processes bonus transactions |
| Admin | admin@example.com | demo123 | Administrator who manages the entire system |
| Observer | observer@example.com | demo123 | Analytics user who monitors system performance |

> **Note:** These accounts are for demonstration purposes only and are not connected to a real backend.

## Demo Flows

### Buyer Flow
1. Login as the Buyer
2. See your current bonus balance on the dashboard
3. View transaction history
4. Generate a QR code to use at stores
5. Find participating stores

### Seller Flow
1. Login as the Seller
2. Select a store location
3. Process a new transaction (scan QR code, enter amount)
4. View transaction history
5. Process a return if needed

### Admin Flow
1. Login as the Admin
2. View system overview on the dashboard
3. Manage companies
4. Manage users
5. Approve/reject store registrations

### Observer Flow
1. Login as the Observer
2. View system analytics
3. Explore company performance
4. Analyze transaction data

## Development Notes

### API Integration

For the prototype, the application uses mock data and simulated API calls. In production, you would connect to the actual backend API:

1. Update the API base URL in `/src/services/api.js`
2. Remove mock data and enable the real API calls

### Authentication

The prototype uses simulated JWT authentication. For production:

1. Update the `AuthContext.js` to use the real JWT implementation
2. Remove the demo account special handling

## Folder Structure

- `/src/components` - Reusable UI components
- `/src/contexts` - React contexts (auth, etc.)
- `/src/pages` - Main page components by role
- `/src/services` - API services
- `/src/utils` - Utility functions

## License

This project is proprietary and confidential.
