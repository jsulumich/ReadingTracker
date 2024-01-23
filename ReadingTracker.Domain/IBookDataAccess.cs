namespace ReadingTracker.Domain
{
    public interface IBookDataAccess
    {
        public Task<IEnumerable<int>> GetDistinctYears();
        public Task<IEnumerable<IBook>> GetBooksForYear(int year);
        public Task<IBook> GetBookById(int? id);
        public Task<int> CreateBook(IBook book);
        public Task<int> EditBook(IBook book);
        public bool BookExists(int id);
        public Task<int> DeleteBook(int id);
        public Task<IEnumerable<IBook>> SearchForBooks(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword);
        Task<IEnumerable<IGenre>> GetGenresAsync();
        Task<IGenre> GetGenreById(int? id);
        IGenre GetEmptyGenre();
        Task<int> CreateGenre(IGenre genre);
        Task<int> EditGenre(IGenre genre);
        Task<int> DeleteGenre(int id);

    }

}
