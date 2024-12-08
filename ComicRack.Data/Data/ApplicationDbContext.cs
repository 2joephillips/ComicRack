using ComicRack.Core;
using Microsoft.EntityFrameworkCore;

namespace ComicRack.Data.Data;

public partial class ApplicationDbContext : DbContext
{
    public DbSet<Comic> Comics { get; set; }
    public DbSet<Setting> Settings { get; set; }

    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
