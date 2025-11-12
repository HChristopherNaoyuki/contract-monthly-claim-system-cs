# Contract Monthly Claim System - Technical Documentation

## Table of Contents
1. [System Overview](#system-overview)
2. [Architecture Design](#architecture-design)
3. [Database Schema](#database-schema)
4. [API Documentation](#api-documentation)
5. [User Interface](#user-interface)
6. [Security Implementation](#security-implementation)
7. [Testing Strategy](#testing-strategy)
8. [Deployment Guide](#deployment-guide)

## System Overview

### Purpose and Scope
The Contract Monthly Claim System (CMCS) is a web-based application designed 
to automate and streamline the monthly claim submission and approval process 
for independent contractor lecturers. The system replaces manual paper-based 
processes with a digital workflow that enhances efficiency, transparency, 
and accuracy.

### Key Business Objectives
- Automate claim calculation and validation
- Provide multi-level approval workflows
- Enable real-time status tracking
- Generate comprehensive reports and analytics
- Ensure data security and access control

### Technical Specifications
- **Framework**: ASP.NET Core 8.0 MVC
- **Programming Language**: C# 12.0
- **Storage**: Text file-based (No database dependency)
- **Authentication**: Session-based with role authorization
- **Testing**: xUnit with Moq for mocking
- **Frontend**: Razor Pages with custom CSS

## Architecture Design

### High-Level Architecture
```
Presentation Layer (Views)
    |
Business Logic Layer (Controllers)
    |
Service Layer (Services)
    |
Data Access Layer (TextFileDataService)
    |
Storage Layer (Text Files)
```

### Component Responsibilities

#### Controllers Layer
- **AuthController**: Handles user authentication, registration, and session management
- **ClaimsController**: Manages claim lifecycle including submission, approval, and tracking
- **HomeController**: Provides public-facing pages and system navigation
- **ServerController**: Offers system diagnostics and troubleshooting
- **TestController**: Provides database and system testing capabilities

#### Services Layer
- **TextFileDataService**: Implements CRUD operations using text file storage
- **DatabaseService**: Provides health checks and connection testing (for future DB integration)

#### Models Layer
- **Data Models**: Define entity structures (User, Claim, Document, Approval, Lecturer)
- **View Models**: Provide data transfer objects for views
- **Enums**: Define system constants and status values

### Data Flow
1. User authentication and role validation
2. Claim submission with automatic calculation
3. Multi-level approval workflow
4. Status tracking and notification
5. Reporting and analytics generation

## Database Schema

### Text File Storage Structure
The system uses text files for data persistence, organized in the Data directory:

```
Data/
├── users.txt          # User accounts and profiles
├── lecturers.txt      # Lecturer-specific information
├── claims.txt         # Claim submissions and status
├── documents.txt      # Supporting document metadata
├── approvals.txt      # Approval history and comments
└── analytics.txt      # System usage statistics
```

### Data Models

#### User Model
```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
```

#### Claim Model
```csharp
public class Claim
{
    public int ClaimId { get; set; }
    public int LecturerId { get; set; }
    public DateTime ClaimDate { get; set; } = DateTime.Now;
    public string MonthYear { get; set; } = string.Empty;
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal Amount { get; set; }
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    public string SubmissionComments { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}
```

#### Enum Definitions
```csharp
public enum UserRole
{
    Lecturer = 0,
    ProgrammeCoordinator = 1,
    AcademicManager = 2,
    HumanResource = 3
}

public enum ClaimStatus
{
    Submitted = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3,
    Paid = 4
}
```

## API Documentation

### Authentication Endpoints

#### POST /Auth/Login
Authenticates user and creates session.

**Request Body:**
```json
{
    "Username": "string",
    "Password": "string",
    "RememberMe": "boolean"
}
```

**Response:**
- Success: Redirect to Home/Index
- Failure: Returns to login page with error messages

#### POST /Auth/Register
Creates new user account.

**Request Body:**
```json
{
    "Name": "string",
    "Surname": "string",
    "Username": "string",
    "Password": "string",
    "ConfirmPassword": "string",
    "Role": "number"
}
```

### Claims Management Endpoints

#### GET /Claims/Submit
Displays claim submission form.

**Response:** ClaimSubmissionViewModel with pre-populated data

#### POST /Claims/Submit
Processes new claim submission.

**Request Body:**
```json
{
    "HoursWorked": "number",
    "HourlyRate": "number",
    "Comments": "string",
    "Documents": "file[]"
}
```

**Response:**
- Success: Redirect to Status page with claim details
- Failure: Returns to submission form with validation errors

#### GET /Claims/Approve
Displays pending claims for approval.

**Response:** List of ClaimApprovalViewModel objects

#### POST /Claims/ApproveClaim
Processes claim approval or rejection.

**Request Body:**
```json
{
    "claimId": "number",
    "isApproved": "boolean",
    "comments": "string"
}
```

### HR Analytics Endpoints

#### GET /Claims/HRDashboard
Displays HR analytics dashboard.

**Response:** HRDashboardViewModel with comprehensive analytics data

#### GET /Claims/GenerateHRReport
Generates PDF report for HR analytics.

**Query Parameters:**
- reportType: "comprehensive" or "summary"

**Response:** PDF file download

## User Interface

### Design Philosophy
- Minimalist Apple-inspired design
- Mobile-first responsive approach
- Consistent color scheme and typography
- Intuitive navigation patterns
- Accessibility considerations

### Page Layouts

#### Authentication Pages
- Clean, centered forms with clear validation
- Tab-based login/registration switching
- Responsive design for all device sizes

#### Dashboard Pages
- Card-based information display
- Statistical overview with key metrics
- Quick action buttons for common tasks
- Role-specific content and features

#### Form Pages
- Clear form labels and instructions
- Real-time validation feedback
- Progressive disclosure of complex sections
- Accessible form controls

### Responsive Breakpoints
- Mobile: < 768px
- Tablet: 768px - 1024px
- Desktop: > 1024px

## Security Implementation

### Authentication System
- Session-based authentication
- Secure password storage (plain text for demo purposes)
- Automatic session timeout (30 minutes)
- Login attempt tracking

### Authorization Framework
- Role-based access control (RBAC)
- Four distinct user roles with specific permissions
- Action-level authorization checks
- Data isolation between roles

### Data Protection
- Input validation on all endpoints
- File upload restrictions (type and size)
- XSS prevention through proper encoding
- CSRF protection with anti-forgery tokens

### Security Headers
- Content Security Policy (CSP)
- X-Frame-Options denial
- X-Content-Type-Options nosniff
- Strict-Transport-Security (when using HTTPS)

## Testing Strategy

### Unit Testing
- Controller action testing with mocked dependencies
- Model validation testing
- Service layer unit tests
- Custom extension method tests

### Test Coverage Areas
- User authentication and registration
- Claim submission and validation
- Approval workflow logic
- HR analytics calculations
- File upload processing

### Test Data Management
- Mock services for isolated testing
- In-memory session implementation
- Test-specific data initialization
- Clean test environment setup

### Example Test Structure
```csharp
public class ClaimsControllerTests
{
    private readonly Mock<TextFileDataService> _dataServiceMock;
    private readonly Mock<ILogger<ClaimsController>> _loggerMock;
    private readonly ClaimsController _controller;

    [Fact]
    public void Submit_Get_ReturnsViewWithViewModel()
    {
        // Arrange
        // Act
        var result = _controller.Submit();
        
        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClaimSubmissionViewModel>(viewResult.Model);
    }
}
```

## Deployment Guide

### Development Environment Setup

#### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Git version control

#### Setup Steps
1. Clone the repository
2. Restore NuGet packages
3. Build the solution
4. Run the application
5. Access via http://localhost:5000

### Configuration

#### AppSettings.json Structure
```json
{
  "ConnectionStrings": {
    "CMCSDatabase": "Server=localhost;Database=CMCS_Database;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "FileUpload": {
    "MaxFileSize": 5242880,
    "AllowedExtensions": [ ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" ]
  }
}
```

### Production Considerations

#### Security Hardening
- Enable HTTPS in production
- Implement proper password hashing
- Configure secure session settings
- Set up proper file permissions

#### Performance Optimization
- Implement response compression
- Configure static file caching
- Optimize text file read/write operations
- Set up proper logging levels

#### Monitoring and Maintenance
- Regular log review
- File system cleanup
- Backup procedures for text files
- Performance monitoring

### Troubleshooting

#### Common Issues
1. **Port conflicts**: Change ports in launchSettings.json
2. **File permissions**: Ensure write access to Data directory
3. **Session issues**: Clear browser cookies and cache
4. **Upload failures**: Check file size and type restrictions

#### Diagnostic Tools
- Server status page (/Server/Status)
- Database connection testing (/Test/TestConnection)
- System health checks (/health)

## Maintenance Procedures

### Regular Tasks
- Monitor disk space for text file storage
- Review application logs for errors
- Backup Data directory regularly
- Update dependencies and security patches

### Data Management
- Archive old claims periodically
- Clean up temporary files
- Monitor file system performance
- Implement data retention policies

This documentation provides comprehensive technical information for 
developers, system administrators, and maintainers of the Contract 
Monthly Claim System. The modular architecture and clear separation 
of concerns make the system maintainable and extensible for future 
enhancements.

---