using System.ComponentModel.DataAnnotations;

namespace contract_monthly_claim_system_cs.Models.ViewModels
{
    /// <summary>
    /// ViewModel for user login
    /// </summary>
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}