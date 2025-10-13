using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents a supporting document for a claim
    /// </summary>
    [Table("Documents")]
    public class Document
    {
        /// <summary>
        /// Gets or sets the unique document identifier
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the associated claim ID
        /// </summary>
        [Required(ErrorMessage = "Claim ID is required")]
        [ForeignKey("Claim")]
        public int ClaimId { get; set; }

        /// <summary>
        /// Gets or sets the original file name
        /// </summary>
        [Required(ErrorMessage = "File name is required")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        [Column("FileName")]
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the server file path
        /// </summary>
        [Required(ErrorMessage = "File path is required")]
        [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
        [Column("FilePath")]
        public string FilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file size in bytes
        /// </summary>
        [Required(ErrorMessage = "File size is required")]
        [Range(0, long.MaxValue, ErrorMessage = "File size must be positive")]
        [Column("FileSize")]
        public long FileSize { get; set; }

        /// <summary>
        /// Gets or sets the file type/extension
        /// </summary>
        [Required(ErrorMessage = "File type is required")]
        [StringLength(50, ErrorMessage = "File type cannot exceed 50 characters")]
        [Column("FileType")]
        public string FileType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the upload date
        /// </summary>
        [DataType(DataType.DateTime)]
        [Column("UploadDate")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets whether the document is active
        /// </summary>
        [Column("IsActive")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the associated claim
        /// </summary>
        public virtual Claim? Claim { get; set; }

        /// <summary>
        /// Gets the formatted file size for display
        /// </summary>
        [NotMapped]
        public string FormattedFileSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                double len = FileSize;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }
    }
}