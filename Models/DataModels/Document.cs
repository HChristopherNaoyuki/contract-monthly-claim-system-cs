using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents a supporting document for a claim with enhanced properties
    /// </summary>
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey("Claim")]
        public int ClaimId { get; set; }

        public virtual Claim? Claim { get; set; }

        [Required(ErrorMessage = "File name is required")]
        [Display(Name = "File Name")]
        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters")]
        public string FileName { get; set; } = string.Empty;

        [Required(ErrorMessage = "File path is required")]
        [Display(Name = "File Path")]
        [StringLength(500, ErrorMessage = "File path cannot exceed 500 characters")]
        public string FilePath { get; set; } = string.Empty;

        [Display(Name = "File Size (bytes)")]
        public long FileSize { get; set; }

        [Display(Name = "File Type")]
        [StringLength(50, ErrorMessage = "File type cannot exceed 50 characters")]
        public string FileType { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets the file size in a human-readable format
        /// </summary>
        [Display(Name = "File Size")]
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