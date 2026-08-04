namespace Application.Interfaces.General;

public interface IGenericRepository<T> where T : class
{
    Task<IReadOnlyList<T>> GetAllAsync(FormattableString spc);
}
