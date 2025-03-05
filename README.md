# BonusSystem Prototype

A bonus tracking and management platform designed to provide a flexible, scalable solution for managing bonus transactions across multiple user roles.

## Project Overview

BonusSystem is a comprehensive platform that allows different user roles (buyers, sellers, admins, etc.) to interact with a bonus point system. It follows a Vertical Slice Monolith with BFF (Backend for Frontend) architecture using .NET 9 and Supabase as the backend service.

## Architecture

- **Architecture Style**: Monolithic with Modular Design
- **API Pattern**: Backend for Frontend (BFF) with Vertical Slices
- **Technology Stack**:
  - Backend: .NET 9, Supabase (PostgreSQL)
  - Authentication: Supabase Authentication with JWT

## Project Structure

- **BonusSystem.Api**: Web API with BFF implementation
- **BonusSystem.Core**: Domain models and business logic
- **BonusSystem.Infrastructure**: Supabase integration
- **BonusSystem.Shared**: DTOs and shared utilities

## Development Environment Setup

### Prerequisites

- Docker and Docker Compose
- .NET 9 SDK (for local development)

### Getting Started with Docker

1. Clone the repository:

```bash
git clone https://github.com/danila-permogorskii/bonus-system.git
cd bonus-system
```

2. Start the Docker environment:

```bash
docker-compose up -d
```

This will start the following services:

- **API**: .NET 9 API (http://localhost:5001, https://localhost:5002)
- **PostgreSQL**: Database (localhost:5432)
- **pgAdmin**: Database management tool (http://localhost:5050)
- **Supabase**: Local Supabase instance (API: http://localhost:8000, Studio: http://localhost:8080)

3. Access the services:

- **API Swagger UI**: http://localhost:5001/swagger
- **pgAdmin**: http://localhost:5050 (Email: admin@bonussystem.com, Password: admin)
- **Supabase Studio**: http://localhost:8080

### Environment Variables

All configuration is stored in the `.env` file at the root of the project. You can modify these values to change ports, credentials, etc.

### Stopping the Environment

```bash
docker-compose down
```

To remove volumes as well:

```bash
docker-compose down -v
```

## API User Roles

1. **Buyers**: End users who earn and spend bonuses
2. **Sellers**: Store employees who process transactions
3. **Admins**: System administrators who manage companies and stores
4. **Observers**: Company or system observers who view statistics

## Development Guidelines

1. Follow the Vertical Slice architecture pattern
2. Implement role-specific endpoints in their respective feature folders
3. Use BFF services to implement business logic
4. Maintain separation of concerns between layers

## License

This project is for demonstration purposes only.
