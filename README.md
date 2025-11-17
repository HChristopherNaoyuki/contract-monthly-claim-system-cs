# Contract Monthly Claim System - Project Documentation

**GITHUB LINK:** https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git

## Table of Contents
1. [Project Overview](#1-project-overview)
2. [System Architecture](#2-system-architecture)
3. [UML Class Diagram](#3-uml-class-diagram)
4. [Project Timeline](#4-project-timeline)
   - [Part 1: Core System & Prototype (4 Weeks)](#part-1-core-system--prototype-4-weeks)
   - [Part 2: Feedback Implementation & Enhancement (3 Weeks)](#part-2-feedback-implementation--enhancement-3-weeks)
   - [Part 3: Final Development, Deployment & Polish (3 Weeks)](#part-3-final-development-deployment--polish-3-weeks)
5. [GUI Design Philosophy](#5-gui-design-philosophy)
6. [Core Functionality](#6-core-functionality)
7. [Technical Implementation Details](#7-technical-implementation-details)
   - [Authentication System](#authentication-system)
   - [Data Storage](#data-storage)
   - [File Upload System](#file-upload-system)
   - [Validation & Error Handling](#validation--error-handling)
8. [Feedback Implementation](#8-feedback-implementation)
9. [Features Summary](#9-features-summary)
10. [Installation & Setup](#10-installation--setup)
11. [Future Enhancements](#11-future-enhancements)

## 1. Project Overview
The Contract Monthly Claim System (CMCS) is a comprehensive web-based application 
designed to streamline the monthly claim submission and approval process for 
independent contractor lecturers. This initial 4-week phase focused on building a 
functional prototype with core features, establishing a user-centric design, and 
creating a scalable architecture to serve three distinct user roles: Lecturers, 
Program Coordinators, and Academic Managers.

## 2. System Architecture
The system is built on the **ASP.NET Core MVC** framework using **C#**, adhering 
to the Model-View-Controller pattern for a clear separation of concerns. The initial 
prototype utilized **session-based authentication** and **in-memory data storage**. 
The frontend was developed with modern CSS features like Grid and Flexbox to ensure 
a fully responsive design across all devices.

## 3. UML Class Diagram

```mermaid
classDiagram
    direction TB
    
    note for User "Base class for authentication"
    note for Lecturer "Extends User with contract details"
    note for Claim "Core claim entity"
    note for Document "Manages file uploads"
    note for Approval "Tracks review process"

    class User {
        +int UserId
        +string Name
        +string Surname
        +string Username
        +string Password
        +UserRole Role
    }

    class Lecturer {
        +int LecturerId
        +string FirstName
        +string LastName
        +string Email
        +double HourlyRate
        +SubmitClaim()
    }

    class Claim {
        +int ClaimId
        +int LecturerId
        +DateTime ClaimDate
        +decimal HoursWorked
        +double Amount
        +ClaimStatus Status
        +CalculateAmount()
        +UpdateStatus()
    }

    class Document {
        +int DocumentId
        +int ClaimId
        +string FileName
        +string FilePath
        +DateTime UploadDate
        +ValidateFile()
    }

    class Approval {
        +int ApprovalId
        +int ClaimId
        +string ApproverRole
        +DateTime ApprovalDate
        +bool IsApproved
        +string Comments
        +ProcessApproval()
    }

    class ClaimSubmissionViewModel {
        +decimal HoursWorked
        +decimal HourlyRate
        +decimal Amount
        +string Comments
        +List~IFormFile~ Documents
        +Validate()
    }

    class ClaimApprovalViewModel {
        +int ClaimId
        +string LecturerName
        +DateTime ClaimDate
        +decimal HoursWorked
        +decimal HourlyRate
        +decimal Amount
        +string Status
        +List~string~ DocumentNames
        +string SubmissionComments
        +string ApprovalComments
    }

    %% Define enumerations as classes
    class UserRole {
        <<enumeration>>
        Lecturer
        ProgrammeCoordinator
        AcademicManager
    }

    class ClaimStatus {
        <<enumeration>>
        Pending
        Approved
        Rejected
    }

    %% Relationships
    User <|-- Lecturer
    Lecturer ||--o{ Claim : submits
    Claim ||--o{ Document : contains
    Claim ||--o{ Approval : undergoes
    
    %% ViewModel relationships
    ClaimSubmissionViewModel ..> Claim : maps to
    ClaimApprovalViewModel ..> Claim : maps to
    ClaimApprovalViewModel ..> Lecturer : maps to
```

## 4. Project Timeline

### Part 1: Core System & Prototype (4 Weeks)
- **Week 1:** Requirements analysis and foundational UML class diagram design.
- **Week 2:** Core ASP.NET MVC framework setup and basic session authentication.
- **Week 3:** Development of the Claim Submission module with dynamic form validation and calculation logic.
- **Week 4:** Implementation of the Review and Approval interface, followed by initial testing and prototype documentation.

### Part 2: Feedback Implementation & Enhancement (3 Weeks)
- **Week 5:** Foundation & Text File Database implementation.
- **Week 6:** Enhanced Authentication, Document Upload, and UI/UX improvements.
- **Week 7:** Comprehensive Validation, Error Handling, Testing, and Final Polish.

### Part 3: Final Development, Deployment & Polish (3 Weeks)
- **Week 8:** Enhanced Features & Data Management
- **Week 9:** Security & Performance Optimization
- **Week 10:** Final Testing, Polish & Documentation

## 5. GUI Design Philosophy
The interface was designed with a minimalist philosophy, emphasizing:
- Clean, uncluttered layouts with strategic use of white space.
- Consistent typography and a clear visual hierarchy.
- Intuitive navigation with distinct interactive elements.
- A responsive design that works seamlessly on desktop and mobile.

## 6. Core Functionality
- Role-based authentication and session management.
- Dynamic claim submission form with real-time amount calculation.
- File upload system with type and size validation.
- Dual-comment system for submissions and approvals.
- Status tracking with visual indicators for different claim states.
- Responsive data tables for efficient information review.

## 7. Technical Implementation Details

### Authentication System
- Session-based authentication with custom session extensions
- Role-based access control (Lecturer, Coordinator, Manager)
- 30-minute idle timeout configuration
- Secure password storage and anti-forgery protection

### Data Storage
- JSON-based text file storage system
- Automatic file creation and initialization
- Sample data population on first run
- Files: users.txt, claims.txt, documents.txt, approvals.txt

### File Upload System
- Physical files stored in wwwroot/uploads directory
- GUID-based file naming to prevent conflicts
- Support for PDF, DOC, DOCX, JPG, PNG formats
- 5MB file size limit per file
- Client-side and server-side validation

### Validation & Error Handling
- Model validation with data annotations
- Client-side validation using jQuery
- Global exception handling middleware
- Comprehensive error messages and user feedback

## 8. Feedback Implementation
The system successfully addressed all Part 1 feedback:
- UML Diagram: Updated to reflect actual implementation
- Database: Replaced with text file storage system
- Document Upload: Made fully functional with proper validation
- UI/UX: Enhanced with Apple-like minimalist design
- Error Handling: Implemented comprehensive validation
- Session Management: Improved user state persistence

## 9. Features Summary
- **Core Functionality**
  - Text file database system
  - Document upload with file storage
  - Enhanced authentication
  - Role-based access control
  - Comprehensive validation

- **User Experience**
  - Minimalist design
  - Responsive interface
  - Real-time form calculations
  - Interactive file upload
  - Improved error messages

- **Technical Improvements**
  - Custom session management
  - File validation and handling
  - JSON-based data storage
  - Comprehensive testing
  - Robust error handling

## 10. Installation & Setup
1. Clone the repository from GitHub
2. Open solution in Visual Studio 2022+
3. Build the solution to restore NuGet packages
4. Run the application - text files will be auto-generated
5. Use default credentials from auto-generated users.txt

## 11. Future Enhancements
- Advanced reporting dashboard with charts
- Email notification system
- BCrypt password hashing implementation
- Docker containerization for deployment
- Advanced filtering and export capabilities

---
