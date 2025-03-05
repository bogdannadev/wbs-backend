-- Seed Data for BonusSystem Database

-- Set search path to our schema
SET search_path TO bonus, public;

-- Seed Users
INSERT INTO bonus.users (id, username, email, role, bonus_balance, created_at)
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'buyer1', 'buyer1@example.com', 'Buyer', 150, CURRENT_TIMESTAMP - INTERVAL '30 days'),
    ('22222222-2222-2222-2222-222222222222', 'seller1', 'seller1@example.com', 'Seller', 0, CURRENT_TIMESTAMP - INTERVAL '30 days'),
    ('33333333-3333-3333-3333-333333333333', 'admin1', 'admin1@example.com', 'SystemAdmin', 0, CURRENT_TIMESTAMP - INTERVAL '30 days')
ON CONFLICT (id) DO NOTHING;

-- Seed Companies
INSERT INTO bonus.companies (id, name, contact_email, contact_phone, bonus_balance, original_bonus_balance, status, created_at)
VALUES 
    ('44444444-4444-4444-4444-444444444444', 'Alpha Store Chain', 'contact@alphastore.com', '+1234567890', 1000000, 1000000, 'Active', CURRENT_TIMESTAMP - INTERVAL '30 days'),
    ('55555555-5555-5555-5555-555555555555', 'Beta Retail Group', 'info@betaretail.com', '+1987654321', 500000, 500000, 'Active', CURRENT_TIMESTAMP - INTERVAL '15 days'),
    ('66666666-6666-6666-6666-666666666666', 'Gamma Markets', 'support@gammamarkets.com', '+1122334455', 250000, 250000, 'Pending', CURRENT_TIMESTAMP - INTERVAL '5 days')
ON CONFLICT (id) DO NOTHING;

-- Seed Stores
INSERT INTO bonus.stores (id, company_id, name, location, address, contact_phone, status, created_at)
VALUES 
    ('77777777-7777-7777-7777-777777777777', '44444444-4444-4444-4444-444444444444', 'Alpha Downtown', 'Downtown', '123 Main St, Downtown', '+1234567890', 'Active', CURRENT_TIMESTAMP - INTERVAL '25 days'),
    ('88888888-8888-8888-8888-888888888888', '44444444-4444-4444-4444-444444444444', 'Alpha Uptown', 'Uptown', '456 Park Ave, Uptown', '+1234567891', 'Active', CURRENT_TIMESTAMP - INTERVAL '20 days'),
    ('99999999-9999-9999-9999-999999999999', '55555555-5555-5555-5555-555555555555', 'Beta Central', 'Midtown', '789 Center Blvd, Midtown', '+1987654320', 'Active', CURRENT_TIMESTAMP - INTERVAL '15 days'),
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', '66666666-6666-6666-6666-666666666666', 'Gamma Express', 'Riverside', '321 River Rd, Riverside', '+1122334450', 'PendingApproval', CURRENT_TIMESTAMP - INTERVAL '3 days')
ON CONFLICT (id) DO NOTHING;

-- Seed Store Seller Assignments
INSERT INTO bonus.store_seller_assignments (store_id, user_id)
VALUES ('77777777-7777-7777-7777-777777777777', '22222222-2222-2222-2222-222222222222')
ON CONFLICT (store_id, user_id) DO NOTHING;

-- Seed Transactions
INSERT INTO bonus.transactions (id, user_id, company_id, store_id, amount, type, timestamp, status, description)
VALUES 
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444444', '77777777-7777-7777-7777-777777777777', 100, 'Earn', CURRENT_TIMESTAMP - INTERVAL '10 days', 'Completed', 'Purchase at Alpha Downtown Store'),
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', '11111111-1111-1111-1111-111111111111', '44444444-4444-4444-4444-444444444444', '77777777-7777-7777-7777-777777777777', 50, 'Spend', CURRENT_TIMESTAMP - INTERVAL '5 days', 'Completed', 'Redemption at Alpha Downtown Store'),
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', '11111111-1111-1111-1111-111111111111', '55555555-5555-5555-5555-555555555555', '99999999-9999-9999-9999-999999999999', 75, 'Earn', CURRENT_TIMESTAMP - INTERVAL '2 days', 'Completed', 'Purchase at Beta Central Store')
ON CONFLICT (id) DO NOTHING;

-- Seed Notifications
INSERT INTO bonus.notifications (recipient_id, message, type, created_at, is_read)
VALUES 
    ('11111111-1111-1111-1111-111111111111', 'Welcome to BonusSystem! Earn bonuses with every purchase.', 'System', CURRENT_TIMESTAMP - INTERVAL '30 days', TRUE),
    ('11111111-1111-1111-1111-111111111111', 'You''ve earned 100 bonus points from your purchase at Alpha Downtown.', 'Transaction', CURRENT_TIMESTAMP - INTERVAL '10 days', TRUE),
    ('11111111-1111-1111-1111-111111111111', 'You''ve spent 50 bonus points at Alpha Downtown.', 'Transaction', CURRENT_TIMESTAMP - INTERVAL '5 days', FALSE),
    ('22222222-2222-2222-2222-222222222222', 'Welcome to BonusSystem! You''re now a seller at Alpha Downtown.', 'System', CURRENT_TIMESTAMP - INTERVAL '30 days', TRUE),
    ('22222222-2222-2222-2222-222222222222', 'Transaction processed: 100 bonus points awarded to a customer.', 'Transaction', CURRENT_TIMESTAMP - INTERVAL '10 days', TRUE),
    ('33333333-3333-3333-3333-333333333333', 'Welcome to BonusSystem Admin Portal!', 'System', CURRENT_TIMESTAMP - INTERVAL '30 days', TRUE),
    ('33333333-3333-3333-3333-333333333333', 'New store pending approval: Gamma Express', 'AdminMessage', CURRENT_TIMESTAMP - INTERVAL '5 days', FALSE);
