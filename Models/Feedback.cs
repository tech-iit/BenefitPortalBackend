


namespace BenefitPortalServices.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        public int BenefitId { get; set; }
        public int EmployeeId { get; set; }
        public string FeedbackText { get; set; }
        public string Suggestion { get; set; }
    }
}
