#!/bin/bash

echo "Setting up BonusSystem Prototype Demo Environment"
echo "------------------------------------------------"

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "Docker is not installed. Please install Docker and Docker Compose first."
    exit 1
fi

# Check if Docker Compose is installed
if ! command -v docker-compose &> /dev/null; then
    echo "Docker Compose is not installed. Please install Docker Compose first."
    exit 1
fi

echo "Creating necessary directories..."
mkdir -p scripts/init-db

# Create initial PostgreSQL setup script
echo "Creating database initialization script..."
cat > scripts/init-db/01-init.sql << 'EOF'
-- Create bonus schema
CREATE SCHEMA IF NOT EXISTS bonus;

-- Create extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create tables and seed data will be handled by EF Core migrations
EOF

echo "Setting executable permissions..."
chmod +x setup-demo.sh

echo "Building Docker images and starting containers..."
docker-compose up -d --build

echo "Waiting for services to start..."
sleep 5

echo "------------------------------------------------"
echo "BonusSystem Demo is now running!"
echo ""
echo "Frontend: http://localhost:3000"
echo "API: http://localhost:5001/api-docs"
echo "pgAdmin: http://localhost:5050"
echo ""
echo "Demo Accounts:"
echo "Buyer: buyer1@example.com / Password123!"
echo "Seller: seller1@example.com / Password123!"
echo "Admin: admin1@example.com / Password123!"
echo "Observer: observer1@example.com / Password123!"
echo ""
echo "To stop the demo environment, run: docker-compose down"
echo "------------------------------------------------"
