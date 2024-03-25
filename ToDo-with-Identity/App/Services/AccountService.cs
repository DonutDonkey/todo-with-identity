using Microsoft.EntityFrameworkCore;

using Tasker.App.Models;
using Tasker.App.DbContext;
using Tasker.App.Controllers;

namespace Tasker.App.Services;

public class AccountService(Logger logger) : IEndpoint {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("account"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", () => false);
        grp.MapGet("/accounts", async (AccountDb db) => await db.Accounts.ToListAsync());

        grp.MapPost("/", async (AccountDb db, string? identity) =>
            await db.Accounts.FirstOrDefaultAsync(acc => acc.Identity == identity)
        );

        grp.MapPost("/register", async (AccountDb db, AccountRecord account) => {
            logger.Log<AccountService>($"Adding account : {account}");

            if (await db.Accounts.FirstOrDefaultAsync(e => e.Email == account.Email) is not null)
                return Results.Conflict("Account already exists");

            await db.Accounts.AddAsync(account);
            await db.SaveChangesAsync();

            return Results.Created();
        });
    }
}