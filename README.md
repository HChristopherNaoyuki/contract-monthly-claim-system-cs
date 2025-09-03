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
9. [DISCLAIMER](#disclaimer)

## Overview

The Contract Monthly Claim System (CMCS) is a comprehensive web-based application developed using ASP.NET Core MVC. 
This system streamlines the process of submitting, reviewing, and approving monthly claims for Independent Contractor (IC) lecturers in educational institutions.

## System Architecture

The application follows the Model-View-Controller (MVC) architectural pattern with clear separation of concerns:

- **Models**: Data entities and view models
- **Views**: User interface components
- **Controllers**: Business logic and request handling

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── AuthController.cs          # Handles authentication (login/register)
│   ├── ClaimsController.cs        # Manages claim operations
│   └── HomeController.cs          # Handles home and privacy pages
├── Models/
│   ├── DataModels/                # Data entities
│   │   ├── User.cs                # User authentication model
│   │   ├── Lecturer.cs            # Lecturer entity
│   │   ├── Claim.cs               # Claim entity
│   │   ├── Document.cs            # Document entity
│   │   └── Approval.cs            # Approval entity
│   ├── ViewModels/                # View-specific models
│   │   ├── LoginViewModel.cs      # Login form model
│   │   ├── RegisterViewModel.cs   # Registration form model
│   │   └── ClaimViewModels/       # Claim-related view models
│   │       ├── ClaimSubmissionViewModel.cs
│   │       ├── ClaimApprovalViewModel.cs
│   │       └── DocumentViewModel.cs
│   └── ErrorViewModel.cs          # Error page model
├── Views/
│   ├── Auth/                      # Authentication views
│   │   ├── Index.cshtml           # Login/Register page
│   │   └── ForgotPassword.cshtml  # Password reset page
│   ├── Claims/                    # Claim management views
│   │   ├── Submit.cshtml          # Claim submission form
│   │   ├── Approve.cshtml         # Claim approval interface
│   │   └── Status.cshtml          # Claim status tracking
│   ├── Home/                      # Home controller views
│   │   ├── Index.cshtml           # Home page
│   │   └── Privacy.cshtml         # Privacy policy page
│   └── Shared/                    # Shared layout and components
│       ├── _Layout.cshtml         # Main layout template
│       ├── _Layout.cshtml.css     # Layout-specific styles
│       ├── _ValidationScriptsPartial.cshtml
│       └── Error.cshtml           # Error page
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Main stylesheet
│   ├── js/
│   │   └── site.js                # Client-side JavaScript
│   ├── lib/                       # Third-party libraries
│   └── favicon.ico
├── Documentation/
│   ├── Project_Plan.txt           # Project planning documentation
│   └── Documentation.txt          # System documentation
├── appsettings.json               # Application configuration
└── Program.cs                     # Application entry point
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
- Apple-inspired minimalist design
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
- **Styling**: Custom CSS with Apple design principles
- **Authentication**: Session-based with role management

## Development Notes

This project was developed following:
- Clean code principles with Allman-style formatting
- Proper separation of concerns
- User-centered design approach
- Accessibility considerations
- Performance optimization techniques

## Notes

*This system is designed for educational purposes as part of the PROG6212 Programming 2B curriculum. All design and implementation follow academic standards and best practices.*

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
