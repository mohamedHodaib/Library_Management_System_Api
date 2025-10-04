# 📚 Library Management System API

A robust, enterprise-grade RESTful API built with ASP.NET Core for comprehensive library operations management. Features JWT authentication, intelligent caching, advanced search capabilities, and real-time analytics.

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=flat&logo=.net&logoColor=white)](https://docs.microsoft.com/ef/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## ✨ Highlights

- 🔐 **Secure Authentication** - JWT-based auth with refresh tokens and role-based access control
- ⚡ **High Performance** - Response caching, pagination, and optimized database queries
- 📊 **Analytics Ready** - Real-time statistics and insights for library operations
- 🏗️ **Clean Architecture** - Repository pattern, Unit of Work, and SOLID principles
- 📝 **Production Ready** - Structured logging, error handling, and monitoring built-in

---

## 🎯 Features

### 📖 Library Operations
- **Book Management**
  - Complete CRUD operations with availability tracking
  - Category-based organization and filtering
  - Loan history and reservation system
  
- **Author Management**
  - Author profiles with biographical information
  - Publication statistics and performance metrics
  - Book catalog per author

- **Borrower Services**
  - User profile management
  - Active loan tracking
  - Overdue notification system
  - Borrowing history and limits

### 🔐 Authentication & Security
- JWT token-based authentication with refresh token support
- Role-based authorization (Admin, Librarian, Member)
- Secure password management with reset functionality
- Token revocation on logout
- ASP.NET Core Identity integration
- Secrets management using user secrets

### ⚡ Performance Features
- **Response Caching** - Intelligent caching for GET endpoints
- **Pagination** - Efficient data loading with metadata headers
- **Search & Filter** - Advanced querying with sorting capabilities
- **Performance Monitoring** - Request timing tracked via action filters

### 🛠️ Technical Excellence
- **Three-Tier Architecture** with clear separation of concerns
- **Repository & Unit of Work** patterns for data access
- **Entity Framework Core** with code-first migrations
- **AutoMapper** for clean object-to-object mapping
- **Serilog** for structured logging with correlation IDs
- **Custom Middleware** for exception handling and request/response logging
- **Dependency Injection** throughout the application
- **Options Pattern** for configuration management

---

## 🚀 Getting Started

### Prerequisites

Before running the project, ensure you have:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- SQL Server (LocalDB, Express, or full version)
- Your favorite IDE ([Visual Studio](https://visualstudio.microsoft.com/), [VS Code](https://code.visualstudio.com/), or [Rider](https://www.jetbrains.com/rider/))

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/mohamedHodaib/Library_Management_System_Api.git
   cd Library_Management_System_Api
   ```

2. **Configure the database connection**
   
   Update the connection string in `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LibraryDB;Trusted_Connection=true"
     }
   }
   ```

3. **Set up user secrets** (for sensitive data)
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Secret" "your-super-secret-key-here"
   ```

4. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

5. **Restore dependencies**
   ```bash
   dotnet restore
   ```

### Running the Application

1. **Build the project**
   ```bash
   dotnet build
   ```

2. **Run the API**
   ```bash
   dotnet run
   ```

3. **Access the API**
   - API Base URL: `https://localhost:5001`
   - Swagger UI: `https://localhost:5001/swagger`

### Quick Test

Once running, you can test the API:

```bash
# Register a new user
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Test@123"}'

# Login
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user@example.com","password":"Test@123"}'
```

---

## 📚 API Documentation

### Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login and receive JWT token |
| POST | `/api/auth/refresh` | Refresh access token |
| POST | `/api/auth/logout` | Revoke token and logout |
| POST | `/api/auth/forgot-password` | Request password reset |
| POST | `/api/auth/reset-password` | Reset user password |

### Book Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/books` | Get all books (paginated) | ✅ |
| GET | `/api/books/{id}` | Get book by ID | ✅ |
| POST | `/api/books` | Create new book | ✅ Admin |
| PUT | `/api/books/{id}` | Update book | ✅ Admin |
| DELETE | `/api/books/{id}` | Delete book | ✅ Admin |
| POST | `/api/books/{id}/borrow` | Borrow a book | ✅ |
| POST | `/api/books/{id}/return` | Return a book | ✅ |

### Author Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/authors` | Get all authors |
| GET | `/api/authors/{id}` | Get author details |
| GET | `/api/authors/{id}/statistics` | Get author statistics |
| POST | `/api/authors` | Create author (Admin) |
| PUT | `/api/authors/{id}` | Update author (Admin) |
| DELETE | `/api/authors/{id}` | Delete author (Admin) |

### Borrower Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/borrowers/loans` | Get current loans |
| GET | `/api/borrowers/overdue` | Get overdue loans |
| GET | `/api/borrowers/history` | Get borrowing history |

> 📖 **Full Documentation**: Visit `/swagger` endpoint when running the API for interactive documentation

---

## 🏗️ Project Structure

```
Library_Management_System_Api/
├── API/                          # Web API layer
│   ├── Controllers/              # API endpoints
│   ├── Middleware/               # Custom middleware
│   └── Program.cs               # Application entry point
├── Core/                         # Domain layer
│   ├── Entities/                # Domain entities
│   ├── Interfaces/              # Repository interfaces
│   └── DTOs/                    # Data transfer objects
├── Infrastructure/               # Data access layer
│   ├── Data/                    # DbContext and configurations
│   ├── Repositories/            # Repository implementations
│   └── Identity/                # Identity configuration
└── Services/                     # Business logic layer
    ├── BookService.cs
    ├── AuthorService.cs
    └── AuthService.cs
```

---

## 🔧 Configuration

### JWT Settings

Configure JWT in `appsettings.json`:

```json
{
  "Jwt": {
    "Issuer": "LibraryAPI",
    "Audience": "LibraryClients",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  }
}
```

### Caching Configuration

```json
{
  "CacheSettings": {
    "DefaultExpirationMinutes": 10,
    "SlidingExpirationMinutes": 5
  }
}
```

### Logging Configuration

The project uses Serilog for structured logging. Logs are written to both console and file.

---

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork** the repository
2. **Create** a feature branch
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Commit** your changes
   ```bash
   git commit -m 'Add some amazing feature'
   ```
4. **Push** to the branch
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Open** a Pull Request

### Development Guidelines

- Follow existing code style and conventions
- Write unit tests for new features
- Update documentation as needed
- Keep commits atomic and well-described

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Mohamed Hodaib**

- GitHub: [@mohamedHodaib](https://github.com/mohamedHodaib)
- LinkedIn: [Connect with me](https://linkedin.com/in/mohamedhodaib)

---

## 🙏 Acknowledgments

- Built with [ASP.NET Core](https://dotnet.microsoft.com/apps/aspnet)
- Uses [Entity Framework Core](https://docs.microsoft.com/ef/core/) for data access
- Authentication powered by [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)
- Logging via [Serilog](https://serilog.net/)
- Object mapping with [AutoMapper](https://automapper.org/)
- Thanks to the open-source community for inspiration and tools

---

## 📞 Support

If you have any questions or run into issues:

- 📫 Open an [issue](https://github.com/mohamedHodaib/Library_Management_System_Api/issues)
- 💬 Start a [discussion](https://github.com/mohamedHodaib/Library_Management_System_Api/discussions)
- ⭐ Star the project if you find it helpful!

---

<div align="center">
Made with ❤️ by Mohamed Hodaib
</div>
