using System.Data.Entity;
using WarsztatV2.Tables;

namespace WarsztatV2
{
    internal class databaseConnection : DbContext
    {
        public DbSet<Adres> Adresy { set; get; }
        public DbSet<Warsztat> Warsztat { set; get; }
        public databaseConnection() : base(nameOrConnectionString: "databaseConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            base.OnModelCreating(modelBuilder);
        }
    }
}
