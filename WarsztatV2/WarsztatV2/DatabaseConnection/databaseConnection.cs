using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Conventions;
using WarsztatV2.Tables;

namespace WarsztatV2
{
    internal class databaseConnection : DbContext
    {
        public DbSet<Adres> Adresy { get; set; }
        public DbSet<Warsztat> Warsztaty { get; set; }
        public DbSet<Pracownik> Pracownicy { get; set; }
        public DbSet<Pojazd> Pojazdy { get; set; }
        public DbSet<Klient> Klienci { get; set; }
        public DbSet<Faktura> Faktury { get; set; }
        public DbSet<Naprawa> Naprawy { get; set; }
        //public DbSet<Czesc> Czesci { get; set; }
        //public DbSet<Uzyte_czesci> Uzyteczesci { get; set; }
        //public databaseConnection() : base(nameOrConnectionString: "databaseConnection") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Warsztat>().ToTable("warsztat", "public");
            modelBuilder.Entity<Adres>().ToTable("adres", "public");
            modelBuilder.Entity<Pracownik>().ToTable("pracownik", "public");
            modelBuilder.Entity<Pojazd>().ToTable("pojazd", "public");
            modelBuilder.Entity<Klient>().ToTable("klient", "public");
            modelBuilder.Entity<Faktura>().ToTable("faktura", "public");
            modelBuilder.Entity<Naprawa>().ToTable("naprawa", "public");
            //modelBuilder.Entity<Czesc>().ToTable("czesci", "public");
            //modelBuilder.Entity<Uzyte_czesci>().ToTable("uzyte_czesci", "public");

            modelBuilder.Conventions.Add<StoreGeneratedIdentityKeyConvention>();
        }
    }
}
