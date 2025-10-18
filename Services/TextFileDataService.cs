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
    /// Service for managing data storage using text files
    /// Replaces database functionality for prototype phase
    /// </summary>
    public class TextFileDataService
    {
        private readonly string _dataDirectory;
        private readonly ILogger<TextFileDataService> _logger;

        /// <summary>
        /// Initializes a new instance of TextFileDataService
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public TextFileDataService(ILogger<TextFileDataService> logger)
        {
            _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
            _logger = logger;

            if (!Directory.Exists(_dataDirectory))
            {
                Directory.CreateDirectory(_dataDirectory);
                _logger.LogInformation("Created Data directory: {DataDirectory}", _dataDirectory);
            }
        }

        /// <summary>
        /// Gets the file path for a specific data type
        /// </summary>
        /// <param name="dataType">Type of data (users, claims, etc.)</param>
        /// <returns>Full file path</returns>
        private string GetFilePath(string dataType)
        {
            return Path.Combine(_dataDirectory, $"{dataType}.txt");
        }

        /// <summary>
        /// Reads all data from a file
        /// </summary>
        /// <typeparam name="T">Type of data to read</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <returns>List of data objects</returns>
        private List<T> ReadData<T>(string dataType)
        {
            var filePath = GetFilePath(dataType);
            try
            {
                if (File.Exists(filePath))
                {
                    var json = File.ReadAllText(filePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var result = JsonSerializer.Deserialize<List<T>>(json);
                        return result ?? new List<T>();
                    }
                }
                else
                {
                    _logger.LogInformation("Data file not found, creating empty file: {FilePath}", filePath);
                    File.WriteAllText(filePath, "[]");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading data from {FilePath}", filePath);
            }

            return new List<T>();
        }

        /// <summary>
        /// Writes data to a file
        /// </summary>
        /// <typeparam name="T">Type of data to write</typeparam>
        /// <param name="dataType">Type of data file</param>
        /// <param name="data">Data to write</param>
        private void WriteData<T>(string dataType, List<T> data)
        {
            var filePath = GetFilePath(dataType);
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(filePath, json);
                _logger.LogDebug("Successfully wrote {Count} items to {FilePath}", data.Count, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing data to {FilePath}", filePath);
                throw;
            }
        }

        // User operations
        public List<User> GetAllUsers()
        {
            return ReadData<User>("users");
        }

        public User? GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        public User? GetUserByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

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
            _logger.LogInformation("User saved: {Username} (ID: {UserId})", user.Username, user.UserId);
        }

        // Lecturer operations
        public List<Lecturer> GetAllLecturers()
        {
            return ReadData<Lecturer>("lecturers");
        }

        public Lecturer? GetLecturerById(int lecturerId)
        {
            var lecturers = GetAllLecturers();
            return lecturers.FirstOrDefault(l => l.LecturerId == lecturerId);
        }

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
        }

        // Claim operations
        public List<Claim> GetAllClaims()
        {
            return ReadData<Claim>("claims");
        }

        public Claim? GetClaimById(int claimId)
        {
            var claims = GetAllClaims();
            return claims.FirstOrDefault(c => c.ClaimId == claimId);
        }

        public List<Claim> GetClaimsByLecturerId(int lecturerId)
        {
            var claims = GetAllClaims();
            return claims.Where(c => c.LecturerId == lecturerId).ToList();
        }

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
            _logger.LogInformation("Claim saved: ID {ClaimId} for Lecturer {LecturerId}", claim.ClaimId, claim.LecturerId);
        }

        // Document operations
        public List<Document> GetAllDocuments()
        {
            return ReadData<Document>("documents");
        }

        public List<Document> GetDocumentsByClaimId(int claimId)
        {
            var documents = GetAllDocuments();
            return documents.Where(d => d.ClaimId == claimId).ToList();
        }

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
        }

        // Approval operations
        public List<Approval> GetAllApprovals()
        {
            return ReadData<Approval>("approvals");
        }

        public List<Approval> GetApprovalsByClaimId(int claimId)
        {
            var approvals = GetAllApprovals();
            return approvals.Where(a => a.ClaimId == claimId).ToList();
        }

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
        }

        /// <summary>
        /// Gets the next available ID for a data type
        /// </summary>
        /// <param name="dataType">Type of data</param>
        /// <returns>Next available ID</returns>
        public int GetNextId(string dataType)
        {
            return dataType.ToLower() switch
            {
                "users" => GetAllUsers().Count > 0 ? GetAllUsers().Max(u => u.UserId) + 1 : 1,
                "claims" => GetAllClaims().Count > 0 ? GetAllClaims().Max(c => c.ClaimId) + 1 : 1,
                "documents" => GetAllDocuments().Count > 0 ? GetAllDocuments().Max(d => d.DocumentId) + 1 : 1,
                "approvals" => GetAllApprovals().Count > 0 ? GetAllApprovals().Max(a => a.ApprovalId) + 1 : 1,
                "lecturers" => GetAllLecturers().Count > 0 ? GetAllLecturers().Max(l => l.LecturerId) + 1 : 1,
                _ => 1
            };
        }

        /// <summary>
        /// Initializes sample data for the application
        /// Creates default users and sample data if no data exists
        /// </summary>
        public void InitializeSampleData()
        {
            try
            {
                var existingUsers = GetAllUsers();
                if (!existingUsers.Any())
                {
                    _logger.LogInformation("Creating sample data...");

                    // Create sample users
                    var sampleUsers = new List<User>
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

                    // Create sample lecturer
                    var lecturer = new Lecturer
                    {
                        LecturerId = 2,
                        EmployeeNumber = "EMP001",
                        Department = "Computer Science",
                        HourlyRate = 150.00m,
                        ContractStartDate = DateTime.Now.AddYears(-1),
                        ContractEndDate = DateTime.Now.AddYears(1)
                    };

                    SaveLecturer(lecturer);

                    // Create sample claim
                    var sampleClaim = new Claim
                    {
                        ClaimId = 1,
                        LecturerId = 2,
                        MonthYear = DateTime.Now.ToString("yyyy-MM"),
                        HoursWorked = 40,
                        HourlyRate = 150.00m,
                        Amount = 6000.00m,
                        Status = ClaimStatus.Submitted,
                        SubmissionComments = "Sample claim for testing purposes",
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };

                    SaveClaim(sampleClaim);

                    _logger.LogInformation("Sample data created successfully");
                }
                else
                {
                    _logger.LogInformation("Sample data already exists - {UserCount} users found", existingUsers.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing sample data");
            }
        }
    }
}