using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace contract_monthly_claim_system_cs
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
                        return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                    }
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error writing data to {FilePath}", filePath);
                throw;
            }
        }

        // User operations
        public List<Models.DataModels.User> GetAllUsers()
        {
            return ReadData<Models.DataModels.User>("users");
        }

        public Models.DataModels.User? GetUserById(int userId)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.UserId == userId);
        }

        public Models.DataModels.User? GetUserByUsername(string username)
        {
            var users = GetAllUsers();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public void SaveUser(Models.DataModels.User user)
        {
            var users = GetAllUsers();
            var existingUser = users.FirstOrDefault(u => u.UserId == user.UserId);

            if (existingUser != null)
            {
                users.Remove(existingUser);
            }

            users.Add(user);
            WriteData("users", users);
        }

        // Lecturer operations
        public List<Models.DataModels.Lecturer> GetAllLecturers()
        {
            return ReadData<Models.DataModels.Lecturer>("lecturers");
        }

        public Models.DataModels.Lecturer? GetLecturerById(int lecturerId)
        {
            var lecturers = GetAllLecturers();
            return lecturers.FirstOrDefault(l => l.LecturerId == lecturerId);
        }

        public void SaveLecturer(Models.DataModels.Lecturer lecturer)
        {
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
        public List<Models.DataModels.Claim> GetAllClaims()
        {
            return ReadData<Models.DataModels.Claim>("claims");
        }

        public Models.DataModels.Claim? GetClaimById(int claimId)
        {
            var claims = GetAllClaims();
            return claims.FirstOrDefault(c => c.ClaimId == claimId);
        }

        public List<Models.DataModels.Claim> GetClaimsByLecturerId(int lecturerId)
        {
            var claims = GetAllClaims();
            return claims.Where(c => c.LecturerId == lecturerId).ToList();
        }

        public void SaveClaim(Models.DataModels.Claim claim)
        {
            var claims = GetAllClaims();
            var existingClaim = claims.FirstOrDefault(c => c.ClaimId == claim.ClaimId);

            if (existingClaim != null)
            {
                claims.Remove(existingClaim);
            }

            claims.Add(claim);
            WriteData("claims", claims);
        }

        // Document operations
        public List<Models.DataModels.Document> GetAllDocuments()
        {
            return ReadData<Models.DataModels.Document>("documents");
        }

        public List<Models.DataModels.Document> GetDocumentsByClaimId(int claimId)
        {
            var documents = GetAllDocuments();
            return documents.Where(d => d.ClaimId == claimId).ToList();
        }

        public void SaveDocument(Models.DataModels.Document document)
        {
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
        public List<Models.DataModels.Approval> GetAllApprovals()
        {
            return ReadData<Models.DataModels.Approval>("approvals");
        }

        public List<Models.DataModels.Approval> GetApprovalsByClaimId(int claimId)
        {
            var approvals = GetAllApprovals();
            return approvals.Where(a => a.ClaimId == claimId).ToList();
        }

        public void SaveApproval(Models.DataModels.Approval approval)
        {
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
    }
}