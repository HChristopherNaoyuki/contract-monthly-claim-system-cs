// Models/DataModels/Document.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace contract_monthly_claim_system_cs.Models.DataModels
{
    /// <summary>
    /// Represents a supporting document for a claim
    /// </summary>
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [ForeignKey("Claim")]
        public int ClaimId { get; set; }
        public virtual Claim Claim { get; set; }

        [Required(ErrorMessage = "File name is required")]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Required(ErrorMessage = "File path is required")]
        [Display(Name = "File Path")]
        public string FilePath { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; } = DateTime.Now;
    }
}