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

The Contract Monthly Claim System (CMCS) is a comprehensive web-based application designed 
to streamline the monthly claim submission and approval process for independent contractor 
lecturers. This system addresses complex administrative challenges through an intuitive, 
role-based interface that serves three distinct user roles: lecturers, program coordinators, 
and academic managers.

**Key Objectives:**
- Simplify claim submission for lecturers
- Streamline approval workflows for coordinators and managers
- Provide real-time status tracking and transparency
- Ensure secure document management
- Maintain audit trails for all claim activities

## System Architecture

The application follows the **Model-View-Controller (MVC)** pattern with clear separation of concerns:

**Frontend Layer:**
- ASP.NET Core Razor Views
- Modern CSS with Apple-inspired design
- Responsive design for all devices
- Client-side validation with JavaScript

**Business Logic Layer:**
- Controllers handling user requests
- ViewModels for data transfer
- Session-based authentication
- Role-based authorization

**Data Layer:**
- In-memory data storage (prototype phase)
- Entity Framework Core ready
- Scalable to SQL Server database

**Security Layer:**
- Session-based authentication
- Role-based access control
- Anti-forgery token protection
- Input validation and sanitization

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── AuthController.cs          # Authentication operations
│   ├── ClaimsController.cs        # Claim management
│   └── HomeController.cs          # Basic pages
├── Models/
│   ├── DataModels/                # Entity models
│   │   ├── User.cs
│   │   ├── Lecturer.cs
│   │   ├── Claim.cs
│   │   ├── Document.cs
│   │   └── Approval.cs
│   ├── ViewModels/                # View-specific models
│   │   ├── LoginViewModel.cs
│   │   └── RegisterViewModel.cs
│   └── ClaimViewModels/           # Claim-related view models
│       ├── ClaimSubmissionViewModel.cs
│       ├── ClaimApprovalViewModel.cs
│       └── DocumentViewModel.cs
├── Views/                         # Razor views
│   ├── Auth/                      # Authentication views
│   ├── Claims/                    # Claim management views
│   ├── Home/                      # Basic pages
│   └── Shared/                    # Layout and partial views
├── Extensions/
│   └── SessionExtensions.cs       # Session helper methods
├── wwwroot/                       # Static files
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   └── lib/                       # Third-party libraries
├── Program.cs                     # Application entry point
└── contract-monthly-claim-system-cs.csproj
```

## Installation Guide

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- Git for version control

### Step-by-Step Installation

1. **Clone the Repository**
   ```bash
   git clone https://github.com/your-username/contract-monthly-claim-system-cs.git
   cd contract-monthly-claim-system-cs
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Application**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access the Application**
   - Open browser and navigate to: `https://localhost:7000` or `http://localhost:5000`

### Configuration

**Development Environment:**
- No additional configuration required for prototype
- Uses in-memory data storage
- Default test users are pre-loaded

**Production Considerations:**
- Configure database connection string
- Set up proper session storage
- Configure email services for notifications
- Set up file storage for document uploads

## Usage Manual

### User Roles and Permissions

**Lecturer:**
- Submit monthly claims with hours worked and hourly rates
- Upload supporting documents (PDF, DOC, DOCX, JPG, PNG)
- Track claim status in real-time
- View submission history

**Programme Coordinator:**
- Review submitted claims
- Approve or reject claims with comments
- View all claims in the system
- Access claim tracking dashboard

**Academic Manager:**
- All coordinator permissions
- Final approval authority
- System oversight capabilities
- Access to comprehensive reports

### Step-by-Step Usage

**1. Authentication**
- Navigate to the login page
- Use pre-configured test accounts or register new account
- Select appropriate role during registration

**2. Claim Submission (Lecturers)**
- Click "Submit Claim" from dashboard
- Enter hours worked and hourly rate
- Add optional comments
- Upload supporting documents (optional)
- Submit for review

**3. Claim Approval (Coordinators/Managers)**
- Access "Review Claims" dashboard
- View pending claims with all details
- Approve or reject with comments
- Track decision history

**4. Status Tracking**
- Use "Track Claims" to view all claims
- Filter by status (Submitted, Approved, Rejected)
- View detailed claim information
- Monitor approval progress

### Key Features in Detail

**Real-time Calculations:**
- Automatic amount calculation based on hours and rate
- Instant validation feedback
- Client-side calculations for better UX

**Document Management:**
- Secure file upload with validation
- Support for multiple document types
- File size restrictions (5MB max)
- Secure file naming and storage

**Status Tracking:**
- Visual status indicators
- Real-time updates
- Comprehensive audit trail
- Email notifications (planned)

## Features

### Core Features
- ✅ **Role-based Authentication** - Secure login with session management
- ✅ **Claim Submission** - Intuitive form with real-time calculations
- ✅ **Document Upload** - Secure file upload with validation
- ✅ **Approval Workflow** - Multi-level approval process
- ✅ **Status Tracking** - Real-time claim status monitoring
- ✅ **Responsive Design** - Mobile-friendly interface

### Advanced Features
- ✅ **Auto-calculation** - Automatic amount calculation
- ✅ **Form Validation** - Client and server-side validation
- ✅ **Error Handling** - Comprehensive exception management
- ✅ **Session Management** - Secure user session handling
- ✅ **Security Headers** - Enhanced security protection

### Planned Features
- 🔄 **Email Notifications** - Automated status updates
- 🔄 **Database Integration** - Persistent data storage
- 🔄 **Reporting Dashboard** - Advanced analytics
- 🔄 **Bulk Operations** - Batch claim processing
- 🔄 **API Endpoints** - RESTful API for integration

## Technology Stack

### Backend Technologies
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 7.0+
- **Authentication**: Session-based with custom providers
- **Validation**: Data Annotations with client-side support

### Frontend Technologies
- **UI Framework**: Razor Pages with Bootstrap-inspired design
- **Styling**: Custom CSS with Apple design principles
- **JavaScript**: Vanilla JS for interactive features
- **Icons**: SVG and system fonts

### Development Tools
- **IDE**: Visual Studio 2022 / VS Code
- **Version Control**: Git with GitHub
- **Package Management**: NuGet
- **Testing**: xUnit (planned)

### Dependencies
- Microsoft.AspNetCore.Session (2.3.0)
- Microsoft.EntityFrameworkCore (9.0.8)
- Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation (8.0.0)

## Development Notes

### Code Standards
- **Formatting**: Allman style braces
- **Naming**: PascalCase for classes, camelCase for variables
- **Structure**: MVC pattern with separation of concerns
- **Documentation**: XML comments for public members

### Session Management
```csharp
// Session configuration in Program.cs
services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
```

### File Upload Configuration
- Maximum file size: 5MB per file
- Allowed types: PDF, DOC, DOCX, JPG, PNG
- Secure file naming with GUID prefixes
- Client and server-side validation

### Error Handling Strategy
- Global exception handling middleware
- User-friendly error messages
- Detailed logging for development
- Secure error page for production

## Documentation

### API Documentation
The system provides the following main endpoints:

**Authentication Endpoints:**
- `GET /Auth/Index` - Login/Register page
- `POST /Auth/Login` - User authentication
- `POST /Auth/Register` - User registration
- `POST /Auth/Logout` - Session termination

**Claim Management Endpoints:**
- `GET /Claims/Submit` - Claim submission form
- `POST /Claims/Submit` - Process claim submission
- `GET /Claims/Approve` - Approval dashboard
- `POST /Claims/ApproveClaim` - Process approval decision
- `GET /Claims/Status` - Claim status view
- `GET /Claims/Track` - Claim tracking dashboard

### Database Schema (Planned)
```sql
-- Users table
Users (UserId, Name, Surname, Username, Password, Role)

-- Claims table  
Claims (ClaimId, LecturerId, ClaimDate, HoursWorked, Amount, Status)

-- Documents table
Documents (DocumentId, ClaimId, FileName, FilePath, UploadDate)

-- Approvals table
Approvals (ApprovalId, ClaimId, ApproverRole, ApprovalDate, IsApproved, Comments)
```

### Deployment Guide

**Development Deployment:**
1. Clone repository
2. Restore dependencies
3. Build solution
4. Run application

**Production Deployment:**
1. Set up IIS or Azure App Service
2. Configure database connection
3. Set environment variables
4. Deploy compiled application
5. Configure SSL certificate

## Notes

### Current Limitations (Prototype Phase)
- Uses in-memory data storage (data lost on restart)
- Limited to three pre-configured user roles
- Basic file upload without persistent storage
- No email notification system
- Simplified approval workflow

### Security Considerations
- Session-based authentication suitable for prototype
- Input validation and sanitization implemented
- Anti-forgery tokens for form protection
- Secure file upload validation
- Role-based access control

### Performance Considerations
- Client-side calculations reduce server load
- Efficient session management
- Optimized database queries (when implemented)
- Static file caching strategies

### Browser Compatibility
- Chrome 90+ (Recommended)
- Firefox 88+
- Safari 14+
- Edge 90+
- Mobile browsers supported

## Disclaimer

### Development Status
This application is currently in **prototype development phase**. 
It is intended for educational and demonstration purposes only. 
Not recommended for production use without significant enhancements 
and security audits.

### Data Persistence
The current implementation uses in-memory data storage. All data will 
be lost when the application restarts. For production use, implement 
persistent database storage.

### Security Notice
While basic security measures are implemented, this prototype may not 
meet enterprise security standards. Conduct thorough security testing 
before deployment in production environments.

### Liability
The developers are not liable for any data loss, security breaches, 
or operational issues resulting from the use of this software. Users 
are responsible for implementing proper backup, security, and monitoring 
procedures.

### Third-party Dependencies
This project uses several third-party libraries and frameworks. Ensure 
you comply with all relevant licenses and maintain updated dependencies 
to address security vulnerabilities.

### Support
This is an educational project. No official support or maintenance is 
provided. Users are encouraged to review and understand the code before 
deployment.

---
