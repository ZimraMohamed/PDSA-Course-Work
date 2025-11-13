using Microsoft.EntityFrameworkCore;

namespace PDSA.API.Data
{
    public class PDSADbContext : DbContext
    {
        public PDSADbContext(DbContextOptions<PDSADbContext> options) : base(options)
        {
        }

        // For now, we'll keep this simple and add game persistence later
        // The TSP game functionality works in-memory

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Future: Add TSP game result persistence models here
        }
    }
}