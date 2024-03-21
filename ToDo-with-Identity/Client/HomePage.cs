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

        grp.MapGet("/login", async () => {
            var content = await htmlRenderer.RenderHtml("Account/login.html", new { Title = "Login Page" });
            return Results.Extensions.Html(content);
        });

        grp.MapPost("/login", async ([FromForm] string uname, [FromForm] string psw) => {
            var content =
                await htmlRenderer.RenderHtml("Testing/Content2.html", new { User = new { Name = uname, Pass = psw } });

            await Task.Delay(5000);
            return Results.Extensions.Html(content);
        }).DisableAntiforgery();

        grp.MapGet("/register", async() =>
            Results.Extensions.Html(await htmlRenderer.RenderHtml("Account/register.html", null))
        );

        grp.MapPost("/register", async () => {
            var rsp = await TaskerCall.Resource("account").Post(new {});

            return (rsp.IsSuccessStatusCode)
                ? Results.Extensions.Html(await htmlRenderer.RenderHtml("register.html",
                    new { Title = "Register", Message = $"Registration succesfull, an email confirmation have been sent"}))
                : Results.Extensions.Html(await htmlRenderer.RenderHtml("register.html",
                    new { Title = "Register", Message = $"Registration failed, please try again later"})); //TODO: add reason for fuckups
        });
    }
}