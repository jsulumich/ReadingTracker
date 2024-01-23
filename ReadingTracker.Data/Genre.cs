using ReadingTracker.Domain;

namespace ReadingTracker.Data
{
    public class Genre : IGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
