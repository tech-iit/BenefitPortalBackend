


namespace BenefitPortalServices.Models
{
    public class Reimbursement
    {
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "Pending";
        public string ReimbursementType { get; set; }
    }
}