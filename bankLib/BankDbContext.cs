using Microsoft.EntityFrameworkCore;

namespace bankLib
{
    public class BankDbContext : DbContext
    {
        public DbSet<Accounts> Accounts { get; set; }

        public DbSet<Admin> Admins { get; set; }

        public DbSet<BankTransaction> BankTransactions { get; set; }

        public DbSet<ChequeRequest> ChequeRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=MICHAEL-PC;Database=bankingProjectDB;Trusted_Connection=true;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Accounts>(entity =>
            {
                entity.HasKey(a => a.AccNo);

                entity.Property(a => a.AccNo).ValueGeneratedNever();
                entity.Property(a => a.AccName).HasMaxLength(100);
                entity.Property(a => a.AccType).HasMaxLength(20);
                entity.Property(a => a.AccBalance).HasColumnType("float");
                entity.Property(a => a.Username).HasMaxLength(50);
                entity.Property(a => a.Password).HasMaxLength(200);

                entity.HasIndex(a => a.Username).IsUnique();
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(a => a.AdminId);

                entity.Property(a => a.Username).HasMaxLength(50);
                entity.Property(a => a.Password).HasMaxLength(200);

                entity.HasIndex(a => a.Username).IsUnique();
            });

            modelBuilder.Entity<BankTransaction>(entity =>
            {
                entity.HasKey(t => t.TransactionId);

                entity.Property(t => t.TransactionType).HasMaxLength(20);
                entity.Property(t => t.Amount).HasColumnType("float");
                entity.Property(t => t.BalanceAfter).HasColumnType("float");

                entity.HasOne<Accounts>().WithMany().HasForeignKey(t => t.AccNo)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Accounts>().WithMany().HasForeignKey(t => t.RelatedAccNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChequeRequest>(entity =>
            {
                entity.HasKey(r => r.RequestId);

                entity.Property(r => r.Status).HasMaxLength(20);

                entity.HasOne<Accounts>().WithMany().HasForeignKey(r => r.AccNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
