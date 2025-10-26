using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using contract_monthly_claim_system_cs.Models.DataModels;
using System.Diagnostics;

namespace contract_monthly_claim_system_cs.Services
{
    /// <summary>
    /// Enhanced TextFileDataService with comprehensive data operations
    /// Part 3 POE requirement: Text file storage instead of database
    /// Provides automated data management, analytics, and reporting capabilities
    /// </summary>
    public class TextFileDataService
    {
        private readonly string _dataDirectory;
        private readonly ILogger<TextFileDataService> _logger;

        /// <summary>
        /// Initializes a new instance of TextFileDataService with enhanced automation
        /// </summary>
        /// <param name="logger">Logger instance for tracking operations</param>
        public TextFileDataService(ILogger<TextFileDataService> logger)
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _logger = logger;

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
                _logger.LogInformation("Created automated data directory: {DataDirectory}", _dataDirectory);
            }

            InitializeAutomatedDataStructures();
        }

        /// <summary>
        /// Initializes automated data structures and ensures data integrity
        /// Part 3 requirement for robust data management
        /// </summary>
        private void InitializeAutomatedDataStructures()
        {
            try
            {
                // Ensure all required data files exist with proper structure
                var requiredFiles = new[] { "users", "claims", "documents", "approvals", "lecturers", "analytics" };

                foreach (var file in requiredFiles)
                {
                    var filePath = GetFilePath(file);
                    if (!File.Exists(filePath))
                    {
                        WriteData(file, new List<object>());
                        _logger.LogInformation("Created automated data file: {FilePath}", filePath);
                    }
                }

                // Initialize sample data for demonstration
                InitializeSampleDataForAutomation();

                _logger.LogInformation("Automated data structures initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize automated data structures");
                throw;
            }
        }

        /// <summary>
        /// Enhanced method to get file path with validation
        /// </summary>
        /// <param name="dataType">Type of data file</param>
        /// <returns>Validated file path</returns>
        private string GetFilePath(string dataType)
        {
            var fileName = $"{dataType}.txt";
            var filePath = Path.Combine(_dataDirectory, fileName);

            // Security validation - Part 3 requirement
            if (!fileName.EndsWith(".txt") || fileName.Contains(".."))
            {
                throw new ArgumentException("Invalid data file name");
            }

            return filePath;
        }

        /// <summary>
        /// Enhanced data reading with automated error handling and recovery
        /// Part 3 requirement for robust data access
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

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                // Atomic write operation
                var tempFilePath = filePath + ".tmp";
                File.WriteAllText(tempFilePath, json);
                File.Move(tempFilePath, filePath, true);

                _logger.LogDebug("Successfully wrote {Count} items to {FilePath}", data.Count, filePath);

                // Update analytics after successful write - Part 3 requirement
                UpdateAnalytics(dataType, data.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing data to {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Creates automated backup of data file
        /// Part 3 requirement for data protection
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
            }
        }

        /// <summary>
        /// Updates automated analytics for data operations
        /// Part 3 requirement for monitoring and reporting
        /// </summary>
        /// <param name="dataType">Type of data operation</param>
        /// <param name="recordCount">Number of records processed</param>
        private void UpdateAnalytics(string dataType, int recordCount)
        {
            try
            {
                var analytics = ReadData<DataAnalytics>("analytics");
                var today = DateTime.Today.ToString("yyyy-MM-dd");

                var dailyAnalytic = analytics.FirstOrDefault(a => a.Date == today && a.DataType == dataType);
                if (dailyAnalytic == null)
                {
                    dailyAnalytic = new DataAnalytics
                    {
                        Date = today,
                        DataType = dataType,
                        OperationCount = 0,
                        RecordCount = 0
                    };
                    analytics.Add(dailyAnalytic);
                }

                dailyAnalytic.OperationCount++;
                dailyAnalytic.RecordCount += recordCount;
                dailyAnalytic.LastUpdated = DateTime.Now;

                WriteData("analytics", analytics);

                _logger.LogDebug("Updated analytics for {DataType}: {Operations} operations, {Records} records",
                    dataType, dailyAnalytic.OperationCount, dailyAnalytic.RecordCount);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update analytics for {DataType}", dataType);
            }
        }

        #region User Operations

        /// <summary>
        /// Retrieves all users from text file storage
        /// </summary>
        /// <returns>List of all users in the system</returns>
        public List<User> GetAllUsers()
        {
            return ReadData<User>("users");
        }

        /// <summary>
        /// Retrieves a specific user by ID
        /// </summary>
        /// <param name="userId">User ID to search for</param>
        /// <returns>User object or null if not found</returns>
        public User GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Retrieves a user by username for authentication
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User object or null if not found</returns>
        public User GetUserByUsername(string username)
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
        /// </summary>
        /// <param name="user">User object to save</param>
        public void SaveUser(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.UserId == user.UserId);

            if (existingUser != null)
            {
                users.Remove(existingUser);
            }

            users.Add(user);
            WriteData("users", users);
            _logger.LogInformation("User saved with automation: {Username} (ID: {UserId})", user.Username, user.UserId);
        }

        #endregion

        #region Lecturer Operations

        /// <summary>
        /// Retrieves all lecturers from text file storage
        /// </summary>
        /// <returns>List of all lecturers in the system</returns>
        public List<Lecturer> GetAllLecturers()
        {
            return ReadData<Lecturer>("lecturers");
        }

        /// <summary>
        /// Retrieves a specific lecturer by ID
        /// </summary>
        /// <param name="lecturerId">Lecturer ID to search for</param>
        /// <returns>Lecturer object or null if not found</returns>
        public Lecturer GetLecturerById(int lecturerId)
        {
            var lecturers = GetAllLecturers();
            return lecturers.FirstOrDefault(l => l.LecturerId == lecturerId);
        }

        /// <summary>
        /// Saves a lecturer to text file storage
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
                lecturers.Remove(existingLecturer);
            }

            lecturers.Add(lecturer);
            WriteData("lecturers", lecturers);
            _logger.LogInformation("Lecturer saved: ID {LecturerId}", lecturer.LecturerId);
        }

        #endregion

        #region Claim Operations

        /// <summary>
        /// Retrieves all claims from text file storage
        /// </summary>
        /// <returns>List of all claims in the system</returns>
        public List<Claim> GetAllClaims()
        {
            return ReadData<Claim>("claims");
        }

        /// <summary>
        /// Retrieves a specific claim by ID
        /// </summary>
        /// <param name="claimId">Claim ID to search for</param>
        /// <returns>Claim object or null if not found</returns>
        public Claim GetClaimById(int claimId)
        {
            var claims = GetAllClaims();
            return claims.FirstOrDefault(c => c.ClaimId == claimId);
        }

        /// <summary>
        /// Retrieves claims for a specific lecturer
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
        /// </summary>
        /// <param name="claim">Claim object to save</param>
        public void SaveClaim(Claim claim)
        {
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            var claims = GetAllClaims();
            var existingClaim = claims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);

            if (existingClaim != null)
            {
                claims.Remove(existingClaim);
            }

            claims.Add(claim);
            WriteData("claims", claims);
            _logger.LogInformation("Claim saved with automation: ID {ClaimId} for Lecturer {LecturerId}",
                claim.ClaimId, claim.LecturerId);
        }

        #endregion

        #region Document Operations

        /// <summary>
        /// Retrieves all documents from text file storage
        /// </summary>
        /// <returns>List of all documents in the system</returns>
        public List<Document> GetAllDocuments()
        {
            return ReadData<Document>("documents");
        }

        /// <summary>
        /// Retrieves documents for a specific claim
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
        /// </summary>
        /// <param name="document">Document object to save</param>
        public void SaveDocument(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            var documents = GetAllDocuments();
            var existingDocument = documents.FirstOrDefault(d => d.DocumentId == document.DocumentId);

            if (existingDocument != null)
            {
                documents.Remove(existingDocument);
            }

            documents.Add(document);
            WriteData("documents", documents);
            _logger.LogInformation("Document saved: {FileName} for Claim {ClaimId}",
                document.FileName, document.ClaimId);
        }

        #endregion

        #region Approval Operations

        /// <summary>
        /// Retrieves all approvals from text file storage
        /// </summary>
        /// <returns>List of all approvals in the system</returns>
        public List<Approval> GetAllApprovals()
        {
            return ReadData<Approval>("approvals");
        }

        /// <summary>
        /// Retrieves approvals for a specific claim
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
        /// </summary>
        /// <param name="approval">Approval object to save</param>
        public void SaveApproval(Approval approval)
        {
            if (approval == null)
            {
                throw new ArgumentNullException(nameof(approval));
            }

            var approvals = GetAllApprovals();
            var existingApproval = approvals.FirstOrDefault(a => a.ApprovalId == approval.ApprovalId);

            if (existingApproval != null)
            {
                approvals.Remove(existingApproval);
            }

            approvals.Add(approval);
            WriteData("approvals", approvals);
            _logger.LogInformation("Approval saved: Claim {ClaimId} by User {UserId}",
                approval.ClaimId, approval.ApproverUserId);
        }

        #endregion

        #region Analytics and System Operations

        /// <summary>
        /// Gets automated statistics for system monitoring
        /// Part 3 requirement for system analytics
        /// </summary>
        /// <returns>System statistics with automated calculations</returns>
        public SystemStatistics GetSystemStatistics()
        {
            try
            {
                var users = GetAllUsers();
                var claims = GetAllClaims();
                var approvals = GetAllApprovals();

                return new SystemStatistics
                {
                    TotalUsers = users.Count,
                    ActiveUsers = users.Count(u => u.IsActive),
                    TotalClaims = claims.Count,
                    ApprovedClaims = claims.Count(c => c.Status == ClaimStatus.Approved),
                    PendingClaims = claims.Count(c => c.Status == ClaimStatus.Submitted),
                    TotalApprovals = approvals.Count,
                    AverageClaimAmount = claims.Any() ? claims.Average(c => c.Amount) : 0,
                    SystemUptime = DateTime.Now - Process.GetCurrentProcess().StartTime,
                    DataSize = CalculateTotalDataSize(),
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate system statistics");
                return new SystemStatistics();
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

        /// <summary>
        /// Gets the next available ID with automated sequence management
        /// Part 3 requirement for reliable ID generation
        /// </summary>
        /// <param name="dataType">Type of data</param>
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
                    _ => Enumerable.Range(0, 1)
                };

                return ids.Any() ? ids.Max() + 1 : 1;
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
        /// </summary>
        private void InitializeSampleDataForAutomation()
        {
            var existingUsers = GetAllUsers();
            if (existingUsers.Count == 0)
            {
                // Create comprehensive sample data
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
                    }
                };

                foreach (var user in sampleUsers)
                {
                    SaveUser(user);
                }

                // Create sample lecturer details
                var lecturer = new Lecturer
                {
                    LecturerId = 2,
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

                _logger.LogInformation("Automated sample data initialization completed");
            }
        }

        #endregion
    }

    /// <summary>
    /// Data structure for automated analytics
    /// Part 3 requirement for system monitoring
    /// </summary>
    public class DataAnalytics
    {
        public string Date { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public int OperationCount { get; set; }
        public int RecordCount { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// Comprehensive system statistics
    /// Part 3 requirement for monitoring and reporting
    /// </summary>
    public class SystemStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalClaims { get; set; }
        public int ApprovedClaims { get; set; }
        public int PendingClaims { get; set; }
        public int TotalApprovals { get; set; }
        public decimal AverageClaimAmount { get; set; }
        public TimeSpan SystemUptime { get; set; }
        public long DataSize { get; set; }
        public DateTime GeneratedAt { get; set; } = DateTime.Now;
    }
}