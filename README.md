# Library Management System API

Backend Web API Project for managing Books (borrow, return), authors (get statistics), borrowers (Get Current
and overdue loans), and Users (Login, Register,ForgetPassword,ChangePassword, and manage using ASP.NET Core Identity). applying
authentication and authorization using JWT, Entity framework Core, Logging, Caching API Responses , Searching
And Sorting Responses , DTO, Documentation. also Using AI to optimize my solutions after apply first to get
more knowledge while making projects.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Development](#development)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)
  
## Features
- ✨ CRUD operations - using Entity Framework core in the DataAcces.
- 🚀 Searching & Dashboard statistics - get author statistics.
- 🔒 Pagination & Sorting - apply pagination for enpoints that return many records and optional sorting.
- 📱 Cashing  - apply cashing for GET endpoints to optimize performance.
- Logging - using Serilog Library.
- Automapping - using Automapper library.
- Filters - Action filters for logging performance of handling requests & Result Filter to add pagination header to the response for meta data of paginated data.
- Extentions - To make the registeration at IOC container in program.cs more cleaner.
- Exption Handling - By inherit from IExceptionHandler and using the buil-in UseExceptionHandler middleware to catch the exceptions that occured in the Pipline.
- Custom MiddleWare - For logging requests.
- Authentication & Authorization - using JWT(JSON Web Token) for authentication, refresh token to get access token when access token expired and applying role-based authorization to prevent forbidden access
- DTOs - Shapping the response that returned from the DataAccess.
- Options Design Pattern - To map settings at appsettings Json.
- Repository Design Pattern - For handling Database access.
- Unit Of Work Design Pattern - To Unifiy Repositories that access database under on class, and saving to database at one transaction.
- Dependeny Injection & Dependency Inversion - To prevent Dependency between objects and classes.
### Business Logic
- Book: handle borrowing and returning books.
- Borrower: Get current and over due loans.
- Authors: Get Author Statistics.
- Users & Security: Register,Login,Logout,Assign roles,remove roles,Revoke Refresh Token when Logout, ForgetPassword,and ChangesPassword using ASP.NET Core Identity for managing users.

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
