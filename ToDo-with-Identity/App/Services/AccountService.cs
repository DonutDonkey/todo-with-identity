using Microsoft.EntityFrameworkCore;

using Tasker.App.Models;
using Tasker.App.DbContext;
using Tasker.App.Controllers;

namespace Tasker.App.Services;

public class AccountService(Logger logger) : IEndpoint {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("account"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", () => false);

        grp.MapPost("/register", async (AccountDb db, AccountRecord account) => {
            var accs = await db.Accounts.ToListAsync();

            logger.Log<AccountService>("Creating Account");
            return false;
        });
    }
}