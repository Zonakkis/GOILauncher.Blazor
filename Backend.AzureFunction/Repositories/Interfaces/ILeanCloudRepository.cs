namespace Backend.Reposties.Interfaces
{
    internal interface ILeanCloudRepository<T>
    {
        Task<T?> GetAsync(string id);
        Task<List<T>> ListAsync();
    }
}
