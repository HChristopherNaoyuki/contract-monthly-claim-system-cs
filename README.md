# Contract Monthly Claim System (CMCS)

## Table of Contents
- [Overview](#overview)
- [System Architecture](#system-architecture)
- [Project Structure](#project-structure)
- [Installation Guide](#installation-guide)
- [Usage Manual](#usage-manual)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Development Notes](#development-notes)
- [Documentation](#documentation)
- [Notes](#notes)
- [Disclaimer](#disclaimer)

## Overview

The **Contract Monthly Claim System (CMCS)** is a comprehensive 
web-based application designed to streamline the monthly claim 
submission and approval process for independent contractor 
lecturers. This system provides an intuitive platform for 
lecturers to submit claims, for programme coordinators to review 
them, and for academic managers to provide final approvals.

**Key Objectives:**
- Simplify the claim submission process for lecturers
- Provide transparent tracking of claim status
- Enable efficient review and approval workflows
- Maintain secure data storage without database dependencies

## System Architecture

### Core Components

**Frontend Layer:**
- ASP.NET Core MVC with Razor Views
- Responsive CSS design with modern UI components
- Client-side validation with JavaScript
- Session-based authentication

**Business Logic Layer:**
- Controllers for authentication, claims, and system management
- ViewModels for data validation and presentation
- Service layer for business operations
- Text file-based data persistence

**Data Storage Layer:**
- JSON-based text file storage system
- Automatic file creation and management
- Data serialization/deserialization
- File-based transaction management

### Architecture Patterns
- **MVC Pattern**: Separation of concerns between Models, Views, and Controllers
- **Repository Pattern**: Abstracted data access through service classes
- **Session Management**: Secure user authentication and state management
- **Extension Methods**: Enhanced functionality for built-in types

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── AuthController.cs          # Authentication and user management
│   ├── ClaimsController.cs        # Claim submission and approval
│   ├── HomeController.cs          # Public pages and navigation
│   ├── ServerController.cs        # System diagnostics
│   └── TestController.cs          # Database connectivity tests
├── Models/
│   ├── DataModels/                # Entity models
│   │   ├── User.cs                # System users
│   │   ├── Lecturer.cs            # Lecturer-specific data
│   │   ├── Claim.cs               # Claim submissions
│   │   ├── Document.cs            # Supporting documents
│   │   └── Approval.cs            # Approval records
│   ├── ViewModels/                # View-specific models
│   │   ├── LoginViewModel.cs      # Authentication data
│   │   ├── RegisterViewModel.cs   # User registration
│   │   └── ClaimViewModels/       # Claim-related views
├── Views/                         # Razor view templates
├── Extensions/
│   └── SessionExtensions.cs       # Session management utilities
├── Services/
│   └── TextFileDataService.cs     # Text file data operations
├── Data/                          # Text file storage directory
├── wwwroot/                       # Static assets
└── Program.cs                     # Application entry point
```

## Installation Guide

### Prerequisites
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** or **Visual Studio Code**
- **Web browser** (Chrome, Firefox, Edge, or Safari)

### Step-by-Step Installation

1. **Clone or Download the Project**
   ```bash
   git clone https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git
   cd contract-monthly-claim-system-cs
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Solution**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```
   Or use Visual Studio:
   - Open the solution file (.sln)
   - Press F5 or click "Start Debugging"

5. **Access the Application**
   - Open your web browser
   - Navigate to: `http://localhost:5000`

### Default Login Credentials
- **Username**: `admin`
- **Password**: `admin123`
- **Role**: Academic Manager

## Usage Manual

### For Lecturers

**1. Login/Registration**
- Navigate to the login page
- Register as a new lecturer or use existing credentials
- Complete the registration form with personal details

**2. Submit a Claim**
- Click "Submit Claim" in navigation
- Fill in hours worked and hourly rate
- Add optional comments
- Upload supporting documents (PDF, DOC, images)
- Submit the claim for review

**3. Track Claim Status**
- Use "Track Claims" to view submission history
- Monitor approval progress
- View coordinator comments

### For Programme Coordinators

**1. Review Claims**
- Access "Review Claims" page
- View all pending submissions
- Examine claim details and documents

**2. Approve/Reject Claims**
- Click "Approve" or "Reject" for each claim
- Provide comments for decisions
- Track decision history

### For Academic Managers

**1. System Oversight**
- Access all system features
- Monitor overall claim workflow
- Manage user accounts (if implemented)

## Features

### Core Functionality
- ✅ **User Authentication** - Secure login/registration system
- ✅ **Role-Based Access** - Different interfaces for lecturers, coordinators, and managers
- ✅ **Claim Submission** - Easy-to-use claim form with automatic calculations
- ✅ **Document Upload** - Support for multiple file types with validation
- ✅ **Approval Workflow** - Streamlined review and approval process
- ✅ **Status Tracking** - Real-time claim status monitoring
- ✅ **Session Management** - Secure user session handling

### Technical Features
- ✅ **Text File Database** - No external database dependencies
- ✅ **Responsive Design** - Works on desktop and mobile devices
- ✅ **Form Validation** - Client-side and server-side validation
- ✅ **Error Handling** - Comprehensive error management
- ✅ **Logging** - Application activity tracking
- ✅ **Unit Testing** - xUnit test coverage

### Security Features
- ✅ **Session Security** - Protected user sessions
- ✅ **Input Validation** - Sanitized user inputs
- ✅ **File Type Validation** - Secure document uploads
- ✅ **Anti-Forgery Tokens** - CSRF protection

## Technology Stack

### Backend Technologies
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 7.0
- **Session Management**: Distributed Memory Cache
- **Data Storage**: JSON Text Files
- **Testing**: xUnit 2.5.3

### Frontend Technologies
- **UI Framework**: ASP.NET Core Razor Views
- **Styling**: Custom CSS with CSS Variables
- **JavaScript**: Vanilla ES6+
- **Icons**: SVG Icons

### Development Tools
- **IDE**: Visual Studio 2022 / Visual Studio Code
- **Version Control**: Git
- **Package Management**: NuGet
- **Build Tool**: .NET CLI

## Development Notes

### Text File Database Implementation

The system uses JSON-based text files for data persistence:

```json
// Example: Data/users.txt
[
  {
    "UserId": 1,
    "Name": "System",
    "Surname": "Administrator",
    "Username": "admin",
    "Password": "admin123",
    "Role": 2,
    "Email": "admin@cmcs.com",
    "IsActive": true,
    "CreatedDate": "2024-01-01T00:00:00"
  }
]
```

### Session Management

Custom extension methods provide strongly-typed session access:

```csharp
// Setting session values
HttpContext.Session.SetInt32("UserId", user.UserId);
HttpContext.Session.SetString("Username", user.Username);

// Retrieving session values
var userId = HttpContext.Session.GetInt32("UserId");
var username = HttpContext.Session.GetString("Username");
```

### Error Handling

Comprehensive error handling throughout the application:

- Global exception middleware
- Development vs production error pages
- User-friendly error messages
- Detailed logging

## Documentation

### API Documentation

While this is primarily an MVC application, the controllers follow RESTful principles:

**Authentication Endpoints:**
- `GET /Auth` - Login/registration page
- `POST /Auth/Login` - User authentication
- `POST /Auth/Register` - User registration
- `POST /Auth/Logout` - Session termination

**Claims Endpoints:**
- `GET /Claims/Submit` - Claim submission form
- `POST /Claims/Submit` - Submit new claim
- `GET /Claims/Approve` - Approval dashboard
- `POST /Claims/ApproveClaim` - Process claim approval
- `GET /Claims/Track` - Claim status tracking

### Data Models

**User Model:**
```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}
```

**Claim Model:**
```csharp
public class Claim
{
    public int ClaimId { get; set; }
    public int LecturerId { get; set; }
    public DateTime ClaimDate { get; set; }
    public string MonthYear { get; set; }
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal Amount { get; set; }
    public ClaimStatus Status { get; set; }
    public string SubmissionComments { get; set; }
}
```

### Testing

The project includes comprehensive unit tests:

```csharp
// Example test for authentication
public class AuthControllerTests
{
    [Fact]
    public void Login_ValidCredentials_RedirectsToHome()
    {
        // Test implementation
    }
}
```

Run tests using:
```bash
dotnet test
```

## Notes

### Performance Considerations
- Text file storage is suitable for small to medium datasets
- For larger deployments, consider migrating to a proper database
- Session data is stored in memory - consider distributed cache for scaling

### Security Considerations
- Passwords are stored in plain text (for demonstration only)
- In production, implement proper password hashing
- Consider HTTPS enforcement in production
- Implement additional security headers

### Scalability
- Current architecture supports small to medium organizations
- For larger deployments:
  - Implement proper database (SQL Server, PostgreSQL)
  - Add caching layers
  - Consider microservices architecture
  - Implement API rate limiting

## Disclaimer

### Educational Purpose
This application is developed as part of an academic assignment 
and is intended for educational purposes. It demonstrates core 
concepts of web application development using ASP.NET Core MVC.

### Production Readiness
⚠️ **Not Production Ready** - This system contains several limitations for production use:

- **Security**: Passwords stored in plain text
- **Data Persistence**: Text files not suitable for high-volume applications
- **Error Handling**: Basic error management
- **Performance**: No caching or optimization for high load

### License and Usage
This project is provided as-is for educational purposes. 
Users are encouraged to review and enhance security 
measures before considering any production deployment.

### Data Privacy
The system handles user data and claims information. 
In a production environment, ensure compliance with 
data protection regulations (GDPR, POPIA, etc.) and 
implement proper data encryption and privacy measures.

---