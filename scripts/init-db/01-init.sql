-- Create bonus schema
CREATE SCHEMA IF NOT EXISTS bonus;

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create users table
CREATE TABLE IF NOT EXISTS bonus.users (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    auth_id UUID NOT NULL,
    username VARCHAR(100) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    role VARCHAR(20) NOT NULL,
    bonus_balance DECIMAL(15, 2) NOT NULL DEFAULT 0,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    last_login TIMESTAMP WITH TIME ZONE,
    is_active BOOLEAN NOT NULL DEFAULT TRUE
);

-- Create companies table
CREATE TABLE IF NOT EXISTS bonus.companies (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    name VARCHAR(255) NOT NULL,
    contact_email VARCHAR(255) NOT NULL,
    contact_phone VARCHAR(50),
    bonus_balance DECIMAL(15, 2) NOT NULL DEFAULT 0,
    original_bonus_balance DECIMAL(15, 2) NOT NULL DEFAULT 0,
    status VARCHAR(20) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE
);

-- Create stores table
CREATE TABLE IF NOT EXISTS bonus.stores (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    company_id UUID NOT NULL REFERENCES bonus.companies(id),
    name VARCHAR(255) NOT NULL,
    location VARCHAR(255) NOT NULL,
    address VARCHAR(500) NOT NULL,
    contact_phone VARCHAR(50),
    status VARCHAR(20) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE
);

-- Create transactions table
CREATE TABLE IF NOT EXISTS bonus.transactions (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    user_id UUID REFERENCES bonus.users(id),
    company_id UUID REFERENCES bonus.companies(id),
    store_id UUID REFERENCES bonus.stores(id),
    amount DECIMAL(15, 2) NOT NULL,
    type VARCHAR(20) NOT NULL,
    timestamp TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    status VARCHAR(20) NOT NULL,
    description TEXT
);

-- Create notifications table
CREATE TABLE IF NOT EXISTS bonus.notifications (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    recipient_id UUID NOT NULL REFERENCES bonus.users(id),
    message TEXT NOT NULL,
    type VARCHAR(20) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_read BOOLEAN NOT NULL DEFAULT FALSE
);

-- Create store_seller_assignments table
CREATE TABLE IF NOT EXISTS bonus.store_seller_assignments (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    store_id UUID NOT NULL REFERENCES bonus.stores(id),
    user_id UUID NOT NULL REFERENCES bonus.users(id),
    assigned_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(store_id, user_id)
);

-- Insert demo data for users
INSERT INTO bonus.users (id, auth_id, username, email, role, bonus_balance) VALUES
    ('11111111-1111-1111-1111-111111111111', uuid_generate_v4(), 'buyer1', 'buyer1@example.com', 'Buyer', 150),
    ('22222222-2222-2222-2222-222222222222', uuid_generate_v4(), 'seller1', 'seller1@example.com', 'Seller', 0),
    ('33333333-3333-3333-3333-333333333333', uuid_generate_v4(), 'admin1', 'admin1@example.com', 'SystemAdmin', 0),
    ('44444444-4444-4444-4444-444444444444', uuid_generate_v4(), 'observer1', 'observer1@example.com', 'CompanyObserver', 0)
ON CONFLICT DO NOTHING;

-- Insert demo data for companies
INSERT INTO bonus.companies (id, name, contact_email, contact_phone, bonus_balance, original_bonus_balance, status) VALUES
    (uuid_generate_v4(), 'Global Retail Inc.', 'contact@globalretail.com', '555-123-4567', 250000, 250000, 'Active'),
    (uuid_generate_v4(), 'Metro Grocery Chain', 'info@metrogrocery.com', '555-987-6543', 180000, 180000, 'Active'),
    (uuid_generate_v4(), 'Fashion Outlets Ltd', 'support@fashionoutlets.com', '555-456-7890', 0, 0, 'Pending'),
    (uuid_generate_v4(), 'Tech Gadgets Corp', 'info@techgadgets.com', '555-789-0123', 45000, 50000, 'Active')
ON CONFLICT DO NOTHING;
