using Microsoft.EntityFrameworkCore;

public class PlanItContext : DbContext
{
    public DbSet<Utilisateur> Utilisateurs { get; set; } = null!;
    public DbSet<Tache> Taches { get; set; } = null!;
    public DbSet<Todo> Todos { get; set; } = null!;
    public string DbPath { get; private set; }

    public PlanItContext()
    {
        // Path to SQLite database file
        DbPath = "PlanIt.db";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disponibilite>()
        .HasOne(d => d.Utilisateur)
        .WithMany(u => u.Disponibilites)
        .HasForeignKey(d => d.UtilisateurId)
        .IsRequired();
    }


    // The following configures EF to create a SQLite database file locally
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // Use SQLite as database
        options.UseSqlite($"Data Source={DbPath}");
        // Optional: log SQL queries to console
        //options.LogTo(Console.WriteLine, new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
    }
}