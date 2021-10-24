using Microsoft.EntityFrameworkCore;
using PassengerApi.Model;

namespace PassengerApi.Repository
{
    public class PassengerApiContext : DbContext
    {
        public PassengerApiContext(DbContextOptions<PassengerApiContext> options) : base(options) { }
        public DbSet<Passenger> Passenger { get; set; }
    }
}
