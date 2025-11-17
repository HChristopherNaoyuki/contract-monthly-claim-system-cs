using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using contract_monthly_claim_system_cs.Models.DataModels;

namespace contract_monthly_claim_system_cs.Services
{
    /// <summary>
    /// Enhanced TextFileDataService with comprehensive data operations
    /// Part 3 POE requirement: Text file storage instead of database
    /// Provides automated data management, analytics, and reporting capabilities
    /// Handles all CRUD operations for users, lecturers, claims, documents, and approvals
    /// </summary>
    public class TextFileDataService
    {
        private readonly string _dataDirectory;
        private readonly ILogger<TextFileDataService> _logger;

        /// <summary>
        /// Initializes a new instance of TextFileDataService with enhanced automation
        /// Creates data directory and initializes sample data if needed
        /// </summary>
        /// <param name="logger">Logger instance for tracking operations</param>
        public TextFileDataService(ILogger<TextFileDataService> logger)
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _logger = logger;

            // Ensure data directory exists
            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
                _logger.LogInformation("Created automated data directory: {DataDirectory}", _dataDirectory);
            }

            // Initialize data structures and sample data
            InitializeAutomatedDataStructures();
        }

        /// <summary>
        /// Initializes automated data structures and ensures data integrity
        /// Part 3 requirement for robust data management
        /// Creates all required data files and populates with sample data if empty
        /// </summary>
        private void InitializeAutomatedDataStructures()
        {
            try
            {
                // Ensure all required data files exist with proper structure
                var requiredFiles = new[] { "users", "claims", "documents", "approvals", "lecturers" };

                foreach (var file in requiredFiles)
                {
                    var filePath = GetFilePath(file);
                    if (!File.Exists(filePath))
                    {
                        // Create empty list for each data type
                        WriteData(file, new List<object>());
                        _logger.LogInformation("Created automated data file: {FilePath}", filePath);
                    }
                    else
                    {
                        // Verify file is readable and has valid JSON structure
                        VerifyFileStructure(filePath);
                    }
                }

                // Initialize sample data for demonstration if no data exists
                InitializeSampleDataForAutomation();

                _logger.LogInformation("Automated data structures initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize automated data structures");
                throw new InvalidOperationException("Failed to initialize text file storage system", ex);
            }
        }

        /// <summary>
        /// Verifies that a data file has valid JSON structure
        /// Attempts to repair corrupted files automatically
        /// </summary>
        /// <param name="filePath">Path to the data file to verify</param>
        private void VerifyFileStructure(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        // Try to parse JSON to verify structure
                        JsonSerializer.Deserialize<List<object>>(json);
                        _logger.LogDebug("File structure verified: {FilePath}", filePath);
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.LogWarning(jsonEx, "Invalid JSON structure detected in {FilePath}, attempting repair", filePath);

                // Create backup of corrupted file
                CreateDataBackup(filePath);

                // Reset file with empty list
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                WriteData(fileName, new List<object>());

                _logger.LogInformation("File repaired: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify file structure for {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Enhanced method to get file path with validation
        /// Prevents directory traversal attacks and ensures file security
        /// </summary>
        /// <param name="dataType">Type of data file (users, claims, documents, etc.)</param>
        /// <returns>Validated file path</returns>
        private string GetFilePath(string dataType)
        {
            var fileName = $"{dataType}.txt";
            var filePath = Path.Combine(_dataDirectory, fileName);

            // Security validation - Part 3 requirement
            if (!fileName.EndsWith(".txt") || fileName.Contains(".."))
            {
                throw new ArgumentException("Invalid data file name - potential security risk");
            }

            return filePath;
        }

        /// <summary>
        /// Enhanced data reading with automated error handling and recovery
        /// Part 3 requirement for robust data access
        /// Implements retry logic and automatic file repair for corrupted data
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <returns>List of data objects with automated recovery</returns>
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
                            var result = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                            _logger.LogDebug("Successfully read {Count} items from {FilePath}", result.Count, filePath);
                            return result;
                        }
                    }
                    break;
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogWarning(jsonEx, "JSON parsing error on attempt {Attempt} for {FilePath}", attempt, filePath);

                    if (attempt == maxRetries)
                    {
                        // Automated recovery - create backup and reset file
                        CreateDataBackup(filePath);
                        WriteData(dataType, new List<T>());
                        _logger.LogWarning("Automated recovery completed for corrupted file: {FilePath}", filePath);
                        return new List<T>();
                    }

                    System.Threading.Thread.Sleep(100 * attempt); // Exponential backoff
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reading data from {FilePath} on attempt {Attempt}", filePath, attempt);

                    if (attempt == maxRetries)
                    {
                        throw new InvalidOperationException($"Failed to read data after {maxRetries} attempts", ex);
                    }

                    System.Threading.Thread.Sleep(100 * attempt);
                }
            }

            return new List<T>();
        }

        /// <summary>
        /// Enhanced data writing with automated backup and validation
        /// Part 3 requirement for data integrity
        /// Implements atomic write operations with temporary files
        /// </summary>
        /// <typeparam name="T">Type of data to write</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <param name="data">Data to write with validation</param>
        private void WriteData<T>(string dataType, List<T> data)
        {
            var filePath = GetFilePath(dataType);

            try
            {
                // Create backup before writing - Part 3 requirement
                CreateDataBackup(filePath);

                // Validate data before writing
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "Data cannot be null");
                }

                // Configure JSON serialization options
                var jsonOptions = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(data, jsonOptions);

                // Atomic write operation using temporary file
                var tempFilePath = filePath + ".tmp";
                File.WriteAllText(tempFilePath, json);

                // Replace original file with temporary file
                File.Move(tempFilePath, filePath, true);

                _logger.LogDebug("Successfully wrote {Count} items to {FilePath}", data.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing data to {FilePath}", filePath);
                throw new InvalidOperationException($"Failed to write data to {dataType} storage", ex);
            }
        }

        /// <summary>
        /// Creates automated backup of data file
        /// Part 3 requirement for data protection
        /// Maintains backup copies for disaster recovery
        /// </summary>
        /// <param name="filePath">Path to file to backup</param>
        private void CreateDataBackup(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var backupPath = filePath + ".backup";
                    File.Copy(filePath, backupPath, true);
                    _logger.LogDebug("Created automated backup: {BackupPath}", backupPath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create backup for {FilePath}", filePath);
                // Don't throw - backup failure shouldn't prevent main operation
            }
        }

        #region User Operations

        /// <summary>
        /// Retrieves all users from text file storage
        /// Used for authentication, authorization, and user management
        /// </summary>
        /// <returns>List of all users in the system</returns>
        public List<User> GetAllUsers()
        {
            return ReadData<User>("users");
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// Used for user lookup and profile management
        /// </summary>
        /// <param name="userId">User ID to search for</param>
        /// <returns>User object or null if not found</returns>
        public User? GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Retrieves a user by username for authentication
        /// Used during login process and username validation
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User object or null if not found</returns>
        public User? GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Saves a user to text file storage with automated validation
        /// Handles both new user creation and existing user updates
        /// </summary>
        /// <param name="user">User object to save</param>
        public void SaveUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            // Validate required fields
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
            {
                throw new ArgumentException("Username and password are required");
            }

            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.UserId == user.UserId);

            if (existingUser != null)
            {
                // Update existing user
                users.Remove(existingUser);
                _logger.LogDebug("Updating existing user: {Username}", user.Username);
            }
            else
            {
                _logger.LogDebug("Creating new user: {Username}", user.Username);
            }

            users.Add(user);
            WriteData("users", users);
            _logger.LogInformation("User saved successfully: {Username} (ID: {UserId})", user.Username, user.UserId);
        }

        #endregion

        #region Lecturer Operations

        /// <summary>
        /// Retrieves all lecturers from text file storage
        /// Used for HR analytics and lecturer management
        /// </summary>
        /// <returns>List of all lecturers in the system</returns>
        public List<Lecturer> GetAllLecturers()
        {
            return ReadData<Lecturer>("lecturers");
        }

        /// <summary>
        /// Retrieves a specific lecturer by ID
        /// Used for claim processing and lecturer profile access
        /// </summary>
        /// <param name="lecturerId">Lecturer ID to search for</param>
        /// <returns>Lecturer object or null if not found</returns>
        public Lecturer? GetLecturerById(int lecturerId)
        {
            var lecturers = GetAllLecturers();
            return lecturers.FirstOrDefault(l => l.LecturerId == lecturerId);
        }

        /// <summary>
        /// Saves a lecturer to text file storage
        /// Handles both new lecturer creation and existing lecturer updates
        /// </summary>
        /// <param name="lecturer">Lecturer object to save</param>
        public void SaveLecturer(Lecturer lecturer)
        {
            if (lecturer == null)
            {
                throw new ArgumentNullException(nameof(lecturer));
            }

            var lecturers = GetAllLecturers();
            var existingLecturer = lecturers.FirstOrDefault(l => l.LecturerId == lecturer.LecturerId);

            if (existingLecturer != null)
            {
                // Update existing lecturer
                lecturers.Remove(existingLecturer);
                _logger.LogDebug("Updating existing lecturer: ID {LecturerId}", lecturer.LecturerId);
            }
            else
            {
                _logger.LogDebug("Creating new lecturer: ID {LecturerId}", lecturer.LecturerId);
            }

            lecturers.Add(lecturer);
            WriteData("lecturers", lecturers);
            _logger.LogInformation("Lecturer saved successfully: ID {LecturerId}", lecturer.LecturerId);
        }

        #endregion

        #region Claim Operations

        /// <summary>
        /// Retrieves all claims from text file storage
        /// Used for claim tracking, approval workflows, and analytics
        /// </summary>
        /// <returns>List of all claims in the system</returns>
        public List<Claim> GetAllClaims()
        {
            return ReadData<Claim>("claims");
        }

        /// <summary>
        /// Retrieves a specific claim by ID
        /// Used for claim status viewing and approval processing
        /// </summary>
        /// <param name="claimId">Claim ID to search for</param>
        /// <returns>Claim object or null if not found</returns>
        public Claim? GetClaimById(int claimId)
        {
            var claims = GetAllClaims();
            return claims.FirstOrDefault(c => c.ClaimId == claimId);
        }

        /// <summary>
        /// Retrieves claims for a specific lecturer
        /// Used for lecturer-specific claim tracking and reporting
        /// </summary>
        /// <param name="lecturerId">Lecturer ID to filter claims</param>
        /// <returns>List of claims for the specified lecturer</returns>
        public List<Claim> GetClaimsByLecturerId(int lecturerId)
        {
            var claims = GetAllClaims();
            return claims.Where(c => c.LecturerId == lecturerId).ToList();
        }

        /// <summary>
        /// Saves a claim to text file storage with automated processing
        /// Handles both new claim submission and existing claim updates
        /// </summary>
        /// <param name="claim">Claim object to save</param>
        public void SaveClaim(Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            // Validate required fields
            if (claim.LecturerId <= 0 || claim.HoursWorked <= 0 || claim.HourlyRate <= 0)
            {
                throw new ArgumentException("Invalid claim data - missing required fields");
            }

            var claims = GetAllClaims();
            var existingClaim = claims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);

            if (existingClaim != null)
            {
                // Update existing claim
                claims.Remove(existingClaim);
                _logger.LogDebug("Updating existing claim: ID {ClaimId}", claim.ClaimId);
            }
            else
            {
                _logger.LogDebug("Creating new claim: ID {ClaimId}", claim.ClaimId);
            }

            claims.Add(claim);
            WriteData("claims", claims);
            _logger.LogInformation("Claim saved successfully: ID {ClaimId} for Lecturer {LecturerId}",
                claim.ClaimId, claim.LecturerId);
        }

        #endregion

        #region Document Operations

        /// <summary>
        /// Retrieves all documents from text file storage
        /// Used for document management and claim verification
        /// </summary>
        /// <returns>List of all documents in the system</returns>
        public List<Document> GetAllDocuments()
        {
            return ReadData<Document>("documents");
        }

        /// <summary>
        /// Retrieves documents for a specific claim
        /// Used for displaying supporting documents with claims
        /// </summary>
        /// <param name="claimId">Claim ID to filter documents</param>
        /// <returns>List of documents for the specified claim</returns>
        public List<Document> GetDocumentsByClaimId(int claimId)
        {
            var documents = GetAllDocuments();
            return documents.Where(d => d.ClaimId == claimId).ToList();
        }

        /// <summary>
        /// Saves a document to text file storage
        /// Handles document uploads and associations with claims
        /// </summary>
        /// <param name="document">Document object to save</param>
        public void SaveDocument(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            // Validate required fields
            if (string.IsNullOrEmpty(document.FileName) || string.IsNullOrEmpty(document.FilePath))
            {
                throw new ArgumentException("Document filename and path are required");
            }

            var documents = GetAllDocuments();
            var existingDocument = documents.FirstOrDefault(d => d.DocumentId == document.DocumentId);

            if (existingDocument != null)
            {
                // Update existing document
                documents.Remove(existingDocument);
                _logger.LogDebug("Updating existing document: {FileName}", document.FileName);
            }
            else
            {
                _logger.LogDebug("Creating new document: {FileName}", document.FileName);
            }

            documents.Add(document);
            WriteData("documents", documents);
            _logger.LogInformation("Document saved successfully: {FileName} for Claim {ClaimId}",
                document.FileName, document.ClaimId);
        }

        #endregion

        #region Approval Operations

        /// <summary>
        /// Retrieves all approvals from text file storage
        /// Used for audit trails and approval history
        /// </summary>
        /// <returns>List of all approvals in the system</returns>
        public List<Approval> GetAllApprovals()
        {
            return ReadData<Approval>("approvals");
        }

        /// <summary>
        /// Retrieves approvals for a specific claim
        /// Used for displaying approval history and comments
        /// </summary>
        /// <param name="claimId">Claim ID to filter approvals</param>
        /// <returns>List of approvals for the specified claim</returns>
        public List<Approval> GetApprovalsByClaimId(int claimId)
        {
            var approvals = GetAllApprovals();
            return approvals.Where(a => a.ClaimId == claimId).ToList();
        }

        /// <summary>
        /// Saves an approval to text file storage
        /// Handles approval and rejection decisions with audit trail
        /// </summary>
        /// <param name="approval">Approval object to save</param>
        public void SaveApproval(Approval approval)
        {
            if (approval == null)
            {
                throw new ArgumentNullException(nameof(approval));
            }

            // Validate required fields
            if (approval.ClaimId <= 0 || approval.ApproverUserId <= 0)
            {
                throw new ArgumentException("Invalid approval data - missing required fields");
            }

            var approvals = GetAllApprovals();
            var existingApproval = approvals.FirstOrDefault(a => a.ApprovalId == approval.ApprovalId);

            if (existingApproval != null)
            {
                // Update existing approval
                approvals.Remove(existingApproval);
                _logger.LogDebug("Updating existing approval: ID {ApprovalId}", approval.ApprovalId);
            }
            else
            {
                _logger.LogDebug("Creating new approval: ID {ApprovalId}", approval.ApprovalId);
            }

            approvals.Add(approval);
            WriteData("approvals", approvals);
            _logger.LogInformation("Approval saved successfully: Claim {ClaimId} by User {UserId}",
                approval.ClaimId, approval.ApproverUserId);
        }

        #endregion

        #region System Operations

        /// <summary>
        /// Gets the next available ID with automated sequence management
        /// Part 3 requirement for reliable ID generation
        /// Ensures unique IDs across all data types
        /// </summary>
        /// <param name="dataType">Type of data (users, claims, documents, approvals, lecturers)</param>
        /// <returns>Next available ID with sequence validation</returns>
        public int GetNextId(string dataType)
        {
            try
            {
                var ids = dataType.ToLower() switch
                {
                    "users" => GetAllUsers().Select(u => u.UserId),
                    "claims" => GetAllClaims().Select(c => c.ClaimId),
                    "documents" => GetAllDocuments().Select(d => d.DocumentId),
                    "approvals" => GetAllApprovals().Select(a => a.ApprovalId),
                    "lecturers" => GetAllLecturers().Select(l => l.LecturerId),
                    _ => throw new ArgumentException($"Unknown data type: {dataType}")
                };

                var nextId = ids.Any() ? ids.Max() + 1 : 1;
                _logger.LogDebug("Generated next ID for {DataType}: {NextId}", dataType, nextId);
                return nextId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate next ID for {DataType}", dataType);
                return 1; // Fallback ID
            }
        }

        /// <summary>
        /// Initializes sample data for automation demonstration
        /// Part 3 requirement for system readiness
        /// Creates comprehensive sample data for testing and demonstration
        /// </summary>
        private void InitializeSampleDataForAutomation()
        {
            try
            {
                var existingUsers = GetAllUsers();
                if (existingUsers.Count == 0)
                {
                    _logger.LogInformation("Creating sample data for Part 3 POE demonstration...");

                    // Create comprehensive sample data for all roles
                    var sampleUsers = new[]
                    {
                        new User
                        {
                            UserId = 1,
                            Name = "System",
                            Surname = "Administrator",
                            Username = "admin",
                            Password = "admin123",
                            Role = UserRole.AcademicManager,
                            Email = "admin@cmcs.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new User
                        {
                            UserId = 2,
                            Name = "John",
                            Surname = "Smith",
                            Username = "lecturer",
                            Password = "lecturer123",
                            Role = UserRole.Lecturer,
                            Email = "john.smith@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new User
                        {
                            UserId = 3,
                            Name = "Sarah",
                            Surname = "Johnson",
                            Username = "coordinator",
                            Password = "coordinator123",
                            Role = UserRole.ProgrammeCoordinator,
                            Email = "sarah.johnson@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        },
                        new User
                        {
                            UserId = 4,
                            Name = "Michael",
                            Surname = "Brown",
                            Username = "hr",
                            Password = "hr123",
                            Role = UserRole.HumanResource,
                            Email = "michael.brown@university.com",
                            IsActive = true,
                            CreatedDate = DateTime.UtcNow
                        }
                    };

                    foreach (var user in sampleUsers)
                    {
                        SaveUser(user);
                        _logger.LogDebug("Created sample user: {Username}", user.Username);
                    }

                    // Create sample lecturer details
                    var lecturer = new Lecturer
                    {
                        LecturerId = 2, // Matches John Smith's user ID
                        EmployeeNumber = "EMP001",
                        Department = "Computer Science",
                        HourlyRate = 150.00m,
                        ContractStartDate = DateTime.Now.AddYears(-1),
                        ContractEndDate = DateTime.Now.AddYears(1),
                        BankAccountNumber = "123456789",
                        BankName = "Sample Bank",
                        TaxNumber = "TAX001"
                    };

                    SaveLecturer(lecturer);

                    // Create sample claims for Part 3 POE demonstration
                    var sampleClaim = new Claim
                    {
                        ClaimId = 1,
                        LecturerId = 2, // John Smith's claims
                        MonthYear = DateTime.Now.ToString("yyyy-MM"),
                        HoursWorked = 40,
                        HourlyRate = 150.00m,
                        Amount = 6000.00m,
                        Status = ClaimStatus.Submitted,
                        SubmissionComments = "Sample claim for Part 3 POE demonstration",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    SaveClaim(sampleClaim);

                    // Create additional sample claims for comprehensive testing
                    var additionalClaims = new[]
                    {
                        new Claim
                        {
                            ClaimId = 2,
                            LecturerId = 2,
                            MonthYear = DateTime.Now.AddMonths(-1).ToString("yyyy-MM"),
                            HoursWorked = 35,
                            HourlyRate = 150.00m,
                            Amount = 5250.00m,
                            Status = ClaimStatus.Approved,
                            SubmissionComments = "Previous month claim - approved",
                            CreatedDate = DateTime.Now.AddMonths(-1),
                            ModifiedDate = DateTime.Now.AddMonths(-1).AddDays(2)
                        },
                        new Claim
                        {
                            ClaimId = 3,
                            LecturerId = 2,
                            MonthYear = DateTime.Now.AddMonths(-2).ToString("yyyy-MM"),
                            HoursWorked = 45,
                            HourlyRate = 150.00m,
                            Amount = 6750.00m,
                            Status = ClaimStatus.Paid,
                            SubmissionComments = "Two months ago claim - paid",
                            CreatedDate = DateTime.Now.AddMonths(-2),
                            ModifiedDate = DateTime.Now.AddMonths(-2).AddDays(5)
                        }
                    };

                    foreach (var claim in additionalClaims)
                    {
                        SaveClaim(claim);
                    }

                    // Create sample approval records
                    var sampleApproval = new Approval
                    {
                        ApprovalId = 1,
                        ClaimId = 2, // Approved claim
                        ApproverUserId = 3, // Sarah Johnson (Coordinator)
                        ApproverRole = "ProgrammeCoordinator",
                        ApprovalDate = DateTime.Now.AddMonths(-1).AddDays(1),
                        IsApproved = true,
                        Comments = "Claim verified and approved",
                        ApprovalOrder = 1
                    };

                    SaveApproval(sampleApproval);

                    _logger.LogInformation("Sample data initialization completed successfully");
                }
                else
                {
                    _logger.LogInformation("Existing data found ({UserCount} users), skipping sample data initialization", existingUsers.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize sample data");
                throw new InvalidOperationException("Failed to initialize sample data for Part 3 POE", ex);
            }
        }

        /// <summary>
        /// Provides system statistics for monitoring and reporting
        /// Part 3 requirement for system analytics
        /// </summary>
        /// <returns>Comprehensive system statistics</returns>
        public SystemStatistics GetSystemStatistics()
        {
            try
            {
                var users = GetAllUsers();
                var claims = GetAllClaims();
                var approvals = GetAllApprovals();
                var lecturers = GetAllLecturers();

                return new SystemStatistics
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.IsActive),
                    TotalLecturers = lecturers.Count,
                    TotalClaims = claims.Count,
                    ApprovedClaims = claims.Count(c => c.Status == ClaimStatus.Approved),
                    PendingClaims = claims.Count(c => c.Status == ClaimStatus.Submitted),
                    TotalApprovals = approvals.Count,
                    AverageClaimAmount = claims.Any() ? claims.Average(c => c.Amount) : 0,
                    TotalAmountProcessed = claims.Where(c => c.Status == ClaimStatus.Approved || c.Status == ClaimStatus.Paid).Sum(c => c.Amount),
                    SystemUptime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime,
                    DataSize = CalculateTotalDataSize(),
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate system statistics");
                return new SystemStatistics { GeneratedAt = DateTime.Now };
            }
        }

        /// <summary>
        /// Calculates total data size for monitoring
        /// Part 3 requirement for resource management
        /// </summary>
        /// <returns>Total data size in bytes</returns>
        private long CalculateTotalDataSize()
        {
            try
            {
                var dataFiles = Directory.GetFiles(_dataDirectory, "*.txt");
                return dataFiles.Sum(file => new FileInfo(file).Length);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to calculate total data size");
                return 0;
            }
        }

        #endregion
    }

    /// <summary>
    /// Comprehensive system statistics for monitoring and reporting
    /// Part 3 requirement for system analytics
    /// </summary>
    public class SystemStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int PendingClaims { get; set; }
        public int TotalApprovals { get; set; }
        public decimal AverageClaimAmount { get; set; }
        public decimal TotalAmountProcessed { get; set; }
        public TimeSpan SystemUptime { get; set; }
        public long DataSize { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets the approval rate as a percentage
        /// </summary>
        public decimal ApprovalRate
        {
            get
            {
                if (TotalClaims == 0) return 0;
                return Math.Round((decimal)ApprovedClaims / TotalClaims * 100, 2);
            }
        }

        /// <summary>
        /// Gets the average claims per lecturer
        /// </summary>
        public decimal AverageClaimsPerLecturer
        {
            get
            {
                if (TotalLecturers == 0) return 0;
                return Math.Round((decimal)TotalClaims / TotalLecturers, 2);
            }
        }
    }
}