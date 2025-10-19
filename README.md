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

The **Contract Monthly Claim System (CMCS)** is a comprehensive web-based application designed 
to streamline the monthly claim submission and approval process for independent contractor 
lecturers. This system addresses complex administrative challenges through an intuitive interface 
that serves three distinct user roles: lecturers, program coordinators, and academic managers.

**GitHub Repository:** [https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git](https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git)

## System Architecture

The prototype is built on **ASP.NET Core MVC framework** using **C# 7.0**, following industry-standard 
development practices. The architecture employs the **Model-View-Controller pattern**, ensuring clear 
separation of concerns and maintainable code structure.

### Key Technical Components:
- **Session-based authentication** with role-based access control
- **Text file storage system** replacing traditional database dependencies
- **Client-side validation** with jQuery and data annotations
- **In-memory data storage** for the prototype phase
- **Modern CSS features** including Grid and Flexbox for responsive design

### Data Model:
The system uses a comprehensive UML class diagram featuring key entities:
- **User Class**: Manages authentication with attributes for user identification and role assignment
- **Lecturer Class**: Extends user functionality with specific attributes for contract details
- **Claim Class**: Core entity handling claims submissions with properties for hours worked and status tracking
- **Document Class**: Manages supporting file uploads and associations with specific claims
- **Approval Class**: Tracks review processes with timestamps, decisions, and approver comments

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── AuthController.cs          # Authentication & user management
│   ├── ClaimsController.cs        # Claim submission & approval
│   ├── HomeController.cs          # Public pages & navigation
│   ├── ServerController.cs        # Server diagnostics
│   └── TestController.cs          # Database testing
├── Models/
│   ├── DataModels/                # Core business entities
│   │   ├── User.cs
│   │   ├── Lecturer.cs
│   │   ├── Claim.cs
│   │   ├── Document.cs
│   │   └── Approval.cs
│   ├── ViewModels/                # UI-specific models
│   │   ├── LoginViewModel.cs
│   │   └── RegisterViewModel.cs
│   └── ClaimViewModels/           # Claim-related view models
├── Services/
│   └── TextFileDataService.cs     # Text file database implementation
├── Views/                         # Razor views
├── wwwroot/
│   ├── css/site.css              # Apple-inspired design system
│   └── js/site.js                # Client-side functionality
├── Data/                         # Text file storage directory
├── Documentation/                # Project documentation
└── Program.cs                    # Application entry point
```

## Installation Guide

### Prerequisites
- .NET 8.0 SDK or later
- Visual Studio 2022 or VS Code
- Web browser (Chrome, Firefox, Safari, or Edge)

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

3. **Build the Application**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

5. **Access the Application**
   - Open your web browser
   - Navigate to: `http://localhost:5000`
   - Use default credentials

### Default User Accounts
- **Academic Manager**: `admin` / `admin123`
- **Lecturer**: `lecturer` / `lecturer123`
- **Program Coordinator**: `coordinator` / `coordinator123`

## Usage Manual

### For Lecturers

1. **Login & Navigation**
   - Access the system at `http://localhost:5000`
   - Login with lecturer credentials
   - Navigate to "Submit Claim" from the main menu

2. **Submit a Claim**
   - Enter hours worked and hourly rate
   - Add optional comments (max 500 characters)
   - Upload supporting documents (PDF, DOC, DOCX, JPG, PNG)
   - Review calculated amount and submit

3. **Track Claim Status**
   - Use "Track Claims" to view submission history
   - Monitor approval progress in real-time
   - View approval comments and status updates

### For Program Coordinators & Academic Managers

1. **Review Claims**
   - Login with coordinator or manager credentials
   - Navigate to "Review Claims"
   - View all pending claims requiring approval

2. **Approve/Reject Claims**
   - Review claim details and supporting documents
   - Click "Approve" or "Reject" with optional comments
   - Monitor approval workflow progress

### File Upload Guidelines
- **Supported Formats**: PDF, DOC, DOCX, JPG, PNG
- **Maximum File Size**: 5MB per file
- **Multiple Files**: Upload multiple documents per claim
- **Security**: Files are stored securely with unique names

## Features

### ✅ Core Functionality
- **Role-based Authentication**: Secure login for lecturers, coordinators, and managers
- **Claim Submission**: Intuitive form with real-time amount calculation
- **Document Upload**: Support for multiple file types with validation
- **Approval Workflow**: Streamlined review and approval process
- **Status Tracking**: Real-time claim status monitoring
- **Text File Storage**: Database-free data persistence

### ✅ User Experience
- **Apple-inspired Design**: Minimalist, clean interface
- **Responsive Layout**: Optimized for desktop and mobile devices
- **Real-time Validation**: Immediate feedback on form inputs
- **Interactive Elements**: Smooth animations and transitions
- **Accessibility**: Keyboard navigation and screen reader support

### ✅ Technical Features
- **Session Management**: Secure user state persistence
- **Error Handling**: Comprehensive validation and error messages
- **File Management**: Secure document storage and retrieval
- **Data Integrity**: JSON-based serialization with validation
- **Unit Testing**: xUnit test coverage for critical components

## Technology Stack

### Backend
- **Framework**: ASP.NET Core MVC 8.0
- **Language**: C# 7.0
- **Storage**: Text files with JSON serialization
- **Session**: Distributed memory cache
- **Validation**: Data annotations and model state

### Frontend
- **UI Framework**: Razor Pages with Bootstrap-inspired CSS
- **Styling**: Custom CSS with Apple design principles
- **JavaScript**: Vanilla JS for interactive features
- **Icons**: SVG icons for crisp rendering

### Development Tools
- **Testing**: xUnit 2.5.3
- **Logging**: Microsoft.Extensions.Logging
- **Package Management**: NuGet
- **IDE**: Visual Studio 2022/VS Code compatible

## Development Notes

### Text File Database Implementation
The system uses a custom `TextFileDataService` that replaces traditional database dependencies:

```csharp
// Example data operations
var users = _dataService.GetAllUsers();
var claim = _dataService.GetClaimById(claimId);
_dataService.SaveUser(newUser);
```

### Key Design Decisions
1. **No Database Dependencies**: Uses text files for complete portability
2. **JSON Serialization**: Human-readable data storage format
3. **Automatic File Creation**: Data files created on first run
4. **Sample Data**: Pre-populated with demo users and claims

### Session Management
Custom session extensions provide strongly-typed session storage:

```csharp
// Session usage examples
HttpContext.Session.SetSessionInt("UserId", user.UserId);
var userId = HttpContext.Session.GetSessionInt("UserId");
```

## Documentation

### Project Documentation
- **UML Diagrams**: Complete class diagrams in Mermaid format
- **API Documentation**: Inline XML comments throughout codebase
- **User Guides**: Step-by-step usage instructions
- **Technical Specifications**: Architecture and implementation details

### Development Documentation
- **Code Standards**: Allman style formatting with comprehensive comments
- **Testing Strategy**: Unit and integration test coverage
- **Deployment Guide**: Setup and configuration instructions

## Notes

### Part 1 vs Part 2 Implementation
- **Part 1**: Focused on prototype design, UML diagrams, and non-functional UI
- **Part 2**: Enhanced with full functionality, text file storage, and document upload

### Key Improvements in Part 2
1. **Functional Document Upload**: Complete file handling system
2. **Text File Database**: Eliminated database dependencies
3. **Enhanced UI/UX**: Apple-inspired design implementation
4. **Comprehensive Testing**: xUnit test coverage
5. **Error Handling**: Robust validation and user feedback

### Performance Considerations
- Text file storage suitable for small to medium datasets
- File uploads stored in `wwwroot/uploads` directory
- Session-based authentication with 30-minute timeout
- Client-side validation reduces server load

## Disclaimer

This application is developed as part of an academic project for **PROG6212: Programming 2B**. It represents a prototype system designed to demonstrate web application development skills using ASP.NET Core MVC and modern software engineering practices.

### Important Notes:
- **Educational Purpose**: This system is for demonstration and learning purposes
- **Data Security**: While security measures are implemented, production deployment would require additional security hardening
- **Scalability**: Text file storage is suitable for prototyping but may require database migration for production use
- **Browser Compatibility**: Tested on modern browsers but may have limitations on older versions

### License
This project is developed for academic purposes. 
All code and documentation are provided as-is 
for educational reference.

---