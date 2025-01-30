using System;


namespace BenefitPortalServices.Models
{
    public class AdminReimbursement
    {
        public int ReimbursementId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public string Status { get; set; } = "Pending"; // Default status
        public string ReimbursementType { get; set; }
        public DateTime Date { get; set; }

        public string BillResponse { get; set; }
    }
}
