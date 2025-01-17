


namespace BenefitPortalServices.Models
{
        public class Benefit
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImagePath { get; set; }
            public int MinEligibilityCriteria { get; set; }
            public string Category { get; set; }
            public int AdminId { get; set; }
            public bool IsBookmarked { get; set; }  // Add this property
        }
}