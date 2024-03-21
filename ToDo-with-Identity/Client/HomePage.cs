using Microsoft.AspNetCore.Mvc;
using ToDo_with_Identity.App.Controllers;
using ToDo_with_Identity.Client.Services;

namespace ToDo_with_Identity.Client;

public class HomePage(Logger logger, HtmlService htmlRenderer) : IPage {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("/Home"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", async () => {
            var content = await htmlRenderer.RenderTemplate("home.html", new { Title = "Home", Model = new { RandomQuote = "Sunny day" } });
            return Results.Extensions.Html(content);
        });

        grp.MapGet("/content1", async () => {
            var content = await htmlRenderer.RenderTemplate("Testing/Content2.html", new {
                Title = "Content"
            });
            
            return Results.Extensions.Html(content);
        });
        
        grp.MapGet("/content2", async () => {
            var content = await htmlRenderer.RenderTemplate("Testing/Content2.html", new {
                Title = "Content",
                Person = new { Name = "Joe Mama", Age = 69 }
            });
            
            return Results.Extensions.Html(content);
        });

        grp.MapGet("/login", async () => {
            var content = await htmlRenderer.RenderTemplate("Account/login.html", new { Title = "Login Page" });
            return Results.Extensions.Html(content);
        });

        grp.MapPost("/login", async ([FromForm] string uname, [FromForm] string psw) => {
            var content =
                await htmlRenderer.RenderHtml("Testing/Content2.html", new { User = new { Name = uname, Pass = psw } });

            await Task.Delay(5000);
            return Results.Extensions.Html(content);
        }).DisableAntiforgery();
    }
}