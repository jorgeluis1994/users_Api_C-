using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; } // Cambié a Customers para seguir la convención
    public DbSet<Account> Accounts { get; set; } // Cambié a Accounts para seguir la convención

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración para Customer
        modelBuilder.Entity<Customer>()
            .ToTable("Customers")
            .HasKey(c => c.Id);

        modelBuilder.Entity<Customer>()
            .Property(c => c.FirstName) // Asegúrate de que las propiedades coincidan
            .IsRequired()
            .HasMaxLength(50);

        modelBuilder.Entity<Customer>()
            .Property(c => c.LastName) // Asegúrate de que las propiedades coincidan
            .IsRequired()
            .HasMaxLength(50);

        // Configuración para Account
        modelBuilder.Entity<Account>()
            .ToTable("Accounts")
            .HasKey(a => a.Id);

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountNumber)
            .IsRequired()
            .HasMaxLength(20);

        modelBuilder.Entity<Account>()
            .Property(a => a.Balance)
            .HasColumnType("decimal(18,2)");

        // Configuración de relaciones
        modelBuilder.Entity<Account>()
            .HasOne(a => a.Customer)
            .WithMany(c => c.Accounts)
            .HasForeignKey(a => a.CustomerId);
    }
}

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } // Asegúrate de que coincida con la configuración
    public string LastName { get; set; }  // Asegúrate de que coincida con la configuración

    public ICollection<Account> Accounts { get; set; } // Relación uno a muchos
}

public class Account
{
    public int Id { get; set; }
    public string AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } // Relación con la entidad Customer
}
