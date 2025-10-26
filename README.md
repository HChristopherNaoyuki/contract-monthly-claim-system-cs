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

The Contract Monthly Claim System (CMCS) is a comprehensive .NET web-based 
application designed to streamline the process of submitting and approving 
monthly claims for Independent Contractor (IC) lecturers. This system 
provides a seamless platform for claim management with automated workflows, 
comprehensive analytics, and role-based access control.

This implementation represents the complete Portfolio of Evidence (POE) 
submission for PROG6212, addressing all three parts of the assignment 
requirements with a strong focus on Part 3 automation features.

**Repository Link:** https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git

## System Architecture

### Design Pattern
- **MVC (Model-View-Controller)** architecture
- **Repository Pattern** with Text File Data Service
- **Role-Based Access Control** (RBAC)
- **Layered Architecture** with separation of concerns

### Data Storage
- **Text File Storage** instead of traditional database
- JSON-based serialization for data persistence
- Automated backup and recovery mechanisms
- File-based data integrity validation

### Security Features
- Session-based authentication
- Role-based authorization
- Anti-forgery token protection
- Input validation and sanitization

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── AuthController.cs          # Authentication and user management
│   ├── ClaimsController.cs        # Core claims processing with automation
│   ├── HomeController.cs          # Public pages and navigation
│   ├── ServerController.cs        # System diagnostics and troubleshooting
│   └── TestController.cs          # Database and system testing
├── Models/
│   ├── DataModels/                # Entity models
│   │   ├── User.cs
│   │   ├── Claim.cs
│   │   ├── Lecturer.cs
│   │   ├── Document.cs
│   │   └── Approval.cs
│   ├── ViewModels/                # View-specific models
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   ├── HRDashboardViewModel.cs
│   │   └── ClaimViewModels/
│   └── ErrorViewModel.cs
├── Services/
│   └── TextFileDataService.cs     # Text file data operations
├── Views/                         # Razor views with Apple-like aesthetics
├── wwwroot/
│   ├── css/site.css              # Custom CSS with Apple design system
│   ├── js/site.js                # Client-side functionality
│   └── uploads/                  # Document storage
├── Data/                         # Text file database storage
├── Documentation/                # Project documentation
└── Tests/                        # Comprehensive unit tests
```

## Installation Guide

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Git for version control

### Setup Instructions

1. **Clone the Repository**
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

5. **Access the Application**
   - Open browser and navigate to: `http://localhost:5000`
   - Default login credentials:
     - Admin: `admin` / `admin123`
     - Lecturer: `lecturer` / `lecturer123`
     - Coordinator: `coordinator` / `coordinator123`

### Troubleshooting
- If port 5000 is unavailable, check `launchSettings.json` for alternative ports
- Ensure write permissions for the `Data` and `wwwroot/uploads` directories
- Run `dotnet dev-certs https --trust` for SSL certificate issues

## Usage Manual

### User Roles and Permissions

1. **Lecturers**
   - Submit monthly claims with supporting documents
   - Track claim status in real-time
   - View personal claim history
   - Upload supporting documentation

2. **Programme Coordinators**
   - Review and approve/reject claims
   - Access automated claim verification
   - View all pending claims in their department
   - Provide approval comments

3. **Academic Managers**
   - Final approval authority for high-value claims
   - Access comprehensive HR analytics dashboard
   - Generate automated reports
   - System administration capabilities

### Claim Submission Process

1. **Login** to the system with your credentials
2. **Navigate** to "Submit Claim" from the main menu
3. **Enter** hours worked and hourly rate (auto-calculates amount)
4. **Upload** supporting documents (PDF, DOC, DOCX, JPG, PNG)
5. **Submit** the claim for approval
6. **Track** status through the approval workflow

### Approval Workflow

1. **Submission** → Claim enters the system
2. **Coordinator Review** → Initial verification and approval
3. **Manager Approval** → Required for claims over R5,000
4. **Final Status** → Approved, Rejected, or Paid
5. **Notification** → Automated status updates to lecturers

## Features

### Part 1: Project Planning and Prototype (100 Marks)
- Complete project documentation and design specifications
- UML Class Diagram for database structure
- Comprehensive project plan with timelines
- Non-functional GUI prototype with Apple-like aesthetics
- Version control implementation with GitHub

### Part 2: Prototype Implementation (100 Marks)
- Functional claim submission system
- Role-based access control implementation
- Document upload and management
- Real-time claim status tracking
- Comprehensive error handling and validation
- Unit testing implementation with xUnit

### Part 3: POE - Automation Enhancement (100 Marks)

#### Automation Features
- **Auto-calculation** of claim amounts with overtime consideration
- **Automated validation** with business rule enforcement
- **Intelligent claim analysis** for approvers
- **Multi-level approval workflows**
- **Automated notifications** and status updates
- **Comprehensive HR analytics** and reporting
- **Automated document processing** with validation
- **System monitoring** and performance analytics

#### HR Dashboard Features
- Real-time system statistics and KPIs
- Top performer analytics
- Monthly breakdown and trend analysis
- Automated report generation
- Batch payment processing simulation
- Data export capabilities

#### Technical Automation
- Automated data backup and recovery
- Text file storage optimization
- Performance monitoring
- Error recovery mechanisms
- Data integrity validation

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 7.0
- **Authentication**: Session-based with custom implementation
- **Data Storage**: Text files with JSON serialization
- **Logging**: Microsoft.Extensions.Logging

### Frontend
- **UI Framework**: Razor Pages
- **Styling**: Custom CSS with Apple design principles
- **JavaScript**: Vanilla JS for enhanced interactivity
- **Icons**: SVG and Unicode symbols

### Testing
- **Framework**: xUnit 2.5.3
- **Mocking**: Moq framework
- **Coverage**: Comprehensive unit test suite

### Development Tools
- **IDE**: Visual Studio 2022
- **Version Control**: Git with GitHub
- **Package Management**: NuGet

## Development Notes

### Design Principles
- **Minimalistic Apple-like aesthetics** for clean, intuitive user experience
- **Text file storage** implementation as per assignment requirements
- **Comprehensive error handling** with user-friendly messages
- **Role-based security** with proper access controls
- **Responsive design** for various screen sizes

### Key Implementation Details

1. **Text File Data Service**
   - Custom implementation replacing Entity Framework
   - JSON serialization for data persistence
   - Automated backup and recovery
   - Thread-safe operations

2. **Automation Engine**
   - Business rule validation system
   - Workflow automation for approvals
   - Notification system simulation
   - Analytics and reporting automation

3. **Security Implementation**
   - Custom session management
   - Anti-forgery token protection
   - Input validation and sanitization
   - Secure file upload handling

### Performance Considerations
- Efficient text file operations with buffering
- Session state optimization
- Client-side validation to reduce server load
- Asynchronous file operations

## Documentation

### Technical Documentation
- Comprehensive code comments throughout the solution
- XML documentation for all public methods and classes
- Architecture decision records
- Data model specifications

### User Documentation
- System administration guide
- User manual for each role type
- Troubleshooting guide
- API documentation (where applicable)

### Assessment Documentation
- POE requirements mapping document
- Rubric alignment documentation
- Test case specifications
- Implementation evidence

## Notes

### Assignment Compliance
- **No database connection** - Uses text file storage as required
- **xUnit 2.5.3** for comprehensive unit testing
- **Apple-like aesthetics** implemented throughout the UI
- **Part 3 POE focus** with extensive automation features
- **All three parts** completed as per assignment requirements

### Implementation Highlights
- Complete MVC implementation following Allman style formatting
- Comprehensive error handling and validation
- Role-based access control system
- Automated workflow management
- Extensive unit test coverage
- Professional documentation

### Sample Data
The system includes pre-configured sample data for demonstration:
- Admin user with full system access
- Lecturer account for claim submission testing
- Coordinator account for approval workflow testing
- Sample claims and documents

## Disclaimer

This project is submitted as a Portfolio of Evidence (POE) for the PROG6212 
Programming 2B module at The Independent Institute of Education. The 
implementation focuses on academic requirements and demonstration of 
technical competencies.

### Academic Integrity
- All code is original work created for this assessment
- Proper attribution has been provided for any referenced resources
- AI tools were used for initial brainstorming and proofreading as disclosed
- The implementation follows all academic integrity guidelines

### Usage Restrictions
- This system is for academic demonstration purposes only
- Not intended for production use without significant security enhancements
- Data persistence uses text files and may not be suitable for high-volume environments
- Users should implement additional security measures for real-world deployment

### Technical Limitations
- Text file storage has inherent limitations for concurrent access
- Session-based authentication may require enhancement for production use
- File upload functionality should include additional security validation
- Error handling should be enhanced for production environments

For any questions or concerns regarding this implementation, please contact 
the development team through the educational institution's proper channels.

---