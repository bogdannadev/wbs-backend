# BonusSystem Port Configuration Guide

This document outlines the port configuration for the BonusSystem prototype and explains the reasoning behind these choices according to best practices.

## Port Assignments

| Service          | Port  | Internal Container Port |
|------------------|-------|------------------------|
| BonusSystem API  | 5001  | 8080                   |
| PostgreSQL       | 5432  | 5432                   |
| pgAdmin          | 5050  | 80                     |

## Best Practices for Port Configuration

### 1. Standardized Port Ranges

- **User-Defined Services (5000-5999)**: Reserved for custom application services
  - API HTTP port (5001)
  - Management interfaces (5050 for pgAdmin)
- **Database Standard Ports**: Using standard ports makes it easier for tools to connect
  - PostgreSQL standard port (5432)

### 2. Port Selection Reasoning

- **API Port (5001)**:
  - Using port 5001 for HTTP is a common practice for .NET applications
  - This port is typically free on development machines
  - Clear separation from the default .NET ports (5000)
  
- **PostgreSQL (5432)**:
  - Standard port for PostgreSQL
  - Using the standard port minimizes configuration in client tools
  
- **pgAdmin (5050)**:
  - Clear separation from application ports
  - Easy to remember as "50xx" for admin interfaces

### 3. Container Port Configuration

- **Internal Port 8080**: 
  - The API container is configured to listen on port 8080 internally
  - This is mapped to external port 5001 as defined in the docker-compose.yml
  - Using the `ASPNETCORE_URLS` environment variable to explicitly set listening ports

### 4. HTTPS Configuration

In the Docker development environment, we use HTTP-only for simplicity:

- **Development Docker Environment**: HTTP only on port 5001 for easier development
- **Local Development**: HTTPS is still available when running directly on the host machine
- **Production**: Should use HTTPS with proper certificates

For a production environment, you would need to:
1. Configure a valid SSL certificate
2. Set up HTTPS in the container or use a reverse proxy like Nginx or Traefik
3. Implement automatic HTTP to HTTPS redirection

### 5. Security Considerations

- **Development Environment**:
  - HTTP is acceptable for local development
  - Docker creates an isolated network for services
  
- **Production Environment**:
  - Always use HTTPS in production
  - Database ports should never be exposed directly to the internet
  - Management interfaces like pgAdmin should be restricted to VPN/internal networks

### 6. Development vs. Production

- **Development Environment**:
  - Local ports are configured in launchSettings.json and docker-compose.yml
  - Environment variables (.env file) control port assignments for easy configuration
  
- **Production Environment**:
  - Production should use standard web ports (80/443) with a reverse proxy
  - Kubernetes or similar orchestration should handle port mapping
  - Internal services should communicate via service discovery when possible

## API Documentation Path

The API documentation (Swagger UI) is accessible at:

- **Development Docker**: http://localhost:5001/api-docs
- **Local Development**: https://localhost:5002/api-docs

This path follows the best practice of using a clearly identified endpoint that:
1. Makes it clear it's API documentation
2. Doesn't conflict with potential API resource paths
3. Is consistent across environments

## Environment Configuration

All port settings are configurable through environment variables:

```
# Ports Configuration
API_HTTP_PORT=5001
POSTGRES_PORT=5432
PGADMIN_PORT=5050
```

Modify these values in the `.env` file to change port assignments for all components.

## Troubleshooting Port Issues

If the application doesn't respond on the expected ports, check:

1. Docker container logs to see which port the application is actually listening on
2. Ensure the port mappings in docker-compose.yml match the actual listening ports
3. Verify the `ASPNETCORE_URLS` environment variable is set correctly
4. Check if the host machine has another process using the same ports

### Common HTTPS Certificate Issues

When working with Docker and HTTPS:

1. **Development Certificate Issues**: Docker containers don't have access to the host's development certificates
2. **Solutions**:
   - Use HTTP only in the development container (current approach)
   - Generate development certificates inside the container
   - Mount certificate files from the host
   - Use a reverse proxy with SSL termination
