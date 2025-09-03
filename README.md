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

The Contract Monthly Claim System (CMCS) is a comprehensive web-based application developed 
using ASP.NET Core MVC. This system streamlines the process of submitting, reviewing, and 
approving monthly claims for Independent Contractor (IC) lecturers in educational institutions.

## System Architecture

The application follows the Model-View-Controller (MVC) architectural pattern with clear separation 
of concerns. For detailed architecture documentation, see the [Technical Documentation](./Documentation/Documentation.md).

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
│   │   └── Status.cshtml
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
│   └── Documentation.md
├── appsettings.json
└── Program.cs
```

## Installation Guide

### Prerequisites
- .NET 7.0 SDK or later
- Web browser with JavaScript support
- Git for version control

### Setup Instructions
1. Clone the repository
2. Restore NuGet packages: `dotnet restore`
3. Build the solution: `dotnet build`
4. Run the application: `dotnet run`
5. Access via: `https://localhost:7000` or `http://localhost:5000`

## Usage Manual

### For Lecturers
1. Register/Login with lecturer credentials
2. Submit claims with hours worked and hourly rate
3. Upload supporting documents
4. Track claim status in real-time

### For Coordinators/Managers
1. Login with appropriate credentials
2. Review and approve/reject claims
3. Monitor claim workflow
4. Manage user accounts

## Features

### Core Functionality
- User authentication and authorization
- Role-based access control (Lecturer, Coordinator, Manager)
- Claim submission with editable hourly rates
- Document upload and management
- Real-time status tracking
- Automated calculations

### User Interface
- Minimalist design approach
- Responsive layout for all devices
- Intuitive navigation system
- Clean typography and spacing

### Security Features
- Session-based authentication
- Input validation and sanitization
- Role-based access control
- Secure file upload handling

## Technology Stack

- **Backend**: ASP.NET Core 7.0, C# 7.0
- **Frontend**: HTML5, CSS3, JavaScript
- **Architecture**: MVC Pattern
- **Styling**: Custom CSS with clean design principles
- **Authentication**: Session-based with role management

## Development Notes

This project was developed following:
- Clean code principles with Allman-style formatting
- Proper separation of concerns
- User-centered design approach
- Accessibility considerations
- Performance optimization techniques

## Documentation

Comprehensive documentation is available in the Documentation folder:

- [Project Plan](./Documentation/Project_Plan.md) - Detailed project planning and timeline
- [Technical Documentation](./Documentation/Documentation.md) - System architecture and implementation details

## Notes

This system is designed for educational purposes as part of the PROG6212 Programming 2B curriculum. 
All design and implementation follow academic standards and best practices.

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

---