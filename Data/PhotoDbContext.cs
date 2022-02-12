using Microsoft.EntityFrameworkCore;
using PhotoAPI.Models;

namespace PhotoAPI.Data
{
    public class PhotoDbContext : DbContext
    {
        public PhotoDbContext(DbContextOptions<PhotoDbContext> options) : base(options)
        {
        }

        public DbSet<Photo> Photos { get; set; }
    }
}
