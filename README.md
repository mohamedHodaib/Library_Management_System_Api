# Library Management System API

Backend Web API Project for managing Books (borrow, return), authors (get statistics), borrowers (Get Current
and overdue loans), and Users (Login, Register,ForgetPassword,ChangePassword, and manage using ASP.NET Core Identity). applying
authentication and authorization using JWT, Entity framework Core, Logging, Caching API Responses , Searching
And Sorting Responses , DTO, Documentation. also Using AI to optimize my solutions after apply first to get
more knowledge while making projects.

## Table of Contents

- [Features](#features)
- [GettingStarted](#gettingstarted)
- [Contributing](#contributing)
- [License](#license)
  
## 🚀 features

### Core Functionality
- **📚 Library Management** - Complete CRUD operations for books, authors, and borrowers
- **🔄 Loan System** - Handle book borrowing, returns, and overdue tracking
- **📊 Analytics Dashboard** - Real-time author statistics and library insights
- **🔍 Advanced Search** - Full-text search with filtering, pagination, and sorting

### Performance & Scalability
- **⚡ Response Caching** - caching for GET endpoints
- **📄 Smart Pagination** - Efficient data loading with metadata headers
- **🎯 Performance Monitoring** - Request timing with action filters
- **📝 Structured Logging** - Serilog with correlation IDs for request tracking

### Security & Authentication
- **🔐 JWT Authentication** - Secure token-based auth with refresh tokens
- **👥 Role-Based Access Control** - Granular permissions system
- **🛡️ Security Features**
  - Password reset
  - Token revocation on logout
  - Secure secrets management (secrets.json)
  - ASP.NET Core Identity integration

### Architecture & Patterns
- **🏗️ Three-Tier Architecture**
  - Repository & Unit of Work patterns
  - Entity Framework Core with code-first approach
  - Dependency injection throughout
  - SOLID principles adherence

- **🔧 Developer Experience**
  - AutoMapper for object mapping
  - Custom exception handling middleware
  - Request/response logging middleware
  - Clean IOC container registration with extensions
  - Options pattern for configuration

### Business Features
- **Book Management**: Availability tracking, loan history, categorization
- **Borrower Services**: Active loans, overdue notifications, borrowing limits
- **Author Analytics**: Publication stats, popularity metrics, book performance
- **User Management**: Self-registration, profile management, role assignments
## Getting Started

### Prerequisites

To run this project, you need:

- [.NET SDK](https://dotnet.microsoft.com/) installed on your development machine.
- A database system (e.g., SQL Server, MySQL, or others), depending on the project configuration.

### Installation

1. **Clone the repository**:
   ```bash
   git clone https://github.com/mohamedHodaib/Library_Management_System_Api.git
   ```
2. **Navigate to the project directory**:
   ```bash
   cd Library_Management_System_Api
   ```
3. **Install dependencies**:
   Use the .NET CLI to restore any required dependencies.
   ```bash
   dotnet restore
   ```

### Running the API

1. **Build the project**:
   ```bash
   dotnet build
   ```
2. **Run the application**:
   ```bash
   dotnet run
   ```
3. The API will be available at `http://localhost:5000` by default.

## Contributing

Contributions are welcome! If you have ideas for improvements or new features, please open an issue or submit a pull request.

1. Fork the repository.
2. Create a new branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a pull request.

## License

This project currently does not have a license. Please refer to the repository for further details.

## Author

- **Mohamed Hodaib**  
  [GitHub Profile](https://github.com/mohamedHodaib)

## Acknowledgments

- Special thanks to the open-source community for inspiration and tools.
- Any contributors who help improve this project.

---

**Note**: As this repository currently doesn't have a detailed description, topics, or license, consider updating the repository with this information for better clarity and collaboration.
