using Microsoft.EntityFrameworkCore;

using Tasker.App.Models;

namespace Tasker.App.DbContext;

public class AccountDb : Microsoft.EntityFrameworkCore.DbContext {
    public AccountDb(DbContextOptions options) : base(options) { }
    
    public DbSet<AccountRecord> Accounts { get; set; }
}