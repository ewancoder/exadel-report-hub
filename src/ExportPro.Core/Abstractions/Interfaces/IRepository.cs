namespace ExportPro.Core.Abstractions.Interfaces
{
    public interface IRepository
    {
        Task AddAsync<T>(T entity) where T : class;
        Task AddMultipleAsync<T>(IEnumerable<T> entities) where T : class;
        Task DeleteAsync<T>(T entity) where T : class;
        Task DeleteAsync<T>(string id) where T : class;
        Task UpdateAsync<T>(T entity) where T : class;

    }
}
