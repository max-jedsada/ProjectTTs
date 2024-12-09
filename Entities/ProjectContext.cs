using Manager.Entities;
using Microsoft.EntityFrameworkCore;

namespace Project.Context;

public class ProjectContext : DbContext
{
    public ProjectContext(DbContextOptions<ProjectContext> option) : base(option)
    {
    }

    public DbSet<TimeSetting>? TimeSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


    }
}
