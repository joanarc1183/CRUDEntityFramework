using HospitalPatientManager.Data;
using HospitalPatientManager.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HospitalPatientManager.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly HospitalDbContext _context;

    public DbSet<T> _dbSet;

    public Repository(HospitalDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    async Task<List<T>> GetAllAsync()
    {
        await _dbSet.ToListAsync();
    }

    async Task<T?> GetByIdAsync(int id)
    {
        await _dbSet.FindAsync(id);
    }

    async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        await _dbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(T entity) 
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}