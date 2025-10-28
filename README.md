# Contract Monthly Claim System

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

The Contract Monthly Claim System (CMCS) is a comprehensive ASP.NET C
ore web application designed to streamline the monthly claim submission 
and approval process for Independent Contractor lecturers. This system 
addresses complex workflow requirements involving hours worked 
calculations, hourly rates, and multi-level approval processes by 
Programme Coordinators and Academic Managers.

This implementation comprehensively addresses all three parts of the 
PROG6212 assignment requirements, with Part 3 focusing on advanced 
automation features including automated claim calculations, 
HR analytics, and workflow optimization.

## System Architecture

### High-Level Architecture
```
Presentation Layer (Views)
    ↓
Application Layer (Controllers)
    ↓
Business Logic Layer (Services)
    ↓
Data Access Layer (TextFileDataService)
    ↓
Storage Layer (Text Files)
```

### Security Architecture
- **Authentication**: Session-based custom implementation
- **Authorization**: Role-based access control (Lecturer, Programme Coordinator, Academic Manager)
- **Data Protection**: Input validation, anti-forgery tokens, secure session management

## Project Structure

```
contract-monthly-claim-system-cs/
├── Controllers/          # MVC Controllers
├── Models/              # Data and View Models
├── Views/               # Razor Views
├── Services/            # Business Logic Services
├── Data/                # Text File Storage
├── wwwroot/             # Static Files
├── Tests/               # Unit and Integration Tests
└── Documentation/       # Project Documentation
```

## Installation Guide

### Prerequisites
- .NET 8.0 SDK
- Git for version control
- Visual Studio 2022 or VS Code

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
   The application will be available at `http://localhost:5000`

### Default Login Credentials
- **Academic Manager**: admin / admin123
- **Lecturer**: lecturer / lecturer123  
- **Programme Coordinator**: coordinator / coordinator123

## Usage Manual

### For Lecturers
1. **Login** using your credentials
2. **Submit Claims** via the Submit Claim page
3. **Track Status** of submitted claims
4. **Upload Supporting Documents** with each claim

### For Programme Coordinators
1. **Review Pending Claims** in the Approve section
2. **Approve or Reject** claims with comments
3. **Monitor Claim Workflow**

### For Academic Managers
1. **Access HR Analytics Dashboard** for comprehensive reporting
2. **Final Approval** of high-value claims
3. **System Administration** and monitoring

### Key Features in Use
- **Automated Calculations**: Claim amounts are automatically calculated based on hours and rates
- **Multi-level Approval**: Claims requiring manager approval are automatically flagged
- **Document Management**: Secure upload and storage of supporting documents
- **Real-time Tracking**: Transparent status tracking for all users

## Features

### Core Functionality
- User Authentication and Role-based Access
- Monthly Claim Submission with Validation
- Multi-level Approval Workflow
- Document Upload and Management
- Claim Status Tracking

### Part 3 Automation Features
- **Automated Claim Calculations**: Automatic amount calculation with overtime consideration
- **HR Analytics Dashboard**: Comprehensive reporting for Academic Managers
- **Automated Verification**: Predefined criteria checking for claims
- **Performance Analytics**: Lecturer performance tracking and trend analysis
- **Workflow Automation**: Streamlined approval processes with automated notifications

### Security Features
- Session-based Authentication
- Role-based Authorization
- Input Validation and Sanitization
- Anti-Forgery Token Protection
- Secure File Upload Handling

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 MVC
- **Language**: C# 12.0
- **Storage**: Text File-based (JSON serialization)
- **Session Management**: Custom session extensions

### Frontend
- **UI Framework**: Razor Pages
- **Styling**: Custom CSS with Apple-inspired design principles
- **JavaScript**: Vanilla JS for client-side interactions
- **Responsive Design**: Mobile-first approach

### Testing
- **Framework**: xUnit 2.5.3
- **Mocking**: Moq 4.20.70
- **Coverage**: Comprehensive unit and integration tests

### Development Tools
- **IDE**: Visual Studio 2022 / VS Code
- **Version Control**: Git
- **Build System**: .NET CLI

## Development Notes

### Key Design Patterns
- MVC Architecture
- Repository Pattern (Text File implementation)
- Service Layer Pattern
- Dependency Injection

### Automation Implementation
The system implements comprehensive automation as required by Part 3 POE:

1. **Auto-calculation Engine**: 
   - Calculates claim amounts automatically
   - Applies overtime rates for hours over 160
   - Validates business rules in real-time

2. **Workflow Automation**:
   - Automated status updates
   - Multi-level approval routing
   - Notification system for pending actions

3. **Analytics Automation**:
   - Real-time performance metrics
   - Trend analysis and reporting
   - Automated data export capabilities

### Data Storage Approach
- Uses text files instead of database for prototype phase
- JSON serialization for data persistence
- Automated backup and recovery mechanisms
- Optimized file operations with error handling

## Documentation

### Technical Documentation
Comprehensive documentation is available in the `Documentation/` folder:
- `Project_Plan.md`: Project planning and requirements
- `Documentation.md`: Technical specifications
- `Presentation.pdf`: System overview presentation

### API Documentation
The system provides RESTful endpoints for:
- User authentication and management
- Claim submission and processing
- Approval workflow management
- Analytics and reporting

### Testing Documentation
- Unit tests for all controllers and services
- Integration tests for complete workflows
- Model validation tests
- Session management tests

## Notes

### Academic Compliance
This project fulfills all PROG6212 assignment requirements:
- **Part 1**: Project planning, UML diagrams, and prototype design
- **Part 2**: Functional prototype implementation with core features
- **Part 3**: Advanced automation features and system enhancements

### Implementation Highlights
- No database dependency - uses text file storage
- Comprehensive error handling and validation
- Professional-grade user interface design
- Extensive test coverage
- Clean code architecture and separation of concerns

### Visual Media
All visual media, including screenshots and images of the application, 
are stored in a dedicated folder within the project directory. 
This folder is clearly structured and named accordingly to indicate 
that it contains all visual content related to the application.

## Disclaimer

### Software Usage
UNDER NO CIRCUMSTANCES SHOULD IMAGES OR EMOJIS BE INCLUDED DIRECTLY IN 
THE README FILE. ALL VISUAL MEDIA, INCLUDING SCREENSHOTS AND IMAGES OF 
THE APPLICATION, MUST BE STORED IN A DEDICATED FOLDER WITHIN THE PROJECT 
DIRECTORY. THIS FOLDER SHOULD BE CLEARLY STRUCTURED AND NAMED ACCORDINGLY 
TO INDICATE THAT IT CONTAINS ALL VISUAL CONTENT RELATED TO THE APPLICATION 
(FOR EXAMPLE, A FOLDER NAMED IMAGES, SCREENSHOTS, OR MEDIA).

### Liability Notice
I AM NOT LIABLE OR RESPONSIBLE FOR ANY MALFUNCTIONS, DEFECTS, OR ISSUES 
THAT MAY OCCUR AS A RESULT OF COPYING, MODIFYING, OR USING THIS SOFTWARE. 
IF YOU ENCOUNTER ANY PROBLEMS OR ERRORS, PLEASE DO NOT ATTEMPT TO FIX 
THEM SILENTLY OR OUTSIDE THE PROJECT. INSTEAD, KINDLY SUBMIT A PULL REQUEST 
OR OPEN AN ISSUE ON THE CORRESPONDING GITHUB REPOSITORY, SO THAT IT CAN 
BE ADDRESSED APPROPRIATELY BY THE MAINTAINERS OR CONTRIBUTORS.

### Repository
https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git

This software is provided as-is for academic and demonstration purposes. 
Users are encouraged to review the code, understand the implementation, 
and adapt it to their specific requirements while maintaining proper 
attribution and adherence to academic integrity guidelines.

---