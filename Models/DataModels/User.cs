using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents a system user with authentication credentials and role-based access
    /// Part 3 POE requirement: Enhanced role system with Human Resource role
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's first name
        /// </summary>
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters")]
        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's last name
        /// </summary>
        [Required(ErrorMessage = "Surname is required")]
        [StringLength(50, ErrorMessage = "Surname cannot exceed 50 characters")]
        [Column("Surname")]
        public string Surname { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique username for authentication
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, ErrorMessage = "Username cannot exceed 20 characters")]
        [Column("Username")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hashed password for authentication
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, ErrorMessage = "Password cannot exceed 255 characters")]
        [Column("Password")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's role in the system
        /// Part 3 POE requirement: Added Human Resource role
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        [Column("Role")]
        public UserRole Role { get; set; }

        /// <summary>
        /// Gets or sets the user's email address
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters")]
        [Column("Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's phone number
        /// </summary>
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Column("PhoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the user account is active
        /// </summary>
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the date when the user was created
        /// </summary>
        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date of the user's last login
        /// </summary>
        [Column("LastLoginDate")]
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Gets the user's full name
        /// </summary>
        [NotMapped]
        public string FullName
        {
            get
            {
                return $"{Name} {Surname}";
            }
        }
    }

    /// <summary>
    /// Defines the possible roles for system users
    /// Part 3 POE requirement: Enhanced role system with Human Resource role
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Can submit claims and track status
        /// </summary>
        Lecturer = 0,

        /// <summary>
        /// Can review and approve/reject claims
        /// </summary>
        ProgrammeCoordinator = 1,

        /// <summary>
        /// Has full system access and final approval authority
        /// </summary>
        AcademicManager = 2,

        /// <summary>
        /// Part 3 POE requirement: Human Resource role for HR operations
        /// Can access HR analytics and manage lecturer data
        /// </summary>
        HumanResource = 3
    }
}