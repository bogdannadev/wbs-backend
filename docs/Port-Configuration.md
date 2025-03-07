# BonusSystem Port Configuration Guide

This document outlines the port configuration for the BonusSystem prototype and explains the reasoning behind these choices according to best practices.

## Port Assignments

| Service          | HTTP Port | HTTPS Port | Internal Container Port |
|------------------|-----------|------------|------------------------|
| BonusSystem API  | 5001      | 5002       | 80/443                 |
| PostgreSQL       | 5432      | -          | 5432                   |
| pgAdmin          | 5050      | -          | 80                     |

## Best Practices for Port Configuration

### 1. Standardized Port Ranges

- **User-Defined Services (5000-5999)**: Reserved for custom application services
  - API HTTP/HTTPS ports (5001/5002)
  - Management interfaces (5050 for pgAdmin)
- **Database Standard Ports**: Using standard ports makes it easier for tools to connect
  - PostgreSQL standard port (5432)

### 2. Port Selection Reasoning

- **API Ports (5001/5002)**:
  - Using ports 5001/5002 for HTTP/HTTPS is a common practice for .NET applications
  - These ports are typically free on development machines
  - Clear separation from the default .NET ports (5000/5001)
  
- **PostgreSQL (5432)**:
  - Standard port for PostgreSQL
  - Using the standard port minimizes configuration in client tools
  
- **pgAdmin (5050)**:
  - Clear separation from application ports
  - Easy to remember as "50xx" for admin interfaces

### 3. Security Considerations

- **HTTPS Configuration**:
  - All public endpoints should be served over HTTPS (port 5002)
  - HTTP (port 5001) should redirect to HTTPS in production
  
- **Firewall Recommendations**:
  - In production, only the HTTPS port (5002) should be publicly accessible
  - Database ports should never be exposed directly to the internet
  - Management interfaces like pgAdmin should be restricted to VPN/internal networks

### 4. Development vs. Production

- **Development Environment**:
  - Local ports are configured in launchSettings.json and docker-compose.yml
  - Environment variables (.env file) control port assignments for easy configuration
  
- **Production Environment**:
  - Production should use standard web ports (80/443) with a reverse proxy
  - Kubernetes or similar orchestration should handle port mapping
  - Internal services should communicate via service discovery when possible

## API Documentation Path

The API documentation (Swagger UI) is accessible at:

- **Development**: https://localhost:5002/api-docs
- **Docker**: http://localhost:5001/api-docs

This path follows the best practice of using a clearly identified endpoint that:
1. Makes it clear it's API documentation
2. Doesn't conflict with potential API resource paths
3. Is consistent across environments

## Environment Configuration

All port settings are configurable through environment variables:

```
# Ports Configuration
API_HTTP_PORT=5001
API_HTTPS_PORT=5002
POSTGRES_PORT=5432
PGADMIN_PORT=5050
```

Modify these values in the `.env` file to change port assignments for all components.
