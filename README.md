# Absolute Cinema Backend

Absolute Cinema Backend is a modern **ASP.NET Core Web API** built with a strong focus on **security**, **clean architecture**, and **real-world backend practices**.

The project implements **JWT-based authentication with refresh token rotation**, centralized logging, validation, caching, and an architecture that can **evolve into microservices if needed**.

---

## Tech Stack

- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Redis
- JWT (Access Token & Refresh Token)
- Serilog + Elasticsearch
- FluentValidation
- Swagger (OpenAPI)

---

## Architecture

The project follows a **layered architecture**:
API
├── Controllers
├── Authentication / Authorization
└── Swagger

Application
├── Business logic
├── DTOs
├── Validators
└── Abstractions (Interfaces)

Infrastructure
├── Database (EF Core)
├── Redis
├── JWT / Token services
└── External integrations

Core
└── Domain entities
This structure keeps business logic independent from infrastructure and makes the system easier to maintain, test, and scale.

---

## Authentication & Authorization

The authentication system is based on **JWT** with **access tokens** and **refresh tokens**.

### Why JWT?

- Stateless authentication
- Works well with distributed systems
- Suitable for web, mobile, and third-party clients
- Microservice-friendly

---

## Token Types

### Access Token

- JWT
- Short-lived (default: 15 minutes)
- Sent via HTTP header:
  Authorization: Bearer <access_token>
- Required for protected endpoints

### Refresh Token

- Cryptographically secure random string
- Long-lived (default: 7 days)
- Stored as **HttpOnly cookie**
- Used only to obtain a new access token

---

## Refresh Token Flow

1. User logs in or registers
2. Backend returns:

- Access Token in response body
- Refresh Token as HttpOnly cookie

3. When the access token expires:

- Client sends the expired access token in the `Authorization` header
- Refresh token is automatically sent via cookie

4. Backend validates:

- JWT signature, issuer, and audience
- Refresh token existence in Redis
- Refresh token validity in database

5. Old refresh token is revoked
6. New access & refresh tokens are generated

This approach improves security and allows proper logout and session invalidation.

---

## Security Measures

- Short-lived access tokens
- Refresh token rotation
- Refresh token revocation
- HttpOnly cookies
- Redis + database double validation
- Role-based authorization support

---

## Authorization

Protected endpoints use:

```csharp
[Authorize]
Role-based authorization is supported using policies:
options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
Redis Usage

Redis is used to:
	•	Cache refresh token → user mappings
	•	Speed up refresh token validation
	•	Reduce database load

If Redis does not contain the refresh token, the system safely falls back to database validation.

⸻

Database
	•	SQL Server with Entity Framework Core
	•	Refresh tokens are persisted with:
	•	Token value
	•	User ID
	•	Expiration date
	•	Revocation timestamp

This allows:
	•	Multiple active sessions per user
	•	Secure logout
	•	Token revocation

⸻

Logging & Monitoring
	•	Serilog for structured logging
	•	Console logging for development
	•	Elasticsearch integration for centralized logs
	•	Global exception handling middleware

All requests and errors are logged consistently.

⸻

Validation & Error Handling
	•	FluentValidation for request validation
	•	Centralized error response format
	•	Consistent HTTP status codes
	•	Global exception middleware

⸻

Swagger & API Testing

Swagger is configured with Bearer authentication support.

Important notes:
	•	Swagger only attaches the token to requests
	•	Actual validation is done by ASP.NET Core authentication middleware
	•	Invalid or random tokens always result in 401 Unauthorized

⸻

Microservice Readiness

The project is currently implemented as a modular monolith.

This is a deliberate decision to:
	•	Reduce operational complexity
	•	Keep development and debugging simple

However, the architecture allows easy separation into microservices (Auth, Orders, Products) when needed.

⸻

Configuration

Sensitive configuration values are managed using .NET User Secrets, including:
	•	JWT settings
	•	Database connection strings
	•	Redis configuration

This keeps secrets out of source control.

⸻

Summary

Absolute Cinema Backend demonstrates:
	•	Secure JWT authentication with refresh token rotation
	•	Clean and maintainable layered architecture
	•	Production-ready logging and validation
	•	A strong foundation for future scaling and microservices

License

This project is for educational and demonstration purposes.
```
---

## Getting Started (How to Run the Project)

This section explains how to run the project locally from scratch.

### Prerequisites

Make sure the following tools are installed on your machine:

- .NET 8 SDK
- SQL Server (local or Docker)
- Redis (local or Docker)
- Git

Optional but recommended:
- Docker Desktop
- Elasticsearch (for logs)

---

## Clone the Repository

```bash
git clone https://github.com/your-username/absolute-cinema-backend.git
cd absolute-cinema-backend


Initialize User Secrets
dotnet user-secrets init
Required Secrets

Set the following secrets:
dotnet user-secrets set "Jwt:SecretKey" "YOUR_SUPER_SECRET_KEY"
dotnet user-secrets set "Jwt:Issuer" "absolute-cinema-api"
dotnet user-secrets set "Jwt:Audience" "absolute-cinema-client"
dotnet user-secrets set "Jwt:AccessTokenMinutes" "15"
dotnet user-secrets set "Jwt:RefreshTokenDays" "7"

dotnet user-secrets set "Redis:Connection" "localhost:6379"

dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
"Server=localhost,1433;Database=absolute_cinema;User Id=yourid;Password=YourPassword;TrustServerCertificate=True;"
Apply database migrations:

dotnet ef database update

Running Redis (Optional via Docker)

docker run -d -p 6379:6379 redis

Run the Application
dotnet run

The API will be available at:
http://localhost:5190




