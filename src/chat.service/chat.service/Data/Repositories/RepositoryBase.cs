using Microsoft.EntityFrameworkCore;

namespace chat.service.Data.Repositories;

public abstract class RepositoryBase<TContext>(TContext context)
    where TContext : DbContext
{
    protected readonly TContext _context = context;

    public Task<int> SaveChanges() => _context.SaveChangesAsync();
    public Task<int> SaveChanges(CancellationToken cancellationToken) => _context.SaveChangesAsync(cancellationToken);

    public void ClearTracker() => _context.ChangeTracker.Clear();
}