# Travel And Accommodation Booking Platform API  
__A backend platform for booking travel and accommodations.__

## 📖 Overview
The Travel and Accommodation Booking Platform API is a backend service that provides secure, scalable,
and maintainable endpoints for managing users, hotels, rooms, reservations, and reviews. Built with a clean
architecture and RESTful principles, it supports both customer-facing operations and administrative
workflows through Role-Based Access Control (RBAC).

## ✨ Features
### Core Functionality
* 🧑‍💼 User Management: Complete CRUD operations for users with authentication and role assignments.

* 🏨 Hotel Management: Create, update, delete, and retrieve hotels with location details and images.

* 🛏 Room Management: Manage room details, pricing, availability, and associations with hotels.

* 📅 Reservation System: Book, update, cancel, and view reservations with availability checks.

* ⭐ Review Management: Add, edit, delete, and retrieve hotel reviews with ratings.

* 🔍 Search & Filtering: Query hotels by location, city, availability, and other filters.

* 🔐 RBAC: Role-Based Access Control for admin and regular user permissions.

### Technical Features
* 🔑 Secure Authentication: JWT-based stateless authentication for all API endpoints.

* ⚡ High Performance: Optimized database schema and queries for scalability.

* 🧪 Testing: Unit and integration tests ensuring API reliability and correctness.

* 🗄 Data Integrity: Relational schema with foreign key constraints and validation.

* 🌐 RESTful API: Clean and consistent endpoint design for easy integration.

* 📷 File Upload Support: Image handling for hotels and rooms using AWS S3.

* 📊 Scalable Architecture: Layered design separating concerns for maintainability.

## 🛠️ Tech Stack
  ### Core Technologies
* Language & Framework: C# (.NET 8 / .NET 9)

* Architecture: Clean Architecture with layered design (Controllers, Services, Repositories)

* Database: Microsoft SQL Server with Entity Framework Core

* Authentication: JWT (JSON Web Token) for secure, stateless auth

* Authorization: Role-Based Access Control (RBAC) for user, manager, and admin permissions

* File Storage: AWS S3 for storing and retrieving hotel & room images

* Testing: xUnit, Moq, and FluentAssertions for unit and integration testing

* API Design: RESTful principles with clear resource-based routing

* Validation: Data annotations and custom validators for request models
### Development Tools
* AWS EC2 — Hosting and deployment environment for the API backend.

* GitHub Actions — CI/CD pipelines for automated build, test, and deployment.

* Swagger / OpenAPI — Interactive API documentation and endpoint testing.

* Git — Version control for code management and collaboration.

## 🚀 Getting Started

### Prerequisites
Make sure you have the following installed:
- [.NET SDK 8.0 or 9.0](https://dotnet.microsoft.com/en-us/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server)
- [AWS CLI](https://aws.amazon.com/cli/) (configured with your credentials for S3)
- [Git](https://git-scm.com/)

### Installation

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/travel-accommodation-booking-platform.git
   cd travel-accommodation-booking-platform
2. **Restore dependencies:**
  ```bash
 dotnet restore
  ```
  
3. **Set up environment variables:**
  > Create an .env file in the API project and configure:
```

# SQL Server connection
SQLSERVERCONNECTIONSTRING=YourConnectionStringHere

# AWS S3 Configuration
AWS:AWSBUCKETNAME=your-bucket-name
AWS:AWSACCESSKEYID=your-access-key
AWS:AWSSECRETACCESSKEY=your-secret-key
AWS:AWSREGION=your-region

# Stripe Configuration (if using payments)
STRIPESECRETKEY=your-stripe-secret-key
WEBHOOKSECRET=your-webhook-secret

# JWT Authentication
JWT:JWTSECRETKEY=your-jwt-secret-key
JWT:JWTISSUER=your-jwt-issuer
JWT:JWTAUDIENCE=your-jwt-audience
```

4. **Run database migrations:**
   ```
   dotnet ef database update
   ```
5. **Run the API:**
   ```
   dotnet run
   ```
