// Models/ClaimViewModels/DocumentViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ClaimViewModels
{
    /// <summary>
    /// ViewModel for document display
    /// </summary>
    public class DocumentViewModel
    {
        public int DocumentId { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; }
    }
}