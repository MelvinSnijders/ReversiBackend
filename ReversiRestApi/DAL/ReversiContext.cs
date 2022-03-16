using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;
using ReversiRestApi.Model;

namespace ReversiRestApi.DAL
{
    public class ReversiContext : DbContext
    {

        public ReversiContext(DbContextOptions<ReversiContext> options) : base(options) { }

        public DbSet<Game> Games { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new BoardConfiguration());
        }
    }

    public class BoardConfiguration : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.Property(e => e.Board).HasConversion(
            v => JsonConvert.SerializeObject(v),
            v => JsonConvert.DeserializeObject<Color[,]>(v));
        }
    }
}
