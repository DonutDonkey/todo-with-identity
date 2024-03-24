using Tasker.App.Controllers;

namespace Tasker.App.Services;

public class TasksService(Logger logger) : IEndpoint {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("task"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", () => {
            logger.Log<TasksService>($"Test");
        });
    }
}