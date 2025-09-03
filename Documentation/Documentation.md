# Contract Monthly Claim System (CMCS) - Technical Documentation
# Part 1: Project Planning and Prototype Development

## System Overview

The Contract Monthly Claim System (CMCS) is a web-based application designed to 
streamline the monthly claim submission and approval process for Independent 
Contractor lecturers in educational institutions. This documentation covers the 
technical specifications, architecture, and implementation details for Part 1 of the project.

## Table of Contents
1. [System Architecture](#system-architecture)
2. [UML Diagrams](#uml-diagrams)
3. [Technical Specifications](#technical-specifications)
4. [Implementation Details](#implementation-details)
5. [API Reference](#api-reference)
6. [Deployment Guide](#deployment-guide)
7. [References](#references)

## System Architecture

### MVC Architecture Pattern
The application follows the Model-View-Controller architectural pattern:

**Models**:
- Business entities (User, Lecturer, Claim, Document, Approval)
- ViewModels for specific UI requirements
- Data validation and business logic

**Views**:
- Razor pages for dynamic content rendering
- Layout templates for consistent UI
- Partial views for reusable components

**Controllers**:
- AuthController: Handles authentication operations
- ClaimsController: Manages claim-related operations
- HomeController: Handles navigation and static content

### Authentication Flow
1. User accesses application → Redirected to login page
2. User provides credentials → System validates against in-memory storage
3. Successful authentication → Session created with user role
4. Role-based redirect to appropriate dashboard

## UML Diagrams

### Class Diagram
```
+----------------+       +----------------+       +----------------+
|     User       |       |    Claim       |       |   Document     |
+----------------+       +----------------+       +----------------+
| - UserId: int  |       | - ClaimId: int |<>----| - DocumentId:int|
| - Name: string |       | - LecturerId:int|*     | - ClaimId: int |
| - Surname: str |       | - ClaimDate: dt|      | - FileName: str|
| - Username: str|       | - HoursWorked: |      | - FilePath: str|
| - Password: str|       | - HourlyRate:  |      | - UploadDate:dt|
| - Role: enum   |       | - Amount: dec  |      +----------------+
+----------------+       | - Status: enum |
        ^                +----------------+
        |                        |
        |                        |1
        |                        |
+----------------+       +----------------+
|   Lecturer     |       |   Approval     |
+----------------+       +----------------+
| - LecturerId:int|       | - ApprovalId:int|
| - FirstName: str|       | - ClaimId: int |
| - LastName: str |       | - ApproverRole:|
| - Email: string|       | - ApprovalDate:|
| - HourlyRate:dec|       | - IsApproved: |
+----------------+       | - Comments: str|
                         +----------------+

Relationships:
- User (1) --- (0..*) Claim
- Claim (1) --- (0..*) Document
- Claim (1) --- (0..*) Approval
- Lecturer inherits from User
```

### Use Case Diagram
```
[User] --> (Register Account)
[User] --> (Login to System)
[Lecturer] --> (Submit Claim)
[Lecturer] --> (Upload Documents)
[Lecturer] --> (Track Claim Status)
[Coordinator] --> (Review Claims)
[Manager] --> (Approve/Reject Claims)
[All Users] --> (View Privacy Policy)
[All Users] --> (Logout)
```

### Sequence Diagram - User Registration
```
User -> System: Access Registration Page
System -> User: Display Registration Form
User -> System: Submit Registration Data
System -> System: Validate Input Data
System -> System: Check Username Availability
System -> System: Create User Account
System -> User: Auto-Login & Redirect to Home
```

## Technical Specifications

### Development Environment
- **IDE**: Visual Studio 2022
- **Framework**: .NET 7.0
- **Language**: C# 7.0
- **Version Control**: Git

### Frontend Technologies
- HTML5 with Razor syntax
- CSS3 with custom styling
- JavaScript for client-side interactions
- Responsive design principles

### Backend Technologies
- ASP.NET Core MVC 7.0
- Session-based authentication
- In-memory data storage
- Model-View-Controller architecture

### Security Implementation
- Input validation and sanitization
- Session management
- Role-based access control
- Secure file upload handling
- Cross-site scripting (XSS) protection

## Implementation Details

### Authentication System
```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public UserRole Role { get; set; }
}

public enum UserRole
{
    Lecturer,
    ProgrammeCoordinator,
    AcademicManager
}
```

### Claim Management
```csharp
public class ClaimSubmissionViewModel
{
    [Required]
    [Range(0, 744)]
    public decimal HoursWorked { get; set; }
    
    [Required]
    [Range(0, 999.99)]
    public decimal HourlyRate { get; set; }
    
    public decimal Amount { get; set; }
    public List<IFormFile> Documents { get; set; }
}
```

### Session Management
```csharp
public static class SessionKeys
{
    public const string UserId = "UserId";
    public const string Username = "Username";
    public const string Name = "Name";
    public const string Role = "Role";
}
```

## API Reference

### Authentication Endpoints
- `GET /Auth` - Login/Registration page
- `POST /Auth/Login` - User authentication
- `POST /Auth/Register` - User registration
- `POST /Auth/Logout` - Session termination
- `GET /Auth/ForgotPassword` - Password recovery page

### Claim Management Endpoints
- `GET /Claims/Submit` - Claim submission form
- `POST /Claims/Submit` - Process claim submission
- `GET /Claims/Approve` - Claim approval interface
- `GET /Claims/Status` - Claim status tracking

### Home Endpoints
- `GET /Home` - Application homepage
- `GET /Home/Privacy` - Privacy policy page
- `GET /Home/Error` - Error handling page

## Deployment Guide

### Development Environment
1. Install .NET 7.0 SDK
2. Clone repository
3. Restore packages: `dotnet restore`
4. Build solution: `dotnet build`
5. Run application: `dotnet run`

### Build Process
```bash
dotnet clean
dotnet build
dotnet run
dotnet publish -c Release
```

### Test Cases
1. User registration with valid/invalid data
2. Login authentication success/failure
3. Claim submission with various inputs
4. Role-based navigation access
5. Session management and timeout

## References

### Project Documentation
- **README.md**: Complete project overview and setup instructions
- **Project_Plan.md**: Detailed project planning and timeline
- This document: Technical specifications and implementation details

### External References
- ASP.NET Core Documentation
- MVC Pattern
- HTML5 Specification
- CSS3 Guidelines

### Development Standards
- C# Coding Conventions
- ASP.NET Core Best Practices
- Web Content Accessibility Guidelines

## Support

For technical support or questions about the Contract Monthly Claim System:
1. Refer to the README.md file for setup instructions
2. Review this documentation for technical details
3. Check the project plan for development timeline
4. Contact the development team for specific issues

---