using Application.Interfaces;
using Application.Interfaces.General;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class GenericRepository<T>(ECommerceDbContext context) : IGenericRepository<T> where T : class
{
    public async Task<IReadOnlyList<T>> GetAllAsync(FormattableString spc)
    {
        return await context.Set<T>()
            .FromSqlInterpolated(spc)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<int> AddAsync(FormattableString sqlQuery)
    {
        var result = await context.Database.SqlQuery<int>(sqlQuery).ToListAsync();
        await context.SaveChangesAsync();
        return result.FirstOrDefault();

    }
}