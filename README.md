# 📚 Library Management System API

A robust, enterprise-grade RESTful API built with ASP.NET Core for comprehensive library operations management. Features JWT authentication, intelligent caching, advanced search capabilities, and real-time analytics.

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-512BD4?style=flat&logo=.net&logoColor=white)](https://docs.microsoft.com/ef/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## ✨ Highlights

- 🔐 **Secure Authentication** - JWT-based auth with refresh tokens and role-based access control
- ⚡ **High Performance** - Response caching, pagination,Rate Limiting, and optimized database queries
- 📊 **Analytics Ready** - Real-time statistics and insights for library operations
- 🏗️ **Clean Code** - Repository pattern, Unit of Work, and SOLID principles
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
- **Rate Limiting** - Limiting access to endpoints when failure for a period

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

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
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
       "ConnectionString": "Server=(localdb)\\mssqllocaldb;Database=LibraryDB;Trusted_Connection=true"
     }
   }
   ```

3. **Set up user secrets** (for sensitive data)
   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "your-super-secret-key-here"
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
   - API Base URL: `https://localhost:5132`
   - Swagger UI: `https://localhost:5132/swagger`

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

## Authentication Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/authentication` | Register new user | ❌ |
| POST | `/api/authentication/Login` | Login and get JWT tokens | ❌ |
| PUT | `/api/authentication/Logout` | Logout and revoke refresh token | ✅ Any |
| POST | `/api/authentication/Refresh` | Refresh access token | ❌ |

---

## Book Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/books` | Get all books (paginated) | ❌ |
| GET | `/api/books/{id}` | Get book by ID | ❌ |
| GET | `/api/books/isbn/{isbn}` | Get book by ISBN | ❌ |
| GET | `/api/books/by-author/{authorId}` | Get books by author | ❌ |
| GET | `/api/books/available` | Get available books | ❌ |
| GET | `/api/books/Search` | Search books by title/author | ❌ |
| POST | `/api/books/GetByIds` | Get multiple books by IDs | ❌ |
| POST | `/api/books` | Create new book | ✅ Admin |
| POST | `/api/books/Collection` | Create multiple books | ✅ Admin |
| PUT | `/api/books/{id}` | Update book | ✅ Admin |
| PATCH | `/api/books/{id}` | Partially update book | ✅ Admin |
| POST | `/api/books/{bookId}/Authors/{authorId}` | Add author to book | ✅ Admin |
| DELETE | `/api/books/{id}` | Delete book | ✅ Admin |
| POST | `/api/books/{bookId}/Borrow` | Borrow a book | ✅ Borrower |
| PUT | `/api/books/{bookId}/Return` | Return a book | ✅ Borrower |

---

## Author Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/authors` | Get all authors (paginated) | ❌ |
| GET | `/api/authors/{id}` | Get author by ID | ❌ |
| GET | `/api/authors/{id}/Stats` | Get author statistics | ❌ |
| GET | `/api/authors/Search` | Search authors by name | ❌ |
| POST | `/api/authors/GetByIds` | Get multiple authors by IDs | ❌ |
| POST | `/api/authors` | Create new author | ✅ Admin |
| POST | `/api/authors/Collection` | Create multiple authors | ✅ Admin |
| PUT | `/api/authors/{id}` | Update author | ✅ Admin |
| DELETE | `/api/authors/{id}` | Delete author | ✅ Admin |

---

## Borrower Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/borrowers` | Get all borrowers (paginated) | ✅ Admin |
| GET | `/api/borrowers/{id}` | Get borrower by ID | ✅ Admin |
| GET | `/api/borrowers/{id}/BorrowingHistory` | Get borrowing history | ✅ Admin |
| GET | `/api/borrowers/{id}/Loans/Current` | Get current loans | ✅ Admin |
| GET | `/api/borrowers/{id}/Loans/Overdue` | Get overdue loans | ✅ Admin |
| GET | `/api/borrowers/Search` | Search borrowers by name | ✅ Admin |
| POST | `/api/borrowers/GetByIds` | Get multiple borrowers by IDs | ✅ Admin |
| POST | `/api/borrowers` | Create new borrower | ✅ Admin |
| POST | `/api/borrowers/Collection` | Create multiple borrowers | ✅ Admin |
| DELETE | `/api/borrowers/{id}` | Delete borrower | ✅ Admin |

---

## User Management Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/users` | Get all users (paginated) | ✅ Admin |
| GET | `/api/users/{id}` | Get user by ID | ✅ Admin |
| POST | `/api/users` | Create new user | ✅ Admin |
| PUT | `/api/users/{id}` | Update user profile | ✅ Admin |
| DELETE | `/api/users/{id}` | Delete user | ✅ Admin |
| POST | `/api/users/Roles/Assign` | Assign roles to user | ✅ Admin |
| POST | `/api/users/Roles/Remove` | Remove roles from user | ✅ Admin |

---

## Person Endpoints

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| GET | `/api/persons` | Get all persons (paginated) | ✅ Admin |
| GET | `/api/persons/{id}` | Get person by ID | ✅ Admin |
| POST | `/api/persons/GetByIds` | Get multiple persons by IDs | ✅ Admin |
| POST | `/api/persons` | Create new person | ✅ Admin |
| POST | `/api/persons/Collection` | Create multiple persons | ✅ Admin |
| PUT | `/api/persons/{id}` | Update person | ✅ Admin |
| DELETE | `/api/persons/{id}` | Delete person | ✅ Admin |

> 📖 **Full Documentation**: Visit `/swagger` endpoint when running the API for interactive documentation

---

## 🏗️ Project Structure

```
LibraryManagementSystem/ (Solution - 3 projects)
│
├── LibraryManagementSystem.API/           # Web API Layer (Presentation)
│   ├── Controllers/                       # API endpoints
│   │   ├── AuthenticationController.cs    # Auth endpoints
│   │   ├── AuthorsController.cs           # Author management
│   │   ├── BooksController.cs             # Book management
│   │   ├── BorrowersController.cs         # Borrower management
│   │   ├── PersonsController.cs           # Person management
│   │   └── UsersController.cs             # User management
│   │
│   ├── Constants/                         # Application constants
│   │   └── Constants.cs                   # Role names, etc.
│   │
│   ├── ExceptionHandlers/                 # Global exception handling
│   │   ├── BadRequestExceptionHandler.cs
│   │   ├── ConflictExceptionHandler.cs
│   │   ├── DefaultExceptionHandler.cs
│   │   ├── NotFoundException.cs
│   │   └── UnauthorizedExceptionHandler.cs
│   │
│   ├── Extensions/                        # Service registration extensions
│   │   ├── ApiServicesExtensions.cs       # API-specific services
│   │   ├── ApplicationServicesExtensions.cs
│   │   ├── DatabaseExtensions.cs          # Database configuration
│   │   ├── IdentityAndAuthExtensions.cs   # Auth setup
│   │   ├── LoggingExtensions.cs           # Serilog configuration
│   │   └── SeedingExtensions.cs           # Data seeding
│   │
│   ├── Filters/                           # Action filters
│   │   ├── ActionFilters/
│   │   │   └── LogPerformanceFilterAttribute.cs
│   │   └── ResultFilters/
│   │       └── HandlePagedDataFilterAttribute.cs
│   │
│   ├── Logs/                              # Application logs
│   │   └── log-*.txt                      # Daily log files
│   │
│   ├── Middlewares/                       # Custom middleware
│   │   └── RequestLoggingHandler.cs       # Request/response logging
│   │
│   ├── appsettings.json                   # Configuration
│   ├── LibraryBookManagementSystemAPI.http # HTTP test file
│   └── Program.cs                         # Application entry point
│
├── LibraryManagementSystem.Business/      # Business Logic Layer
│   ├── Contract/                          # Service interfaces
│   │   ├── IAuthenticationService.cs
│   │   ├── IAuthorService.cs
│   │   ├── IBookService.cs
│   │   ├── IBorrowerService.cs
│   │   ├── IEmailService.cs
│   │   ├── IPersonService.cs
│   │   └── IUserService.cs
│   │
│   ├── Dtos/                              # Data Transfer Objects
│   │   ├── AccountDtos/                   # Auth DTOs
│   │   │   └── AccountDtos.cs
│   │   ├── AuthorDtos/                    # Author DTOs
│   │   │   └── AuthorDtos.cs
│   │   ├── BookDtos/                      # Book DTOs
│   │   │   └── BookDtos.cs
│   │   ├── BorrowerDtos/                  # Borrower DTOs
│   │   │   └── BorrowerDtos.cs
│   │   ├── BorrowingDtos/                 # Borrowing DTOs
│   │   │   └── BorrowingDtos.cs
│   │   ├── PersonDtos/                    # Person DTOs
│   │   │   └── PersonDtos.cs
│   │   ├── Shared/                        # Shared DTOs
│   │   │   └── Shared.cs
│   │   └── UserDtos/                      # User DTOs
│   │       └── UserDtos.cs
│   │
│   ├── Exceptions/                        # Business exceptions
│   │   └── BusinessExceptions.cs
│   │
│   ├── Mappings/                          # AutoMapper profiles
│   │   ├── AuthorMappingProfile.cs
│   │   ├── BookMappingProfile.cs
│   │   ├── BorrowerMappingProfile.cs
│   │   ├── PersonMappingProfile.cs
│   │   └── UserMappingProfile.cs
│   │
│   ├── Options/                           # Configuration options
│   │   ├── EmailSettings.cs
│   │   ├── ForgotPasswordSettings.cs
│   │   ├── JwtSettings.cs
│   │   └── LoanSettings.cs
│   │
│   ├── Services/                          # Service implementations
│   │   ├── AuthenticationService.cs
│   │   ├── AuthorService.cs
│   │   ├── BookService.cs
│   │   ├── BorrowerService.cs
│   │   ├── EmailService.cs
│   │   ├── PersonService.cs
│   │   └── UserService.cs
│   │
│   └── Validations/                       # FluentValidation validators
│       └── ListElementsRangeAttribute.cs
│
└── LibraryManagementSystem.DataAccess/    # Data Access Layer
    ├── Contract/                          # Repository interfaces
    │   ├── IAuthorRepository.cs
    │   ├── IBaseEntity.cs
    │   ├── IBookRepository.cs
    │   ├── IBorrowerRepository.cs
    │   ├── IBorrowingRepository.cs
    │   ├── IPersonRepository.cs
    │   ├── IRepositoryBase.cs
    │   ├── ISoftDeletable.cs
    │   └── IUnitOfWork.cs
    │
    ├── Data/                              # EF Core DbContext
    │   ├── Config/                        # Entity configurations
    │   │   └── Interceptors/
    │   ├── AppDbContext.cs                # Main DbContext
    │   └── AppDbContextFactory.cs         # Design-time factory
    │
    ├── Entities/                          # Domain entities
    │   ├── People/                        # Person-related entities
    │   ├── Book.cs
    │   └── Borrowing.cs
    │
    ├── Extensions/                        # Data access extensions
    │   ├── Utility/
    │   ├── AuthorExtensions.cs
    │   ├── BookExtensions.cs
    │   ├── BorrowerExtensions.cs
    │   └── PersonExtensions.cs
    │
    ├── Migrations/                        # EF Core migrations
    │   ├── 20251002073901_Final.cs
    │   └── AppDbContextModelSnapshot.cs
    │
    └── Repositories/                      # Repository implementations
        ├── AuthorRepository.cs
        ├── BookRepository.cs
        ├── BorrowerRepository.cs
        ├── BorrowingRepository.cs
        └── PersonRepository.cs
```

---

## 🔧 Configuration

### JWT Settings

Configure JWT in `appsettings.json`:

```json
{
  "Jwt": {
    "Issuer": "https://localhost:7266",
    "Audience": "https://localhost:7266",
    "ExpireMinutes": 30,
    "RefreshTokenExpireDays": 7
  }
}
```

### Caching Configuration

```json
{
  "CacheSettings": {
    "ExpirationMinutes": 10
  }
}
```

### Logging Configuration

The project uses Serilog for structured logging. Logs are written to both console and file.

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
- Update documentation as needed
- Keep commits atomic and well-described

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Mohamed Hodaib**

- GitHub: [@mohamedHodaib](https://github.com/mohamedHodaib)
- LinkedIn: [Connect with me](https://www.linkedin.com/in/mohamed-hodaib-2670b2344/)

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
