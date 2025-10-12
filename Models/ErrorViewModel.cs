namespace contract_monthly_claim_system_cs.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = string.Empty;

        public bool ShowRequestId
        {
            get
            {
                return !string.IsNullOrEmpty(RequestId);
            }
        }
    }
}