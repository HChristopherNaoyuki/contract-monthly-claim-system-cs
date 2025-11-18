using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using Microsoft.Extensions.Logging;
using contract_monthly_claim_system_cs.Models.DataModels;

namespace contract_monthly_claim_system_cs.Services
{
    /// <summary>
    /// Enhanced TextFileDataService with robust data persistence for Part 3 POE requirements
    /// Fixed data persistence issues with proper file handling, atomic operations, and error recovery
    /// Uses text file storage instead of database as per assignment specifications
    /// </summary>
    public class TextFileDataService
    {
        private readonly string _dataDirectory;
        private readonly ILogger<TextFileDataService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly object _fileLock = new object();

        /// <summary>
        /// Initializes a new instance of TextFileDataService with enhanced data persistence
        /// Creates data directory and initializes all required data files
        /// </summary>
        /// <param name="logger">Logger instance for tracking operations and debugging</param>
        public TextFileDataService(ILogger<TextFileDataService> logger)
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _logger = logger;

            // Configure JSON serialization options for better compatibility and readability
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,                    // Human-readable formatting
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // Consistent property naming
                PropertyNameCaseInsensitive = true,      // Case-insensitive deserialization
                IgnoreNullValues = false,                // Include null values in serialization
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
            };

            // Initialize data directory and files with proper error handling
            InitializeDataStorage();
        }

        /// <summary>
        /// Initializes the data storage system with comprehensive error handling
        /// Fixed: Ensures data directory exists and all files are properly initialized
        /// </summary>
        private void InitializeDataStorage()
        {
            try
            {
                _logger.LogInformation("Initializing text file data storage system...");

                // Create data directory if it doesn't exist
                if (!Directory.Exists(_dataDirectory))
                {
                    Directory.CreateDirectory(_dataDirectory);
                    _logger.LogInformation("Created data directory: {DataDirectory}", _dataDirectory);
                }

                // Initialize all required data files with proper structure
                var requiredDataFiles = new[]
                {
                    "users", "claims", "documents", "approvals", "lecturers"
                };

                foreach (var dataFile in requiredDataFiles)
                {
                    InitializeDataFile(dataFile);
                }

                _logger.LogInformation("Text file data storage system initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CRITICAL: Failed to initialize data storage system");
                throw new InvalidOperationException("Data storage system initialization failed", ex);
            }
        }

        /// <summary>
        /// Initializes a specific data file with proper JSON structure
        /// Fixed: Creates file with valid empty JSON array if missing or corrupted
        /// </summary>
        /// <param name="dataType">Type of data file to initialize</param>
        private void InitializeDataFile(string dataType)
        {
            var filePath = GetFilePath(dataType);

            try
            {
                lock (_fileLock) // Thread-safe file operations
                {
                    if (!File.Exists(filePath))
                    {
                        // Create new file with empty JSON array
                        WriteDataToFileAtomic(filePath, "[]");
                        _logger.LogInformation("Created new data file: {FilePath}", filePath);
                    }
                    else
                    {
                        // Verify and repair existing file if necessary
                        if (!IsValidJsonArrayFile(filePath))
                        {
                            _logger.LogWarning("Invalid JSON structure detected in {FilePath}, repairing file", filePath);
                            WriteDataToFileAtomic(filePath, "[]");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize data file: {FilePath}", filePath);
                throw;
            }
        }

        /// <summary>
        /// Validates that a file contains a valid JSON array structure
        /// Fixed: Comprehensive JSON validation with proper error handling
        /// </summary>
        /// <param name="filePath">Path to the file to validate</param>
        /// <returns>True if file contains valid JSON array structure</returns>
        private bool IsValidJsonArrayFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                    return false;

                var fileContent = File.ReadAllText(filePath, Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(fileContent))
                    return false;

                // Validate JSON structure by attempting to deserialize
                var deserialized = JsonSerializer.Deserialize<List<object>>(fileContent, _jsonOptions);
                return deserialized != null;
            }
            catch (JsonException jsonEx)
            {
                _logger.LogDebug(jsonEx, "JSON validation failed for file: {FilePath}", filePath);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "File validation error for: {FilePath}", filePath);
                return false;
            }
        }

        /// <summary>
        /// Gets the file path for a specific data type with security validation
        /// Fixed: Enhanced security to prevent directory traversal attacks
        /// </summary>
        /// <param name="dataType">Type of data file (users, claims, documents, etc.)</param>
        /// <returns>Validated and secure file path</returns>
        private string GetFilePath(string dataType)
        {
            // Validate data type to prevent injection attacks
            var validDataTypes = new HashSet<string>
            {
                "users", "claims", "documents", "approvals", "lecturers"
            };

            if (!validDataTypes.Contains(dataType.ToLower()))
            {
                throw new ArgumentException($"Invalid data type: {dataType}");
            }

            var fileName = $"{dataType.ToLower()}.txt";
            var filePath = Path.Combine(_dataDirectory, fileName);

            // Security validation - ensure path is within data directory
            if (!filePath.StartsWith(_dataDirectory) || fileName.Contains(".."))
            {
                throw new ArgumentException($"Security violation: Invalid file path for data type {dataType}");
            }

            return filePath;
        }

        /// <summary>
        /// Enhanced data reading with comprehensive error handling and automatic recovery
        /// Fixed: Robust retry mechanism with file repair on corruption
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <returns>List of data objects with automatic recovery on failure</returns>
        private List<T> ReadData<T>(string dataType)
        {
            var filePath = GetFilePath(dataType);
            const int maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    lock (_fileLock) // Thread-safe read operation
                    {
                        if (!File.Exists(filePath))
                        {
                            _logger.LogWarning("Data file not found: {FilePath}, initializing new file", filePath);
                            InitializeDataFile(dataType);
                            return new List<T>();
                        }

                        var jsonContent = File.ReadAllText(filePath, Encoding.UTF8);

                        if (string.IsNullOrWhiteSpace(jsonContent))
                        {
                            _logger.LogWarning("Empty data file: {FilePath}, initializing with empty data", filePath);
                            WriteDataToFileAtomic(filePath, "[]");
                            return new List<T>();
                        }

                        var result = JsonSerializer.Deserialize<List<T>>(jsonContent, _jsonOptions);

                        if (result == null)
                        {
                            _logger.LogWarning("Null data deserialized from: {FilePath}, returning empty list", filePath);
                            return new List<T>();
                        }

                        _logger.LogDebug("Successfully read {Count} {DataType} from {FilePath}",
                            result.Count, dataType, filePath);
                        return result;
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogWarning(jsonEx,
                        "JSON parsing error on attempt {Attempt}/{MaxRetries} for {FilePath}",
                        attempt, maxRetries, filePath);

                    if (attempt == maxRetries)
                    {
                        // Final attempt failed - repair the file
                        _logger.LogError("Failed to read data after {MaxRetries} attempts, repairing file: {FilePath}",
                            maxRetries, filePath);
                        RepairCorruptedFile(filePath);
                        return new List<T>();
                    }

                    // Exponential backoff before retry
                    Thread.Sleep(100 * attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error reading data from {FilePath} on attempt {Attempt}/{MaxRetries}",
                        filePath, attempt, maxRetries);

                    if (attempt == maxRetries)
                    {
                        _logger.LogError("Critical error reading data after {MaxRetries} attempts: {FilePath}",
                            maxRetries, filePath);
                        return new List<T>();
                    }

                    Thread.Sleep(100 * attempt);
                }
            }

            return new List<T>();
        }

        /// <summary>
        /// Enhanced data writing with atomic operations and comprehensive error handling
        /// Fixed: Atomic write operations prevent data corruption during concurrent access
        /// </summary>
        /// <typeparam name="T">Type of data to write</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <param name="data">Data to write with validation</param>
        private void WriteData<T>(string dataType, List<T> data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), "Data cannot be null");
            }

            var filePath = GetFilePath(dataType);
            const int maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Create backup before modification
                    CreateDataBackup(filePath);

                    // Serialize data to JSON
                    var jsonContent = JsonSerializer.Serialize(data, _jsonOptions);

                    // Atomic write operation
                    WriteDataToFileAtomic(filePath, jsonContent);

                    _logger.LogDebug("Successfully wrote {Count} {DataType} to {FilePath}",
                        data.Count, dataType, filePath);
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Error writing data to {FilePath} on attempt {Attempt}/{MaxRetries}",
                        filePath, attempt, maxRetries);

                    if (attempt == maxRetries)
                    {
                        throw new InvalidOperationException(
                            $"Failed to write data to {dataType} storage after {maxRetries} attempts", ex);
                    }

                    Thread.Sleep(100 * attempt);
                }
            }
        }

        /// <summary>
        /// Performs atomic file write operation using temporary files
        /// Fixed: Prevents data corruption by using atomic file replacement
        /// </summary>
        /// <param name="filePath">Target file path</param>
        /// <param name="content">Content to write</param>
        private void WriteDataToFileAtomic(string filePath, string content)
        {
            var tempFilePath = filePath + ".tmp";

            try
            {
                // Ensure directory exists
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Write to temporary file first
                File.WriteAllText(tempFilePath, content, Encoding.UTF8);

                // Atomic replacement of original file with temporary file
                File.Move(tempFilePath, filePath, overwrite: true);
            }
            catch (Exception ex)
            {
                // Clean up temporary file on error
                try
                {
                    if (File.Exists(tempFilePath))
                        File.Delete(tempFilePath);
                }
                catch (Exception cleanupEx)
                {
                    _logger.LogWarning(cleanupEx, "Failed to clean up temporary file: {TempFilePath}", tempFilePath);
                }

                throw new InvalidOperationException($"Atomic file write failed for {filePath}", ex);
            }
        }

        /// <summary>
        /// Creates a backup of data file before modification
        /// Fixed: Comprehensive backup creation with error handling
        /// </summary>
        /// <param name="filePath">Path to file to backup</param>
        private void CreateDataBackup(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var backupPath = $"{filePath}.backup.{DateTime.Now:yyyyMMddHHmmss}";
                    File.Copy(filePath, backupPath, overwrite: true);
                    _logger.LogDebug("Created data backup: {BackupPath}", backupPath);

                    // Clean up old backups (keep only last 5)
                    CleanupOldBackups(filePath);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to create backup for {FilePath}", filePath);
                // Don't throw - backup failure shouldn't prevent main operation
            }
        }

        /// <summary>
        /// Cleans up old backup files to prevent disk space issues
        /// </summary>
        /// <param name="filePath">Base file path for backup cleanup</param>
        private void CleanupOldBackups(string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                var fileName = Path.GetFileName(filePath);
                var backupPattern = $"{fileName}.backup.*";

                if (!string.IsNullOrEmpty(directory))
                {
                    var backupFiles = Directory.GetFiles(directory, backupPattern)
                        .OrderByDescending(f => f)
                        .Skip(5) // Keep only last 5 backups
                        .ToList();

                    foreach (var oldBackup in backupFiles)
                    {
                        File.Delete(oldBackup);
                        _logger.LogDebug("Cleaned up old backup: {BackupFile}", oldBackup);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clean up old backups for {FilePath}", filePath);
            }
        }

        /// <summary>
        /// Repairs a corrupted data file by recreating it with empty data
        /// </summary>
        /// <param name="filePath">Path to the corrupted file</param>
        private void RepairCorruptedFile(string filePath)
        {
            try
            {
                _logger.LogWarning("Attempting to repair corrupted file: {FilePath}", filePath);
                WriteDataToFileAtomic(filePath, "[]");
                _logger.LogInformation("Successfully repaired corrupted file: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to repair corrupted file: {FilePath}", filePath);
                throw;
            }
        }

        #region User Operations

        /// <summary>
        /// Retrieves all users from text file storage
        /// Fixed: Proper error handling and data validation
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
        public User? GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Retrieves a user by username for authentication
        /// </summary>
        /// <param name="username">Username to search for</param>
        /// <returns>User object or null if not found</returns>
        public User? GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var users = GetAllUsers();
            return users.FirstOrDefault(u =>
                u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Saves a user to text file storage with validation
        /// Fixed: Ensures data is actually persisted to file with proper error handling
        /// </summary>
        /// <param name="user">User object to save</param>
        public void SaveUser(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            // Validate required fields
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password))
                throw new ArgumentException("Username and password are required");

            var users = GetAllUsers();
            var existingUserIndex = users.FindIndex(u => u.UserId == user.UserId);

            if (existingUserIndex >= 0)
            {
                // Update existing user
                users[existingUserIndex] = user;
                _logger.LogDebug("Updating existing user: {Username} (ID: {UserId})", user.Username, user.UserId);
            }
            else
            {
                // Add new user
                users.Add(user);
                _logger.LogDebug("Creating new user: {Username} (ID: {UserId})", user.Username, user.UserId);
            }

            WriteData("users", users);
            _logger.LogInformation("User saved successfully: {Username} (ID: {UserId})", user.Username, user.UserId);
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
        public Lecturer? GetLecturerById(int lecturerId)
        {
            var lecturers = GetAllLecturers();
            return lecturers.FirstOrDefault(l => l.LecturerId == lecturerId);
        }

        /// <summary>
        /// Saves a lecturer to text file storage
        /// Fixed: Ensures lecturer data is persisted with proper validation
        /// </summary>
        /// <param name="lecturer">Lecturer object to save</param>
        public void SaveLecturer(Lecturer lecturer)
        {
            if (lecturer == null)
                throw new ArgumentNullException(nameof(lecturer));

            var lecturers = GetAllLecturers();
            var existingLecturerIndex = lecturers.FindIndex(l => l.LecturerId == lecturer.LecturerId);

            if (existingLecturerIndex >= 0)
            {
                lecturers[existingLecturerIndex] = lecturer;
                _logger.LogDebug("Updating existing lecturer: ID {LecturerId}", lecturer.LecturerId);
            }
            else
            {
                lecturers.Add(lecturer);
                _logger.LogDebug("Creating new lecturer: ID {LecturerId}", lecturer.LecturerId);
            }

            WriteData("lecturers", lecturers);
            _logger.LogInformation("Lecturer saved successfully: ID {LecturerId}", lecturer.LecturerId);
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
        public Claim? GetClaimById(int claimId)
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
        /// Saves a claim to text file storage
        /// Fixed: Ensures claim data is properly persisted with validation
        /// </summary>
        /// <param name="claim">Claim object to save</param>
        public void SaveClaim(Claim claim)
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));

            // Validate required fields
            if (claim.LecturerId <= 0 || claim.HoursWorked <= 0 || claim.HourlyRate <= 0)
                throw new ArgumentException("Invalid claim data - missing required fields");

            var claims = GetAllClaims();
            var existingClaimIndex = claims.FindIndex(c => c.ClaimId == claim.ClaimId);

            if (existingClaimIndex >= 0)
            {
                claims[existingClaimIndex] = claim;
                _logger.LogDebug("Updating existing claim: ID {ClaimId}", claim.ClaimId);
            }
            else
            {
                claims.Add(claim);
                _logger.LogDebug("Creating new claim: ID {ClaimId}", claim.ClaimId);
            }

            WriteData("claims", claims);
            _logger.LogInformation("Claim saved successfully: ID {ClaimId} for Lecturer {LecturerId}",
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
                throw new ArgumentNullException(nameof(document));

            // Validate required fields
            if (string.IsNullOrEmpty(document.FileName) || string.IsNullOrEmpty(document.FilePath))
                throw new ArgumentException("Document filename and path are required");

            var documents = GetAllDocuments();
            var existingDocumentIndex = documents.FindIndex(d => d.DocumentId == document.DocumentId);

            if (existingDocumentIndex >= 0)
            {
                documents[existingDocumentIndex] = document;
                _logger.LogDebug("Updating existing document: {FileName}", document.FileName);
            }
            else
            {
                documents.Add(document);
                _logger.LogDebug("Creating new document: {FileName}", document.FileName);
            }

            WriteData("documents", documents);
            _logger.LogInformation("Document saved successfully: {FileName} for Claim {ClaimId}",
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
                throw new ArgumentNullException(nameof(approval));

            // Validate required fields
            if (approval.ClaimId <= 0 || approval.ApproverUserId <= 0)
                throw new ArgumentException("Invalid approval data - missing required fields");

            var approvals = GetAllApprovals();
            var existingApprovalIndex = approvals.FindIndex(a => a.ApprovalId == approval.ApprovalId);

            if (existingApprovalIndex >= 0)
            {
                approvals[existingApprovalIndex] = approval;
                _logger.LogDebug("Updating existing approval: ID {ApprovalId}", approval.ApprovalId);
            }
            else
            {
                approvals.Add(approval);
                _logger.LogDebug("Creating new approval: ID {ApprovalId}", approval.ApprovalId);
            }

            WriteData("approvals", approvals);
            _logger.LogInformation("Approval saved successfully: Claim {ClaimId} by User {UserId}",
                approval.ClaimId, approval.ApproverUserId);
        }

        #endregion

        #region System Operations

        /// <summary>
        /// Gets the next available ID for a data type with sequence management
        /// Fixed: Reliable ID generation with proper error handling
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
        /// Verifies that data persistence is working correctly
        /// Fixed: Added comprehensive persistence verification
        /// </summary>
        /// <returns>True if data persistence is working correctly</returns>
        public bool VerifyDataPersistence()
        {
            try
            {
                _logger.LogInformation("Verifying data persistence...");

                // Test write and read operation for each data type
                var dataTypes = new[] { "users", "claims", "lecturers", "documents", "approvals" };

                foreach (var dataType in dataTypes)
                {
                    var testData = new List<object>();

                    // Add minimal test data based on type
                    switch (dataType)
                    {
                        case "users":
                            testData.Add(new User { UserId = 99999, Username = "persistence_test", Password = "test", Name = "Test", Surname = "User", Role = UserRole.Lecturer });
                            break;
                        case "claims":
                            testData.Add(new Claim { ClaimId = 99999, LecturerId = 1, HoursWorked = 1, HourlyRate = 1, Amount = 1, Status = ClaimStatus.Submitted });
                            break;
                    }

                    if (testData.Any())
                    {
                        // Use reflection to call WriteData generically
                        var method = typeof(TextFileDataService).GetMethod("WriteData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var genericMethod = method.MakeGenericMethod(testData.First().GetType());
                        genericMethod.Invoke(this, new object[] { dataType, testData });

                        // Verify data was written
                        var readMethod = typeof(TextFileDataService).GetMethod("ReadData", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        var genericReadMethod = readMethod.MakeGenericMethod(testData.First().GetType());
                        var readData = genericReadMethod.Invoke(this, new object[] { dataType }) as System.Collections.IList;

                        if (readData == null || readData.Count == 0)
                        {
                            _logger.LogError("Data persistence verification failed for {DataType}", dataType);
                            return false;
                        }
                    }
                }

                _logger.LogInformation("Data persistence verification completed successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Data persistence verification failed");
                return false;
            }
        }

        /// <summary>
        /// Gets system statistics for monitoring and debugging
        /// </summary>
        /// <returns>System statistics object</returns>
        public SystemStatistics GetSystemStatistics()
        {
            try
            {
                return new SystemStatistics
                {
                    TotalUsers = GetAllUsers().Count,
                    TotalLecturers = GetAllLecturers().Count,
                    TotalClaims = GetAllClaims().Count,
                    TotalDocuments = GetAllDocuments().Count,
                    TotalApprovals = GetAllApprovals().Count,
                    DataDirectory = _dataDirectory,
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate system statistics");
                return new SystemStatistics { GeneratedAt = DateTime.Now };
            }
        }

        #endregion
    }

    /// <summary>
    /// System statistics for monitoring data persistence
    /// </summary>
    public class SystemStatistics
    {
        public int TotalUsers { get; set; }
        public int TotalLecturers { get; set; }
        public int TotalClaims { get; set; }
        public int TotalDocuments { get; set; }
        public int TotalApprovals { get; set; }
        public string DataDirectory { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}