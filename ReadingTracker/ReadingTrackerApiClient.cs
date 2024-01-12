using ReadingTracker.Domain;
using System.Net.Http;

public interface IReadingTrackerApiClient
{
    public Task<IEnumerable<Book>> GetBooksForYear(int year);
}

public class ReadingTrackerApiClient : IReadingTrackerApiClient //: IBookDataAccess
{
    private readonly HttpClient _httpClient;

    public ReadingTrackerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        // set address in one place, instead of each usage of the client
        // this could be done here or in the IOC container delegate
        // address hard-coded, should be from configuration
        httpClient.BaseAddress = new Uri("https://localhost:7014");
    }

    public bool BookExists(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> CreateBook(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<int> DeleteBook(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> EditBook(Book book)
    {
        throw new NotImplementedException();
    }

    public Task<Book> GetBookById(int? id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Book>> GetBooksForYear(int year)
    {
        // implementation of the other methods is an exercise for the reader
        Uri uri = new Uri(_httpClient.BaseAddress, $"/api/books?year={year}");
        return await _httpClient.GetFromJsonAsync<IEnumerable<Book>>(uri);
    }

    public Task<IEnumerable<int>> GetDistinctYears()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Book>> SearchForBooks(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword)
    {
        throw new NotImplementedException();
    }
}