# Contract Monthly Claim System (CMCS)

## Table of Contents
1. [Overview](#overview)
2. [System Architecture](#system-architecture)
3. [Project Structure](#project-structure)
4. [Installation Guide](#installation-guide)
5. [Usage Manual](#usage-manual)
6. [Features](#features)
7. [Technology Stack](#technology-stack)
8. [Development Notes](#development-notes)
9. [Documentation](#documentation)
10. [Notes](#notes)
11. [Disclaimer](#disclaimer)

## Overview

The Contract Monthly Claim System (CMCS) is a comprehensive web-based application developed using .NET Core MVC framework. 
This system streamlines the process of submitting, reviewing, and approving monthly claims for independent contractor lecturers. 
The application provides three distinct user roles with specific functionalities to ensure an efficient workflow management system.

Repository Link: https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git

## System Architecture

The CMCS follows a Model-View-Controller (MVC) architectural pattern with clear separation of concerns:

- **Models**: Data entities and view models handling business logic and data validation
- **Views**: Razor pages providing user interface components
- **Controllers**: Handle user requests, process data, and return appropriate views

The system implements session-based authentication with role-based access control, ensuring secure access to 
appropriate functionalities based on user roles.

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── AuthController.cs
│   ├── ClaimsController.cs
│   └── HomeController.cs
├── Models/
│   ├── DataModels/
│   │   ├── User.cs
│   │   ├── Lecturer.cs
│   │   ├── Claim.cs
│   │   ├── Document.cs
│   │   └── Approval.cs
│   ├── ViewModels/
│   │   ├── LoginViewModel.cs
│   │   ├── RegisterViewModel.cs
│   │   └── ClaimViewModels/
│   │       ├── ClaimSubmissionViewModel.cs
│   │       ├── ClaimApprovalViewModel.cs
│   │       └── DocumentViewModel.cs
│   └── ErrorViewModel.cs
├── Views/
│   ├── Auth/
│   │   ├── Index.cshtml
│   │   └── ForgotPassword.cshtml
│   ├── Claims/
│   │   ├── Submit.cshtml
│   │   ├── Approve.cshtml
│   │   ├── Status.cshtml
│   │   └── Track.cshtml
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   └── Shared/
│       ├── _Layout.cshtml
│       ├── _Layout.cshtml.css
│       ├── _ValidationScriptsPartial.cshtml
│       └── Error.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   └── site.js
│   ├── lib/
│   └── favicon.ico
├── Documentation/
│   ├── Project_Plan.md
│   ├── Documentation.md
│   ├── DOCUMENTATION.docx
│   └── DOCUMENTATION.pdf
├── appsettings.json
└── Program.cs
```

## Installation Guide

### Prerequisites
- .NET Core SDK 3.1 or later
- Web browser (Chrome, Firefox, Safari, or Edge)
- Git client (for cloning repository)

### Installation Steps
1. Clone the repository:
   ```
   git clone https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git
   ```

2. Navigate to the project directory:
   ```
   cd contract-monthly-claim-system-cs
   ```

3. Restore NuGet packages:
   ```
   dotnet restore
   ```

4. Build the application:
   ```
   dotnet build
   ```

5. Run the application:
   ```
   dotnet run
   ```

6. Open your web browser and navigate to:
   ```
   https://localhost:7000 (or the port shown in the console)
   ```

## Usage Manual

### User Authentication
1. Access the login page through the main application URL
2. Use one of the predefined test accounts:
   - Lecturer: username: `lecturer`, password: `lecturer123`
   - Coordinator: username: `coordinator`, password: `coordinator123`
   - Manager: username: `admin`, password: `admin123`

### For Lecturers
1. Login with lecturer credentials
2. Navigate to "Submit" section
3. Fill in hours worked and hourly rate
4. Add optional comments
5. Upload supporting documents
6. Submit claim for review

### For Coordinators
1. Login with coordinator credentials
2. Navigate to "Review" section
3. View pending claims
4. Verify claim details and documents
5. Add approval comments if needed

### For Academic Managers
1. Login with manager credentials
2. Navigate to "Review" section
3. Approve or reject verified claims
4. Provide final decision comments

### Tracking Claims
All users can track claim status through the "Track" section, which shows complete history and current status of all claims.

## Features

### Core Functionality
- Role-based user authentication system
- Claim submission with automatic amount calculation
- Document upload with file validation
- Multi-stage approval workflow
- Real-time status tracking
- Comments system for communication

### User Interface
- Minimalist Apple-inspired design
- Responsive layout for desktop and mobile
- Intuitive navigation system
- Clean typography and visual hierarchy
- Accessibility features including keyboard navigation

### Technical Features
- Session management and security
- Client-side form validation
- File type and size validation
- Dynamic content updates
- Error handling and user feedback

## Technology Stack

### Backend Technologies
- ASP.NET Core MVC
- C# 7.0
- Session-based authentication
- Model-View-Controller architecture

### Frontend Technologies
- HTML5 with Razor syntax
- CSS3 with custom properties
- JavaScript for client-side functionality
- Responsive design principles

### Development Tools
- .NET Core SDK
- Git version control
- Modern web browsers for testing

## Development Notes

### Code Standards
- Allman style bracketing throughout codebase
- C# 7.0 language features
- Consistent naming conventions
- Comprehensive code comments

### Prototype Limitations
This initial version focuses on core functionality without:
- Database persistence (uses in-memory storage)
- Email notifications
- Advanced reporting features
- External system integrations
- Administrative user management

### Future Enhancement Areas
- Database integration with Entity Framework
- Automated email notifications
- Advanced reporting dashboard
- Enhanced security features
- Administrative controls
- API development for external integrations

## Documentation

The Documentation folder contains comprehensive project documentation:

- **Project_Plan.md**: Detailed project timeline, task breakdown, and milestones
- **Documentation.md**: Technical documentation including system architecture and API references
- **DOCUMENTATION.docx**: Complete project documentation in Microsoft Word format
- **DOCUMENTATION.pdf**: Portable document format version of complete documentation

## Notes

1. This is a prototype version designed for demonstration and testing purposes
2. All data is stored in memory and will be lost on application restart
3. The system includes sample data for testing all functionalities
4. File uploads are simulated for the prototype phase

## DISCLAIMER

UNDER NO CIRCUMSTANCES SHOULD IMAGES OR EMOJIS BE INCLUDED DIRECTLY 
IN THE README FILE. ALL VISUAL MEDIA, INCLUDING SCREENSHOTS AND IMAGES 
OF THE APPLICATION, MUST BE STORED IN A DEDICATED FOLDER WITHIN THE 
PROJECT DIRECTORY. THIS FOLDER SHOULD BE CLEARLY STRUCTURED AND NAMED 
ACCORDINGLY TO INDICATE THAT IT CONTAINS ALL VISUAL CONTENT RELATED TO 
THE APPLICATION (FOR EXAMPLE, A FOLDER NAMED IMAGES, SCREENSHOTS, OR MEDIA).

I AM NOT LIABLE OR RESPONSIBLE FOR ANY MALFUNCTIONS, DEFECTS, OR ISSUES 
THAT MAY OCCUR AS A RESULT OF COPYING, MODIFYING, OR USING THIS SOFTWARE. 
IF YOU ENCOUNTER ANY PROBLEMS OR ERRORS, PLEASE DO NOT ATTEMPT TO FIX THEM 
SILENTLY OR OUTSIDE THE PROJECT. INSTEAD, KINDLY SUBMIT A PULL REQUEST 
OR OPEN AN ISSUE ON THE CORRESPONDING GITHUB REPOSITORY, SO THAT IT CAN 
BE ADDRESSED APPROPRIATELY BY THE MAINTAINERS OR CONTRIBUTORS.