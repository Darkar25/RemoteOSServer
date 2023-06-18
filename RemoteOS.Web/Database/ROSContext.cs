using Microsoft.EntityFrameworkCore;

namespace RemoteOS.Web.Database
{
    public class ROSContext : DbContext
    {
        public ROSContext(DbContextOptions<ROSContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<ComputerDBEntry> Computers { get; set; }
        public DbSet<IBlock> World { get; set; }
        public DbSet<ScannedBlock> ScannedWorld { get; set; }
        public DbSet<AnalyzedBlock> AnalyzedWorld { get; set; }
    }
}
