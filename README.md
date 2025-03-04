# BonusSystem Prototype

A bonus tracking and management platform designed to provide a flexible, scalable solution for managing bonus transactions across multiple user groups.

## Project Overview

BonusSystem is a comprehensive platform that allows different user roles (buyers, sellers, admins, etc.) to interact with a bonus point system. It follows a Vertical Slice Monolith with BFF (Backend for Frontend) architecture using .NET 9 and Supabase as the backend service.

## Architecture

- **Architecture Style**: Monolithic with Modular Design
- **Technology Stack**:
  - Backend: .NET 9, Supabase (PostgreSQL)
  - Authentication: Supabase Authentication

## Project Structure

- **BonusSystem.Api**: Web API with BFF implementation
- **BonusSystem.Core**: Domain models and business logic
- **BonusSystem.Infrastructure**: Supabase integration
- **BonusSystem.Shared**: DTOs and shared utilities
