using Microsoft.EntityFrameworkCore;

namespace AppPraticaIdioma.Models{
    public class AppDbContext : DbContext
    {
        public DbSet<Phrase> Phrases{ get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=banco.db");
        }


    }
}