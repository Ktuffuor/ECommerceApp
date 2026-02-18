using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ECommerceDbContext _context;

    public GenericRepository(ECommerceDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<T>> GetAllAsync(FormattableString spc)
    {
        return await _context.Set<T>()
            .FromSqlInterpolated(spc)
            .AsNoTracking()
            .ToListAsync();
    }
}