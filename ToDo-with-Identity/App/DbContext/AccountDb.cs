using Microsoft.EntityFrameworkCore;

using Tasker.App.Models;

namespace Tasker.App.DbContext;

public class AccountDb(DbContextOptions options) : Microsoft.EntityFrameworkCore.DbContext(options) {
    public DbSet<AccountRecord> Accounts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<AccountRecord>().Property(e => e.Email).IsRequired();
        modelBuilder.Entity<AccountRecord>().Property(e => e.Username).IsRequired();
        modelBuilder.Entity<AccountRecord>().Property(e => e.Password).IsRequired();

        modelBuilder.Entity<AccountRecord>().HasIndex(e => e.Email).IsUnique();
        modelBuilder.Entity<AccountRecord>().HasIndex(e => e.Username).IsUnique();
    }
}