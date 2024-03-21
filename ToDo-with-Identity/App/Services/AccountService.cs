using ToDo_with_Identity.App.Controllers;

namespace ToDo_with_Identity.App.Services;

public class AccountService(Logger logger) : IEndpoint {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("account"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", () => false);
        grp.MapGet("/log", () => logger.Log("Hello Accounts!"));
    }
}