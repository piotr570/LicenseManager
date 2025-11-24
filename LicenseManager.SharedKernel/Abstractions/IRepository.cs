namespace LicenseManager.SharedKernel.Abstractions;

public interface IRepository<TEntity>
{
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
}