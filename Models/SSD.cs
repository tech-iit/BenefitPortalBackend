using System;



namespace BenefitPortalServices.Models
{
    public class SSD
    {
        public int SSDId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
    }
}
