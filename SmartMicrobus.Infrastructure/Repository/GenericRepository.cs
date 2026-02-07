using Microsoft.EntityFrameworkCore;
using SmartMicrobus.Core.RepositoryContracts;
using SmartMicrobus.Infrastructure.Data;
using System.Linq.Expressions;

namespace SmartMicrobus.Infrastructure.Repository
{
    /// <inheritdoc/>
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<TModel> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = context.Set<TModel>();
        }
        /// <inheritdoc/>
        public async Task<TModel> AddAsync(TModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }
            await _db.AddAsync(entity);
            return entity;
        }

        /// <inheritdoc/>
        public async Task<int> CountAsync()
        {
            return await _db.CountAsync();
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }
            var entity = await _db.FindAsync(id);
            if (entity == null)
            {
                return false;
            }
            _db.Remove(entity);
            return true;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TModel>> GetAllAsync()
        {
            return await _db.AsNoTracking().ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<TModel>> GetAllAsync(params Expression<Func<TModel, object>>[] includes)
        {
            if (includes == null)
            {
                throw new ArgumentNullException(nameof(includes), "Includes cannot be null");
            }
            if (includes.Length == 0)
            {
                return await GetAllAsync();
            }
            IQueryable<TModel> query = _db;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<TModel?> GetByIdAsync(Guid id)
        {
            return await _db.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<TModel?> GetByIdAsync(Guid id, params Expression<Func<TModel, object>>[] includes)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(id));
            }
            if (includes == null)
            {
                throw new ArgumentNullException(nameof(includes), "Includes cannot be null");
            }
            if (includes.Length == 0)
            {
                return await GetByIdAsync(id);
            }
            IQueryable<TModel> query = _db;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == id);
        }

        /// <inheritdoc/>
        public async Task<TModel> UpdateAsync(TModel entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null");
            }
            _context.Entry(entity).State = EntityState.Modified;
            int res = await _context.SaveChangesAsync();
            return entity;
        }
    }
}
