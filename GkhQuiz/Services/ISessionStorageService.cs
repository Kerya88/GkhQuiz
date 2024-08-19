namespace GkhQuiz.Services
{
    public interface ISessionStorageService
    {
        Task SetAsync<T>(string key, T item) where T : class;

        Task SetStringAsync(string key, string value);

        Task<T> GetAsync<T>(string key) where T : class;

        Task<string> GetStringAsync(string key);

        Task RemoveAsync(string key);

        Task Clear();
    }
}
