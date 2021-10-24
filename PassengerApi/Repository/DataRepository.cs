using System.Threading.Tasks;

namespace PassengerApi.Repository
{
    public class DataRepository<T> : IDataRepository<T> where T : class
    {
        private readonly PassengerApiContext _context;
        public DataRepository(PassengerApiContext context)
        {
            _context = context;
        }
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public async Task<T> SaveAsync(T entity)
        {
            await _context.SaveChangesAsync();
            return entity;
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
