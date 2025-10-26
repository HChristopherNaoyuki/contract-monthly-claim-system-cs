# Contract Monthly Claim System - Technical Documentation

## Table of Contents
1. [Project Overview](#project-overview)
2. [System Architecture](#system-architecture)
3. [Database Design](#database-design)
4. [Implementation Details](#implementation-details)
5. [API Documentation](#api-documentation)
6. [Testing Strategy](#testing-strategy)
7. [Deployment Guide](#deployment-guide)
8. [Maintenance Procedures](#maintenance-procedures)

## Project Overview

### Business Context
The Contract Monthly Claim System (CMCS) is designed to streamline the monthly 
claim submission and approval process for Independent Contractor (IC) lecturers. 
The system addresses the complex workflow involving hours worked calculations, 
hourly rates, and multi-level approval processes by Programme Coordinators and 
Academic Managers.

### Technical Scope
- **Platform**: ASP.NET Core 8.0 MVC Web Application
- **Storage**: Text File-based data persistence (JSON format)
- **Authentication**: Session-based custom implementation
- **Testing**: xUnit 2.5.3 with Moq framework
- **UI**: Razor Pages with custom Apple-inspired CSS

### POE Requirements Fulfillment
This implementation comprehensively addresses all three parts of the PROG6212 assignment:

**Part 1**: Project Planning and Prototype Development
- Complete UML class diagrams
- Comprehensive project documentation
- Non-functional GUI prototype

**Part 2**: Prototype Implementation
- Functional claim submission system
- Role-based access control
- Document management system

**Part 3**: POE Automation Enhancement
- Advanced automation features
- Comprehensive analytics and reporting
- Workflow optimization

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

### Component Architecture

#### 1. Presentation Layer
- **Technology**: ASP.NET Core Razor Pages
- **Styling**: Custom CSS with Apple design principles
- **Client-Side**: Vanilla JavaScript for enhanced interactivity
- **Responsive Design**: Mobile-first approach

#### 2. Application Layer
- **Controllers**: 
  - `AuthController`: Authentication and user management
  - `ClaimsController`: Core claims processing with automation
  - `HomeController`: Public pages and navigation
  - `ServerController`: System diagnostics
  - `TestController`: System testing utilities

#### 3. Business Logic Layer
- **TextFileDataService**: Comprehensive data operations
- **Validation Services**: Business rule enforcement
- **Automation Engine**: Workflow automation
- **Notification System**: Status updates and alerts

#### 4. Data Access Layer
- **Storage Mechanism**: JSON-serialized text files
- **Data Integrity**: Automated backup and recovery
- **Performance**: Optimized file operations with buffering

### Security Architecture

#### Authentication Flow
```
User Login → Session Creation → Role Validation → Access Control
```

#### Authorization Matrix
| Role | Claim Submission | Claim Approval | HR Analytics | System Admin |
|------|------------------|----------------|--------------|--------------|
| Lecturer | ✓ | ✗ | ✗ | ✗ |
| Programme Coordinator | ✗ | ✓ | ✗ | ✗ |
| Academic Manager | ✗ | ✓ | ✓ | ✓ |

## Database Design

### Text File Storage Structure

#### File Organization
```
Data/
├── users.txt          # User accounts and credentials
├── lecturers.txt      # Lecturer-specific information
├── claims.txt         # Claim submissions and status
├── documents.txt      # Supporting document metadata
├── approvals.txt      # Approval workflow records
└── analytics.txt      # System usage statistics
```

### Data Models

#### User Model
```csharp
public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginDate { get; set; }
}
```

#### Claim Model
```csharp
public class Claim
{
    public int ClaimId { get; set; }
    public int LecturerId { get; set; }
    public DateTime ClaimDate { get; set; } = DateTime.Now;
    public string MonthYear { get; set; } = string.Empty;
    public decimal HoursWorked { get; set; }
    public decimal HourlyRate { get; set; }
    public decimal Amount { get; set; }
    public ClaimStatus Status { get; set; } = ClaimStatus.Submitted;
    public string SubmissionComments { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
}
```

#### Data Relationships
```
User (1) ←→ (1) Lecturer
Lecturer (1) ←→ (Many) Claims
Claim (1) ←→ (Many) Documents
Claim (1) ←→ (Many) Approvals
```

### Data Integrity Measures

#### Validation Rules
1. **User Validation**: Unique usernames, password strength
2. **Claim Validation**: Hours worked (0-744), hourly rate limits
3. **Document Validation**: File size (5MB max), allowed types
4. **Business Rules**: Monthly claim limits, approval workflows

#### Backup Strategy
- Automated backup creation before write operations
- Recovery mechanisms for corrupted files
- Data consistency checks on system startup

## Implementation Details

### Core Controllers

#### ClaimsController
**Responsibilities**:
- Claim submission with automated calculation
- Multi-level approval workflow management
- HR analytics and reporting
- Document processing and management

**Key Automation Features**:
```csharp
// Auto-calculation with overtime
private decimal AutoCalculateClaimAmount(decimal hoursWorked, decimal hourlyRate)
{
    var amount = hoursWorked * hourlyRate;
    
    // Overtime calculation (time and a half after 160 hours)
    if (hoursWorked > 160)
    {
        var overtimeHours = hoursWorked - 160;
        var overtimeRate = hourlyRate * 1.5m;
        amount = (160 * hourlyRate) + (overtimeHours * overtimeRate);
    }
    
    return Math.Round(amount, 2);
}

// Automated validation
private (bool IsValid, string ErrorMessage) ValidateClaimSubmission(ClaimSubmissionViewModel model, int userId)
{
    // Business rule validation
    if (model.HoursWorked > 744) return (false, "Hours worked cannot exceed 744 hours per month.");
    if (model.HourlyRate > 500) return (false, "Hourly rate exceeds maximum allowed amount.");
    
    // Monthly submission limit
    var currentMonthClaims = _dataService.GetAllClaims()
        .Where(c => c.LecturerId == userId && 
                   c.MonthYear == DateTime.Now.ToString("yyyy-MM"))
        .Count();
        
    if (currentMonthClaims >= 3) return (false, "Maximum of 3 claims allowed per month.");
    
    return (true, string.Empty);
}
```

#### TextFileDataService
**Data Operations**:
```csharp
public class TextFileDataService
{
    // Enhanced read operation with error recovery
    private List<T> ReadData<T>(string dataType)
    {
        var filePath = GetFilePath(dataType);
        var maxRetries = 3;
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                    }
                }
                break;
            }
            catch (JsonException jsonEx)
            {
                // Automated recovery for corrupted files
                if (attempt == maxRetries)
                {
                    CreateDataBackup(filePath);
                    WriteData(dataType, new List<T>());
                    return new List<T>();
                }
            }
        }
        return new List<T>();
    }
    
    // Atomic write operation
    private void WriteData<T>(string dataType, List<T> data)
    {
        var filePath = GetFilePath(dataType);
        var tempFilePath = filePath + ".tmp";
        
        // Create backup
        CreateDataBackup(filePath);
        
        // Write to temporary file first
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        
        File.WriteAllText(tempFilePath, json);
        File.Move(tempFilePath, filePath, true);
        
        // Update analytics
        UpdateAnalytics(dataType, data.Count);
    }
}
```

### Automation Features

#### Part 3 POE Automation Implementation

**1. Automated Claim Analysis**
```csharp
public class ClaimApprovalViewModel
{
    // Automated verification flags
    public bool HasExcessiveHours { get; set; }    // > 160 hours
    public bool HasUnusualAmount { get; set; }     // > R10,000
    public bool RequiresManagerApproval { get; set; } // > R5,000 for coordinators
    
    // Intelligent prioritization
    public string Priority 
    {
        get
        {
            if (Amount > 10000 || DaysPending > 14) return "High";
            if (Amount > 5000 || DaysPending > 7) return "Medium";
            return "Low";
        }
    }
}
```

**2. HR Analytics Automation**
```csharp
public class HRDashboardViewModel
{
    // Automated performance metrics
    public decimal ApprovalRate 
    {
        get
        {
            if (TotalClaims == 0) return 0;
            return Math.Round((decimal)ApprovedClaims / TotalClaims * 100, 2);
        }
    }
    
    // Automated trend analysis
    public List<MonthlyBreakdownViewModel> MonthlyBreakdown { get; set; }
    public List<TopLecturerViewModel> TopLecturers { get; set; }
}
```

**3. Workflow Automation**
```csharp
private async Task ProcessUploadedDocuments(List<IFormFile> documents, int claimId)
{
    foreach (var file in documents)
    {
        // Automated validation
        if (file.Length > 5 * 1024 * 1024) continue; // 5MB limit
        
        var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension)) continue;
        
        // Secure file processing
        var fileName = $"{claimId}_{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(uploadsDirectory, fileName);
        
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }
        
        // Automated metadata creation
        var document = new Document
        {
            DocumentId = _dataService.GetNextId("documents"),
            ClaimId = claimId,
            FileName = file.FileName,
            FilePath = $"/uploads/{fileName}",
            FileSize = file.Length,
            FileType = fileExtension,
            UploadDate = DateTime.Now,
            IsActive = true
        };
        
        _dataService.SaveDocument(document);
    }
}
```

### Security Implementation

#### Session Management
```csharp
public static class SessionExtensions
{
    public static void SetSessionInt(this ISession session, string key, int value)
    {
        session.Set(key, BitConverter.GetBytes(value));
    }
    
    public static int? GetSessionInt(this ISession session, string key)
    {
        var data = session.Get(key);
        if (data == null || data.Length == 0) return null;
        return BitConverter.ToInt32(data, 0);
    }
}
```

#### Input Validation
```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Submit(ClaimSubmissionViewModel model)
{
    if (ModelState.IsValid)
    {
        // Additional server-side validation
        var validationResult = ValidateClaimSubmission(model, userId);
        if (!validationResult.IsValid)
        {
            ModelState.AddModelError("", validationResult.ErrorMessage);
            return View(model);
        }
        // Process claim...
    }
    return View(model);
}
```

## API Documentation

### Controller Endpoints

#### Authentication Endpoints
| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | /Auth | Login/Registration page | - |
| POST | /Auth/Login | User authentication | LoginViewModel |
| POST | /Auth/Register | New user registration | RegisterViewModel |
| POST | /Auth/Logout | Session termination | - |

#### Claims Management Endpoints
| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | /Claims/Submit | Claim submission form | - |
| POST | /Claims/Submit | Submit new claim | ClaimSubmissionViewModel |
| GET | /Claims/Approve | Approval dashboard | - |
| POST | /Claims/ApproveClaim | Process approval | claimId, isApproved, comments |
| GET | /Claims/Status | Claim status view | claimId |
| GET | /Claims/Track | Claim tracking | - |
| GET | /Claims/HRDashboard | HR analytics | - |

#### System Endpoints
| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| GET | /Server/Status | System diagnostics | - |
| GET | /Test/Connection | Database connection test | - |

### View Models

#### ClaimSubmissionViewModel
```csharp
public class ClaimSubmissionViewModel
{
    [Required(ErrorMessage = "Hours worked is required")]
    [Range(0, 744, ErrorMessage = "Hours must be between 0 and 744")]
    public decimal HoursWorked { get; set; }

    [Required(ErrorMessage = "Hourly rate is required")]
    [Range(0, 999.99, ErrorMessage = "Hourly rate must be between 0 and 999.99")]
    public decimal HourlyRate { get; set; }

    public decimal Amount { get; set; } // Auto-calculated

    [StringLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
    public string Comments { get; set; } = string.Empty;

    public List<IFormFile> Documents { get; set; } = new List<IFormFile>();
}
```

## Testing Strategy

### Unit Testing Architecture

#### Test Project Structure
```
Tests/
├── Controllers/
│   ├── AuthControllerTests.cs
│   ├── ClaimsControllerTests.cs
│   └── HomeControllerTests.cs
├── Extensions/
│   └── SessionExtensionsTests.cs
├── Helpers/
│   ├── MockLogger.cs
│   └── TestSession.cs
├── Integration/
│   └── IntegrationTest.cs
└── Models/
    ├── ClaimViewModelTests.cs
    ├── DataModelTests.cs
    └── ViewModelTests.cs
```

#### Comprehensive Test Coverage

**Controller Tests**:
```csharp
public class ClaimsControllerTests
{
    [Fact]
    public async Task Submit_WithAutomatedCalculation_CalculatesAmountCorrectly()
    {
        // Arrange
        var model = new ClaimSubmissionViewModel
        {
            HoursWorked = 40,
            HourlyRate = 150.00m
        };

        // Act
        var result = await _controller.Submit(model);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        _dataServiceMock.Verify(d => d.SaveClaim(It.Is<Claim>(c => c.Amount == 6000.00m)), Times.Once);
    }

    [Fact]
    public void Approve_WithHighAmountClaim_SetsRequiresManagerApproval()
    {
        // Arrange
        _sessionMock.Setup(s => s.GetString("Role")).Returns("ProgrammeCoordinator");
        var highAmountClaim = new Claim { Amount = 10000.00m }; // High amount

        // Act
        var result = _controller.Approve();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<List<ClaimApprovalViewModel>>(viewResult.Model);
        var highAmountViewModel = model.First(m => m.Amount == 10000.00m);
        Assert.True(highAmountViewModel.RequiresManagerApproval);
    }
}
```

**Model Validation Tests**:
```csharp
public class DataModelTests
{
    [Fact]
    public void User_ValidModel_PassesValidation()
    {
        var user = new User
        {
            Name = "Test",
            Surname = "User",
            Username = "testuser",
            Password = "password123",
            Role = UserRole.Lecturer,
            Email = "test@example.com"
        };

        var validationResults = ValidateModel(user);
        Assert.Empty(validationResults);
    }

    [Fact]
    public void Claim_ExcessiveHours_FailsValidation()
    {
        var claim = new Claim
        {
            LecturerId = 1,
            MonthYear = "2024-01",
            HoursWorked = 800, // Exceeds maximum
            HourlyRate = 150.00m,
            Amount = 120000.00m,
            Status = ClaimStatus.Submitted
        };

        var validationResults = ValidateModel(claim);
        Assert.Single(validationResults);
        Assert.Contains(validationResults, v => v.MemberNames.Contains("HoursWorked"));
    }
}
```

### Integration Testing
```csharp
public class IntegrationTests
{
    [Fact]
    public void CompleteWorkflow_LoginSubmitClaim_WorksCorrectly()
    {
        var authController = CreateAuthController();
        var claimsController = CreateClaimsController();

        // Login
        var loginResult = authController.Login(new LoginViewModel
        {
            Username = "lecturer",
            Password = "lecturer123"
        });

        Assert.IsType<RedirectToActionResult>(loginResult);

        // Submit claim
        var claimModel = new ClaimSubmissionViewModel
        {
            HoursWorked = 40,
            HourlyRate = 150.00m,
            Comments = "Integration test claim"
        };

        var submitResult = claimsController.Submit(claimModel);
        var submitRedirect = Assert.IsType<RedirectToActionResult>(submitResult);
        Assert.Equal("Status", submitRedirect.ActionName);
    }
}
```

## Deployment Guide

### Development Environment Setup

1. **Prerequisites Installation**
   ```bash
   # Install .NET 8.0 SDK
   # Install Git for version control
   # Install preferred IDE (Visual Studio 2022 or VS Code)
   ```

2. **Project Setup**
   ```bash
   git clone https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git
   cd contract-monthly-claim-system-cs
   dotnet restore
   dotnet build
   ```

3. **Environment Configuration**
   - Ensure `Data` directory has write permissions
   - Create `wwwroot/uploads` directory for file storage
   - Configure app settings in `appsettings.json`

4. **Run Application**
   ```bash
   dotnet run
   # Access at http://localhost:5000
   ```

### Production Considerations

**Security Enhancements**:
- Implement HTTPS with proper certificates
- Add additional input sanitization
- Implement rate limiting
- Add audit logging

**Performance Optimizations**:
- Implement caching mechanisms
- Optimize file I/O operations
- Add database connection pooling (if migrating to database)

**Monitoring**:
- Implement health check endpoints
- Add application performance monitoring
- Set up log aggregation

## Maintenance Procedures

### Routine Maintenance

1. **Data Backup**
   - Automated backups are created during write operations
   - Manual backup: Copy entire `Data` directory
   - Recovery: Replace corrupted files with backup versions

2. **Log Management**
   - Review application logs in `logs` directory
   - Monitor for error patterns and system issues
   - Implement log rotation for production

3. **File System Maintenance**
   - Monitor `wwwroot/uploads` directory size
   - Implement archival procedures for old documents
   - Regular cleanup of temporary files

### Troubleshooting Guide

#### Common Issues and Solutions

**1. Port Conflicts**
```bash
# Check port usage
netstat -ano | findstr :5000
# Kill process or use different port in launchSettings.json
```

**2. File Permission Issues**
- Ensure `Data` and `wwwroot/uploads` directories have write permissions
- Run application with appropriate user privileges

**3. Data Corruption**
- Automated recovery mechanisms attempt to repair corrupted files
- Manual recovery: Restore from backup files (`*.backup`)

**4. Session Issues**
- Clear browser cookies and cache
- Restart application to reset session state

### Performance Monitoring

**Key Metrics to Monitor**:
- Claim processing time
- File upload success rates
- System resource usage (CPU, memory, disk I/O)
- User session statistics

**Automated Health Checks**:
- Database connectivity tests
- File system accessibility
- Application responsiveness

This comprehensive documentation provides complete technical specifications 
for the Contract Monthly Claim System, ensuring maintainability, scalability, 
and adherence to academic requirements while demonstrating professional 
software development practices.

---