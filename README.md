# Contract Monthly Claim System

## Table of Contents

1. [Project Overview](#1-project-overview)
2. [System Architecture](#2-system-architecture)
3. [Core Domain Models](#3-core-domain-models)
4. [Component Architecture](#4-component-architecture)
5. [Development Phases](#5-development-phases)
6. [Feature Implementation](#6-feature-implementation)
7. [Technical Specifications](#7-technical-specifications)
8. [Design System](#8-design-system)
9. [Data Persistence Strategy](#9-data-persistence-strategy)
10. [Security Framework](#10-security-framework)
11. [Quality Assurance](#11-quality-assurance)
12. [Deployment Guide](#12-deployment-guide)
13. [Repository Structure](#13-repository-structure)

---

## 1. Project Overview

The Contract Monthly Claim System is a comprehensive academic administration
platform designed specifically for managing lecturer claims and approvals.
This system streamlines the process of submitting, reviewing, and approving
monthly teaching claims within a university or educational institution.

**Repository Location:**
https://github.com/HChristopherNaoyuki/contract-monthly-claim-system-cs.git

**Core Purpose:**
The system replaces manual paper-based claim processes with a digital
workflow that ensures transparency, accountability, and efficiency. Every
claim moves through a structured approval chain while maintaining complete
audit trails and document attachments.

**Key Stakeholders:**
- Lecturers who submit monthly claims for teaching hours
- Programme Coordinators who perform initial claim reviews
- Academic Managers who provide final approval authority
- Human Resources personnel who process approved claims for payment

**Success Metrics:**
- Reduction in claim processing time from weeks to days
- Complete audit trail for every financial transaction
- Elimination of lost or misplaced paper claims
- Real-time status visibility for all parties involved

**Technology Stack:**
- ASP.NET Core MVC 8.0 as the primary web framework
- Text file storage with JSON serialization for data persistence
- Session-based authentication with role-based access control
- Razor Pages for dynamic server-side rendering
- Bootstrap 5 for responsive front-end design
- JavaScript for enhanced client-side interactions

**Design Philosophy:**
The system follows Apple-inspired design principles focusing on clarity,
progressive disclosure, and intuitive user experiences. Every screen
presents only necessary information, with advanced options revealed
only when the user needs them.

---

## 2. System Architecture

The architecture follows a clean layered approach that separates concerns
while maintaining high cohesion within each layer. This design ensures
that changes to one layer do not cascade through the entire system.

**Layer One: Presentation Layer**

The presentation layer contains all user-facing components including
Razor Views, Controllers, ViewModels, and client-side validation logic.
Controllers handle HTTP requests and responses while Views render the
HTML interface. ViewModels act as data transfer objects between the
presentation and business layers.

All Controllers inherit from a base Controller class that provides
common functionality such as session management, user authentication,
and error handling. The layout system ensures consistent navigation,
headers, footers, and notification areas across all pages.

**Layer Two: Business Logic Layer**

The business layer contains service classes that implement all core
functionality including claim validation, amount calculations, approval
workflow orchestration, and notification management. This layer contains
no direct dependencies on storage mechanisms or presentation concerns.

Service classes follow the single responsibility principle with focused
classes for claims, documents, users, approvals, and notifications.
The ClaimService orchestrates the complete claim lifecycle from initial
submission through final approval or rejection.

**Layer Three: Data Access Layer**

The data access layer implements the repository pattern to abstract
storage operations away from business logic. Generic repositories
provide common CRUD operations while specialized repositories add
domain-specific query methods. JSON serialization converts objects
to and from text file storage.

**Layer Four: Storage Layer**

The storage layer consists of JSON text files stored in the App_Data
directory. Each entity type has its own file: users.json stores all
user accounts and profiles, claims.json contains all claim submissions,
documents.json tracks file metadata, and approvals.json records the
complete approval history.

**Communication Flow:**
User requests flow from the browser to Controllers, which invoke
Service methods. Services interact with Repositories to access data,
then return results to Controllers. Controllers package data into
ViewModels and return appropriate Views with the rendered HTML.

**Cross-Cutting Concerns:**
Logging, exception handling, authentication, and authorization are
implemented as middleware components that process every request.
Custom filters provide role-based access control at controller and
action method levels.

---

## 3. Core Domain Models

The domain model centers around the Claim entity which represents a
lecturer's monthly teaching hours submission. Supporting entities
include User, Document, Approval, Notification, and AuditLog.

**User Hierarchy and Roles:**

The User class serves as an abstract base for all system users. It
contains common properties including UserId, Name, Surname, Username,
PasswordHash, Role, Email, IsActive status, and CreatedDate timestamp.
A computed FullName property concatenates name and surname.

The Lecturer class inherits from User and adds lecturer-specific
properties: EmployeeNumber, Department, HourlyRate, ContractStartDate,
ContractEndDate, BankAccountNumber, BankName, and TaxNumber. Lecturers
maintain a collection of their submitted Claim objects.

The Administrator abstract class inherits from User and adds AdminId
and Department properties. It maintains a collection of Approvals made.
ProgrammeCoordinator and AcademicManager classes both inherit from
Administrator, with ProgrammeCoordinator adding a Programmes collection
and AcademicManager adding a Faculty property.

**Claim Entity Structure:**

The Claim entity represents a single monthly claim submission. Key
properties include ClaimId as unique identifier, LecturerId foreign key,
ClaimDate timestamp of submission, MonthYear string in YYYY-MM format,
HoursWorked decimal value, HourlyRate at time of submission, Amount
calculated as hours multiplied by rate, Status enum for workflow state,
SubmissionComments for lecturer notes, and timestamps for tracking.

ClaimStatus enumeration includes Draft for unsaved work, Submitted for
pending review, UnderReview when actively examined, Approved for success,
Rejected with required justification, Paid after financial processing,
and Archived for historical records.

**Document Entity:**

Documents attached to claims require validation for file type and size.
Properties include DocumentId, ClaimId foreign key, FileName original
name, FilePath storage location, FileSize in bytes, FileType extension,
UploadDate timestamp, UploadedByUserId, and IsActive soft delete flag.

Allowed file types include PDF, DOC, DOCX, JPG, JPEG, and PNG formats.
Maximum file size is limited to five megabytes per document. Multiple
documents can attach to a single claim.

**Approval Workflow Entity:**

The Approval entity tracks each step in the multi-stage approval process.
Properties include ApprovalId, ClaimId, ApproverUserId, ApproverRole,
ApprovalDate when decision made, IsApproved boolean outcome, Comments
from approver, ApprovalOrder sequence number, and CanOverride flag for
managers to reverse prior decisions.

**Notification and Audit Entities:**

Notification objects store messages for users including Message text,
Type enum (Success, Error, Info, Warning), IsRead status, and CreatedDate.
AuditLog entries capture all critical actions with UserId, Action name,
EntityType and EntityId affected, Details text, Timestamp, and IPAddress.

---

## 4. Component Architecture

The component architecture defines how different parts of the system
interact through well-defined interfaces and dependency injection.

**TextFileRepository Generic Component:**

This generic repository class implements IRepository<T> where T must
implement IEntity with an Id property. The constructor accepts a
filePath string pointing to the JSON file location. A SemaphoreSlim
object named _fileLock ensures thread-safe file operations when
multiple requests access the same file simultaneously.

The EnsureFileExists private method creates an empty JSON array file
if none exists at the specified path. The PopulateSampleDataIfEmpty
method adds demonstration users and claims when first initializing
the system.

GetAllAsync reads the entire file, deserializes the JSON array to
List<T>, and returns the collection. GetByIdAsync calls GetAllAsync
and uses LINQ FirstOrDefault to find the matching Id. AddAsync
calculates a new Id by finding the maximum existing Id plus one,
sets this Id on the entity using reflection, adds to collection,
and writes the updated JSON back to disk.

UpdateAsync replaces an existing entity by Id then saves the file.
DeleteAsync removes an entity by Id then persists changes. FindAsync
accepts an Expression<Func<T, bool>> predicate and returns filtered
results using LINQ Where clause.

**ClaimService Business Component:**

The ClaimService orchestrates claim management with dependencies on
IClaimRepository, IUserRepository, IApprovalRepository, INotificationService,
and IAuditService. Constructor injection provides these dependencies
through the ASP.NET Core DI container.

The SubmitClaimAsync method accepts a ClaimSubmissionDto and lecturerId
parameter. It first validates the submission by checking hours range,
month format, and lecturer eligibility. A validation failure returns
a ClaimSubmissionResult with error details.

Successful validation creates a new Claim object with calculated Amount
and Submitted status. The repository adds the claim and returns the
new ClaimId. InitializeApprovalWorkflowAsync creates Approval records
for ProgrammeCoordinator then AcademicManager approval steps.

Notifications send to the submitting lecturer confirming submission
and to all Programme Coordinators alerting them of pending work.
The audit service logs the complete submission details including
financial amount and timestamp.

**DocumentService File Management Component:**

DocumentService handles all file operations with dependencies on
IWebHostEnvironment for path resolution, IRepository<Document> for
metadata storage, and ILogger for error logging. Allowed file types
store in a static dictionary mapping extensions to MIME types.

The UploadDocumentAsync method validates the file first through
ValidateFileAsync which checks for null, zero length, maximum size,
extension whitelist, and MIME type verification. MIME verification
reads the file header bytes and compares magic numbers against
known signatures for each allowed file type.

GenerateSafeFileNameAsync removes special characters from original
names, replaces spaces with hyphens, adds a UTC timestamp and GUID
suffix for uniqueness, then converts to lowercase. Files save to
the wwwroot/uploads directory with this safe name.

Document metadata saves to the repository after successful file write.
Any failure during upload triggers cleanup of partially written files.
The audit service logs successful uploads with file size and name.

---

## 5. Development Phases

The development journey spanned ten weeks across three major phases,
each building upon the previous phase's foundation.

**Phase One: Foundation and Prototype at Four Weeks**

Week one focused on stakeholder requirement analysis and architectural
blueprint creation. We designed the initial UML class diagram and
established coding standards. The project structure was created with
clear separation of concerns across layers.

Week two implemented the basic MVC structure and session-based
authentication system. Base models and view models were created
with proper inheritance hierarchies. Routing configuration and
navigation menus were established for all user roles.

Week three delivered the dynamic claim submission form with real-time
JavaScript calculations. File upload prototype provided basic validation
for file types and sizes. Role-based access control middleware was
implemented to secure controller endpoints. Dashboard views were
created for each user role showing relevant information.

Week four connected all modules into a cohesive prototype system.
Comprehensive form validation was added with client and server
side checking. Initial user testing with sample data identified
areas for improvement. Documentation captured prototype capabilities
and limitations.

**Phase Two: Enhancement and Polish at Three Weeks**

Week five revolutionized data persistence by replacing in-memory
storage with JSON text file system. Automatic file initialization
populated sample data on first run. The repository pattern provided
clean abstraction for data access. Backup and recovery mechanisms
were added for data protection.

Week six completely redesigned the user interface with minimalist
aesthetics inspired by Apple design principles. Responsive layouts
adapted to mobile, tablet, and desktop screen sizes. Real-time form
validation gave immediate user feedback. Enhanced file upload added
drag-and-drop support with visual indicators.

Week seven implemented comprehensive error handling with custom
exception types and global filters. Audit logging captured all
critical operations including create, update, delete actions.
CSRF protection was added to all forms. Cross-browser compatibility
testing verified functionality across Chrome, Firefox, Safari, and Edge.

**Phase Three: Production Readiness at Three Weeks**

Week eight added advanced features including notification system
for real-time status updates. Advanced filtering and search allowed
users to find claims by date, status, amount, or lecturer name.
Export functionality generated PDF and Excel reports. Reporting
dashboard provided visual analytics with charts and graphs.

Week nine focused on performance and security optimization. JSON
serialization was optimized for large datasets using custom contract
resolvers. File cleanup service removed orphaned documents with no
associated claims. Rate limiting protected against brute force attacks.
Session security included sliding expiration and activity monitoring.

Week ten conducted comprehensive end-to-end testing covering all
user journeys. Load testing simulated fifty concurrent users making
requests. Complete system documentation was created including API
references and user guides. Deployment package and installation
guide prepared for production environments.

---

## 6. Feature Implementation

**Authentication Feature:**

The session management system uses ASP.NET Core Session with thirty
minute idle timeouts. Cookie security settings enforce HttpOnly flag,
SameSite Strict policy, and Secure policy requiring HTTPS. Custom
session extensions provide type-safe storage of UserSession objects.

The AuthenticationMiddleware checks every request against a list
of public paths including Login, Register, Error, and Home pages.
Any request to protected resources without a valid session triggers
redirect to Login page with session expired parameter. Session
validity checks include last activity timestamp comparison.

Role-based authorization implements a custom RoleAuthorizationAttribute
accepting an array of allowed UserRole values. The OnAuthorization method
extracts the current user's role from session and verifies membership
in the allowed roles. Forbidden results return for unauthorized access.

**Claim Submission Feature:**

The claim form includes real-time calculation of total amount based
on hours worked and hourly rate. JavaScript listens for input events
on both fields, performs multiplication, and updates the amount
display with currency formatting. The calculated amount also writes
to a hidden form field for server-side submission.

Server-side validation ensures hours worked does not exceed two
hundred hours per month, does not go below one hour, and increments
in half-hour steps. Month validation checks correct YYYY-MM format
and prevents future month submissions. Rate validation ensures
positive values consistent with lecturer contracts.

**Document Management Feature:**

The file upload system provides drag-and-drop functionality with
visual feedback. Drop zone highlighting activates during drag over
events. File validation occurs on drop or selection with immediate
error messaging for invalid files. File list displays each uploaded
item with name, size, and remove button.

MIME type validation reads file headers to verify actual file type
matches the extension. PDF files must start with %PDF signature.
Office documents are checked for PK or D0 CF 11 E0 signatures.
Images must match expected JPEG or PNG magic numbers. This prevents
malicious files disguised with fake extensions.

**Approval Workflow Feature:**

The approval workflow automatically initiates when a lecturer submits
a claim. Approval records are created in sequence: first Programme
Coordinator approval order one, then Academic Manager approval order
two. The CanOverride flag enables managers to approve claims even
if coordinator has not yet reviewed.

Approvers see a list of pending claims requiring their attention.
Each claim displays lecturer name, month, hours, amount, and attached
documents. Approvers select Approve or Reject buttons and must provide
comments explaining their decision.

Claim status updates based on approval outcomes. Rejection at any
stage moves claim to Rejected status and notifies the lecturer.
Full approval from both steps moves claim to Approved status ready
for payment processing. Partial approval keeps claim at UnderReview
until all steps complete.

---

## 7. Technical Specifications

The UML class diagram defines the complete object model with inheritance
hierarchies and relationships. The abstract User class sits at the top
with concrete Lecturer, ProgrammeCoordinator, and AcademicManager classes
inheriting from appropriate base classes.

One-to-many relationships exist between Lecturer and Claim entities,
Claim and Document entities, and Claim and Approval entities. Each
relationship includes proper navigation properties and foreign keys.
Enumerations define UserRole with Lecturer, ProgrammeCoordinator,
AcademicManager, HumanResource values and ClaimStatus with Draft,
Submitted, UnderReview, Approved, Rejected, Paid, Archived values.

The JSON serialization engine uses Newtonsoft.Json with custom contract
resolver that ignores navigation properties to prevent circular reference
errors during serialization. Property names convert to camelCase for
consistent JSON formatting. Null values and default values are ignored
to reduce file size.

File validation pipeline implements the chain of responsibility pattern
with multiple validators executing in sequence. The SizeValidator checks
against five megabyte maximum. ExtensionValidator verifies allowed types.
MimeTypeValidator reads file signatures. VirusScannerValidator integrates
antivirus checking. ContentIntegrityValidator validates PDF structure.

Each validator returns a ValidatorResult with IsValid flag, errors list,
and IsCritical flag. Critical failures stop the validation chain while
non-critical warnings allow continued validation. All validation occurs
on the server side after upload, preventing client-side bypass attempts.

---

## 8. Design System

The visual design follows Apple-inspired principles emphasizing clarity,
consistency, and restraint. Typography uses system fonts with San Francisco
or Segoe UI depending on operating system. Font sizes follow a modular
scale with base size of sixteen pixels for body text.

Color palette employs a monochromatic neutral base with strategic accent
colors for interactive elements. Primary actions use blue with sufficient
contrast ratio against backgrounds. Success states display green, errors
display red, and warnings display amber. All color combinations meet
WCAG 2.1 AA accessibility standards.

Spacing follows an eight-pixel grid system for consistent alignment
across components. Margin and padding values are multiples of eight.
Card components have sixteen pixel internal padding and twelve pixel
gaps between elements. Form groups stack vertically with eight pixel
gaps between label, input, and validation message.

Responsive breakpoints follow mobile-first approach with base styles
for all devices. Tablet breakpoint at seven hundred sixty-eight pixels
reorganizes multi-column forms into grid layouts. Desktop breakpoint
at one thousand twenty-four pixels introduces dashboard sidebar with
navigation on the left and content on the right. Large desktop breakpoint
at one thousand four hundred forty pixels centers content with maximum
width container.

Component library includes cards with header, body, and footer sections.
Cards use subtle border and shadow for elevation without distraction.
Form controls have consistent height of forty pixels, rounded corners
of eight pixels, and transition animations for focus states. Buttons
come in primary, secondary, and danger variants with clear hierarchy
for user actions.

---

## 9. Data Persistence Strategy

The text file storage system deliberately avoids traditional databases
to demonstrate that sophisticated applications can function without
complex database dependencies. Each JSON file serves as a table storing
an array of objects with schema validation at application level.

Data files reside in the App_Data directory at the application root.
The directory is created automatically on first run if it does not exist.
Each file initializes with an empty JSON array when first accessed.
Sample data populates for users, claims, and approvals to demonstrate
system functionality without manual data entry.

File locking using SemaphoreSlim prevents concurrent write conflicts
when multiple requests attempt to update the same file simultaneously.
The lock allows only one thread to write at a time while still allowing
multiple concurrent reads. This balances data integrity with performance.

JSON serialization settings configure indented formatting for human
readability during debugging and manual inspection. Null value handling
ignores properties with null values to reduce file size. Reference loop
handling ignores circular references that would otherwise cause exceptions.

The audit log writes to a separate audit.log file in plain text format
with each entry on a new line. Timestamps use ISO 8601 format for
consistent parsing across time zones. Log entries are never deleted
to maintain complete compliance records.

Data backup strategy recommends daily compression of the entire App_Data
folder with retention of thirty daily backups. Uploaded documents stored
in wwwroot/uploads are included in backup archives. Restoration involves
extracting the backup archive to the application root directory.

---

## 10. Security Framework

Session management implements thirty minute idle timeout with sliding
expiration, resetting the timeout on each user activity. Session cookies
are HttpOnly preventing JavaScript access, SameSite Strict preventing
cross-site requests, and Secure only transmitting over HTTPS.

The authentication middleware validates session on every request to
protected resources. Invalid sessions trigger session clear and redirect
to login page with expiration parameter. Session data includes UserId,
UserRole, Username, and LastActivity timestamp for validation.

Password hashing uses BCrypt algorithm with work factor twelve,
providing computational resistance to brute force attacks. Plain
text passwords are never stored in files or logged anywhere.
Password reset flow requires email verification before allowing changes.

CSRF protection uses anti-forgery tokens on all state-changing requests.
The ValidateAntiForgeryToken attribute checks tokens on POST operations.
Tokens are unique per user session and per request to prevent replay
attacks. Token validation occurs before any business logic executes.

File upload security validates both client-side and server-side to
prevent malicious file uploads. File permissions on the uploads
directory restrict execution of script files. File names are sanitized
to remove path traversal characters. Virus scanning verifies content
safety before saving to disk.

SQL injection is not applicable due to text file storage instead of
relational databases. However, JSON injection is prevented by using
parameterized serialization rather than string concatenation. All
user input is validated before inclusion in JSON output.

Rate limiting middleware restricts login attempts to five per minute
per IP address to prevent brute force attacks. Failed attempt counter
increments in memory with sliding expiration. Exceeding the limit
triggers temporary block for five minutes with user notification.

---

## 11. Quality Assurance

Unit testing covers all service layer business logic using xUnit
framework with Moq for dependency mocking. The ClaimService tests
validate calculation accuracy, validation rules, and approval workflow.
Success path testing confirms happy path functionality. Failure path
testing ensures proper error handling and user messaging.

Test methods follow Arrange-Act-Assert pattern for clear structure.
Arrange section initializes mocks and test data. Act section invokes
the method under test. Assert section verifies expected outcomes
using fluent assertions for readable failure messages.

Data-driven tests using Theory attribute and InlineData explore
multiple input combinations for calculation methods. Decimal hours
and rate combinations verify rounding to two decimal places with
away-from-zero midpoint rounding. Edge cases include zero values,
negative values, and maximum boundary values.

Integration tests verify end-to-end functionality using TestServer
and WebApplicationFactory. These tests initialize a real database
instance then execute complete user journeys through HTTP endpoints.
Authentication tokens are obtained and used for authorized requests.

Load testing simulates concurrent user activity to identify performance
bottlenecks. The simulator creates multiple parallel tasks each
behaving like a real user. Metrics tracked include response time,
throughput, error rate, and resource utilization. Results are logged
for performance baseline establishment.

Security testing includes OWASP Top Ten vulnerability scanning using
automated tools. Penetration testing attempts cross-site scripting,
CSRF, SQL injection, and authentication bypass attacks. All findings
are remediated before production deployment.

User acceptance testing involves stakeholders following predefined
test scripts. Each user role validates their specific workflows including
claim submission, approval review, document upload, and report generation.
Defects are tracked with severity levels and fixed with priority.

---

## 12. Deployment Guide

System requirements include Windows Server 2016 or newer, or Linux
distribution such as Ubuntu 20.04 or CentOS 7 for hosting. NET 8.0
Runtime must be installed before application deployment. Memory
requirements specify two gigabytes minimum with four gigabytes
recommended for production workloads. Disk space requires ten
gigabytes free for application, data files, and uploaded documents.

Windows deployment uses IIS with the ASP.NET Core Module configured
for request handling. Application pools should configure No Managed
Code with Integrated pipeline mode. IIS user requires write permissions
to App_Data and wwwroot/uploads directories.

Linux deployment uses Nginx as reverse proxy with systemd service
for application management. The proxy passes requests to Kestrel
running on localhost port five thousand. Static files are served
directly by Nginx for improved performance.

SSL/TLS configuration requires certificates from trusted certificate
authority. Let's Encrypt provides free certificates with automated
renewal. Certbot tool configures Nginx with HTTPS redirection and
secure cipher suites. HSTS header forces all future connections to HTTPS.

Environment configuration sets ASPNETCORE_ENVIRONMENT to Production
which enables production optimizations and disables developer error
pages. Connection strings are not needed due to file-based storage.
Logging configuration adjusts minimum levels for different components.

Firewall rules must allow HTTP on port eighty, HTTPS on port four
hundred forty-three for web traffic, and SSH on port twenty-two for
administration. Database ports are not required due to file storage.
Monitoring ports for metrics and health checks may be opened as needed.

Monitoring solution uses Prometheus for metrics collection and Grafana
for visualization. Exposed metrics include request count, duration,
error rate, active sessions, and file operation counts. Alert rules
trigger on high error rates or session failures.

Backup automation runs daily at midnight compressing App_Data and
uploads directories. Retention policy keeps thirty daily backups,
twelve monthly backups, and seven yearly backups. Backup verification
restores to test environment weekly to ensure integrity.

---

## 13. Repository Structure

The GitHub repository at the provided URL follows standard ASP.NET
Core MVC project structure with additional documentation folders.

Root directory contains the solution file, global.json for SDK version,
and README.md with project overview. The .gitignore file excludes
App_Data, uploads, bin, obj, and user-specific configuration files.

Source code resides in the root project directory with Controllers,
Models, Views, Services, and Data folders separating concerns.
Program.cs configures the host builder with session, services, and
middleware pipeline. appsettings.json stores configuration with
environment-specific overrides in appsettings.Development.json and
appsettings.Production.json.

wwwroot folder holds static assets including CSS stylesheets, JavaScript
files, and library dependencies. Uploads subdirectory within wwwroot
stores claim documents with unique filenames. Bootstrap and jQuery
are served from local files rather than CDNs for offline compatibility.

Documentation folder contains API reference generated from XML comments,
user guide with screenshots, administrator handbook for configuration,
and deployment guide with step-by-step instructions.

---

End of Document

---
