using System.ComponentModel;

namespace ReadingTracker.Domain
{
    public class Search
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set;}        
        public string? Keyword { get; set; }       
    }
}
