# Contract Monthly Claim System (CMCS)

## Overview

The Contract Monthly Claim System (CMCS) is a web application built with .NET that helps 
manage monthly claims for contract lecturers. The system makes it easier to submit, 
review, and approve claims through an automated process.

## System Design

### Main Components
- ASP.NET Core MVC 8.0 - Web framework
- Text File Storage - No database needed
- Role-Based Access - Different access levels
- Clean User Interface - Simple and easy to use
- Testing Framework - xUnit for testing

### Project Layout
```
contract-monthly-claim-system-cs/
├── Controllers/          # Application controllers
├── Models/               # Data structures
├── Views/                # Web pages
├── Services/             # Business logic
├── wwwroot/              # Website files
├── Data/                 # File storage
└── Tests/                # Test files
```

## Setup Instructions

### What You Need
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Git

### Quick Setup
1. **Get the Code**
   ```bash
   git clone https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git
   cd contract-monthly-claim-system-cs
   ```

2. **Build the Project**
   ```bash
   dotnet build
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Open in Browser**
   - Go to: `http://localhost:5000`
   - Use test accounts below

### Test Accounts
| Role | Username | Password | What They Can Do |
|------|----------|----------|------------------|
| Lecturer | lecturer | lecturer123 | Submit and track claims |
| Coordinator | coordinator | coordinator123 | Review and approve claims |
| Manager | admin | admin123 | Final approval and reports |
| HR | hr | hr123 | HR tasks and analytics |

## How to Use

### For Lecturers
1. Login with lecturer account
2. Fill out claim form
3. Upload supporting files
4. Check claim status
5. View claim history

### For Coordinators
1. Check pending claims
2. Approve or reject claims
3. Add comments
4. Monitor progress

### For Managers and HR
1. View analytics dashboard
2. Create PDF reports
3. Edit claims if needed
4. See performance data

## Features

### Automation Features

#### Automatic Claim Processing
- Automatic amount calculations
- Real-time error checking
- Multi-step approval system
- File handling and storage

#### Analytics Dashboard
- Live statistics and metrics
- Top performer tracking
- Monthly trend analysis
- Department comparisons

#### User Experience
- Clean, simple design
- Works on all devices
- Live status updates
- Easy navigation

#### Security
- Four user roles
- Secure login system
- Data protection by role
- Safe file uploads

### Main System Features

#### Claim Management
- Automated claim submission
- Multi-level approvals
- File uploads
- Status tracking
- Activity history

#### Reports and Analytics
- PDF report creation
- Performance tracking
- Financial reports
- Trend analysis
- Data exports

#### Admin Tools
- User management
- Claim editing (HR only)
- System monitoring
- Data export
- Backup systems

## Technology Used

### Backend
- ASP.NET Core 8.0
- C# 12.0
- Text File Storage
- xUnit testing

### Frontend
- Razor Pages
- Custom CSS
- JavaScript
- Mobile-friendly design

### Development
- Visual Studio 2022
- Git
- xUnit
- Moq testing

## Development Information

### Design Choices

#### System Structure
- MVC pattern for organization
- Service layer for business rules
- Text files for data storage
- Session-based login

#### Automation Features
- Automatic calculations with overtime
- Data validation systems
- Automated approval workflows
- Analytics generation

#### User Interface
- Clean, simple design
- Accessible for all users
- Instant feedback
- Consistent navigation

### Testing Approach
- Unit tests for controllers
- Model validation tests
- Workflow integration tests
- Mock services for testing

## Documentation

### Main Controllers

#### AuthController
Handles user login, registration, and session management.

#### ClaimsController
Manages claim submission, approvals, analytics, and automation.

#### HomeController
Provides main pages and navigation.

### Data Structures

#### User Roles
```csharp
public enum UserRole
{
    Lecturer = 0,
    ProgrammeCoordinator = 1,
    AcademicManager = 2,
    HumanResource = 3
}
```

#### Claim Status
```csharp
public enum ClaimStatus
{
    Submitted = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3,
    Paid = 4
}
```

### Services

#### TextFileDataService
Handles file storage with automated data management and reporting.

#### DatabaseService
Provides connection testing for future database use.

## Important Notes

### Part 3 (POE): Requirements Completed

YES - Automated Claim Submission
- Automatic calculations implemented
- Data validation checks
- Client-side calculations

YES - Automated Verification & Approval
- Criteria checking system
- Streamlined approval workflows
- Multi-level approval process

YES - HR View Automation
- Automated claim processing
- Lecturer data management
- Report generation

YES - Better User Experience
- Clean, simple design
- Easy-to-use interface
- Live status tracking

### Performance
- Text file storage optimized
- Efficient session management
- Smart caching
- Fast algorithms

### Security
- Input validation on all forms
- Safe file upload limits
- Role-based access control
- Session timeouts

## Legal Information

### Software Usage
DO NOT PUT IMAGES OR EMOJIS DIRECTLY IN THE README FILE. ALL PICTURES, INCLUDING SCREENSHOTS AND 
APPLICATION IMAGES, MUST BE KEPT IN A SEPARATE FOLDER WITHIN THE PROJECT. THIS FOLDER SHOULD BE 
CLEARLY NAMED TO SHOW IT CONTAINS ALL VISUAL CONTENT FOR THE APPLICATION (FOR EXAMPLE, A FOLDER 
CALLED IMAGES, SCREENSHOTS, OR MEDIA).

### Responsibility Notice
I AM NOT RESPONSIBLE FOR ANY PROBLEMS, ERRORS, OR ISSUES THAT HAPPEN WHEN COPYING, CHANGING, OR 
USING THIS SOFTWARE. IF YOU FIND ANY BUGS OR ERRORS, PLEASE DO NOT TRY TO FIX THEM QUIETLY OR 
OUTSIDE THIS PROJECT. INSTEAD, PLEASE SUBMIT A PULL REQUEST OR CREATE AN ISSUE ON THE GITHUB 
REPOSITORY, SO IT CAN BE FIXED PROPERLY BY THE PROJECT MAINTAINERS.

---
