namespace RoboConsulta.Data
{
    public interface ILocalRepository<T> where T : class
    {
        Task<int> InsertAsync(T entity);
    }
}
