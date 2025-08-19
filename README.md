# Contract Monthly Claim System (CMCS)

## Table of Contents

1. [Overview](#overview)
2. [Key Features](#key-features)
3. [Technology Stack](#technology-stack)
4. [System Architecture](#system-architecture)
5. [Project Structure](#project-structure)
6. [Database Design](#database-design)
7. [Installation and Setup](#installation-and-setup)
   - [Prerequisites](#prerequisites)
   - [Setup Instructions](#setup-instructions)
   - [Configuration](#configuration)
8. [Usage Guide](#usage-guide)
   - [For Lecturers](#for-lecturers)
   - [For Coordinators and Managers](#for-coordinators-and-managers)
9. [Key Components](#key-components)
   - [Controllers](#controllers)
   - [View Models](#view-models)
   - [Data Models](#data-models)
10. [Styling and Design](#styling-and-design)
11. [Validation and Error Handling](#validation-and-error-handling)
12. [Security Features](#security-features)
13. [Browser Support](#browser-support)
14. [Support](#support)
15. [Disclaimer](#disclaimer)

## Overview

The Contract Monthly Claim System (CMCS) is a comprehensive web-based application developed using ASP.NET Core MVC. 
This system streamlines the process of submitting, reviewing, and approving monthly claims for Independent Contractor (IC) lecturers in educational institutions. 
The application features a clean, minimalist user interface, ensuring an intuitive user experience.

## Key Features

- **Lecturer Claim Submission**: Intuitive form interface for lecturers to submit monthly claims with hours worked and supporting documentation
- **Document Management**: Secure file upload system for supporting documents with type validation and size limits
- **Approval Workflow**: Streamlined review process for program coordinators and academic managers
- **Real-time Status Tracking**: Transparent tracking system that shows claim status throughout the approval lifecycle
- **Automated Calculations**: Automatic calculation of claim amounts based on hours worked and hourly rates
- **Role-based Access**: Different views and functionalities based on user roles (lecturers, coordinators, managers)
- **Responsive Design**: Mobile-friendly interface that works across various device sizes

## Technology Stack

- **Framework**: ASP.NET Core 7.0
- **Frontend**: HTML5, CSS3, JavaScript
- **Database**: Entity Framework Core with SQL Server
- **Styling**: Custom CSS with Apple-inspired minimalist design
- **Architecture**: MVC (Model-View-Controller) pattern

## System Architecture

The application follows a clean architecture pattern with separation of concerns:

- **Controllers**: Handle HTTP requests and application logic
- **Models**: Define data structures and business logic
- **Views**: Render the user interface using Razor templates
- **ViewModels**: Specialized models for view-specific data presentation

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/
│   ├── HomeController.cs
│   └── ClaimsController.cs
├── Documentation/
│   └── Documentation.txt
├── Models/
│   ├── ErrorViewModel.cs
│   ├── ClaimViewModels/
│   │   ├── ClaimSubmissionViewModel.cs
│   │   ├── ClaimApprovalViewModel.cs
│   │   └── DocumentViewModel.cs
│   └── DataModels/
│       ├── Lecturer.cs
│       ├── Claim.cs
│       ├── ClaimSystemContext.cs
│       ├── Document.cs
│       └── Approval.cs
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml
│   │   └── Privacy.cshtml
│   ├── Claims/
│   │   ├── Submit.cshtml
│   │   ├── Approve.cshtml
│   │   └── Status.cshtml
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
├── appsettings.json
└── Program.cs
```

## Database Design

The system uses a relational database with the following main entities:

- **Lecturers**: Stores information about independent contractor lecturers
- **Claims**: Tracks monthly claims submitted by lecturers
- **Documents**: Manages supporting documents uploaded with claims
- **Approvals**: Records approval decisions and comments

## Installation and Setup

### Prerequisites

- .NET 7.0 SDK or later
- SQL Server (LocalDB or full version)
- Web browser with JavaScript support
- Git for version control

### Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git
   cd contract-monthly-claim-system-cs
   ```

2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```

3. Update the database connection string in `appsettings.json`

4. Apply database migrations:
   ```bash
   dotnet ef database update
   ```

5. Build the solution:
   ```bash
   dotnet build
   ```

6. Run the application:
   ```bash
   dotnet run
   ```

### Configuration

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ContractClaimsDB;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

## Usage Guide

### For Lecturers

1. Navigate to the Submit Claim page
2. Enter hours worked (automatically calculates amount based on hourly rate)
3. Upload supporting documents (PDF, DOC, DOCX formats)
4. Submit the claim for review
5. Track claim status on the Status page

### For Coordinators and Managers

1. Access the Approve Claims page to view pending claims
2. Review claim details and supporting documents
3. Approve or reject claims with optional comments
4. Monitor the approval workflow

## Key Components

### Controllers

- **HomeController**: Handles basic navigation (Home, Privacy pages)
- **ClaimsController**: Manages all claim-related operations including submission, approval, and status tracking

### View Models

- **ClaimSubmissionViewModel**: Captures data for claim submission form
- **ClaimApprovalViewModel**: Provides data for claim approval interface
- **DocumentViewModel**: Handles document-related data presentation

### Data Models

- **Lecturer**: Represents independent contractor lecturers
- **Claim**: Stores monthly claim information
- **Document**: Manages uploaded supporting documents
- **Approval**: Tracks approval decisions and workflow

## Styling and Design

The application features a minimalist Apple-inspired design with:

- Clean typography using system fonts
- Subtle color palette with appropriate contrast
- Responsive layout that adapts to different screen sizes
- Intuitive navigation with tab-based interface
- Smooth transitions and hover effects

## Validation and Error Handling

- Client-side validation for immediate feedback
- Server-side validation for data integrity
- Comprehensive error handling with user-friendly messages
- File type and size validation for document uploads

## Security Features

- Input validation and sanitization
- Secure file upload handling
- Role-based access control
- Protection against common web vulnerabilities

## Browser Support

The application supports all modern browsers including:
- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Support

For technical support or questions about the Contract Monthly Claim System, please refer to the system documentation.

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
