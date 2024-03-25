using Microsoft.AspNetCore.Mvc;

using Tasker.App.Models;
using Tasker.App.Controllers;
using Tasker.Client.Services;

namespace Tasker.Client;

public class HomePage(Logger logger, HtmlService htmlRenderer) : IPage {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("/Home"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", async () => {
            var content = await htmlRenderer.RenderTemplate("home.html", new { Title = "Home", Model = new { RandomQuote = "Sunny day" } });
            return Results.Extensions.Html(content);
        });

        grp.MapGet("/identity", async (HttpContext context) => {
            var rsp = await TaskerCall.Resource("account")
                .Post<AccountRecord?>(context.Request.Cookies["TaskerAuthorization"]);
            
            return (rsp is not null)
                ? Results.Extensions.Html(await htmlRenderer.RenderHtml("navbar_identity.html", null))
                : Results.Extensions.Html(await htmlRenderer.RenderHtml("navbar_no_identity.html", null));
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

        grp.MapPost("/register", async ([FromForm] string username, [FromForm] string email, [FromForm] string password) => {
            var rsp = await TaskerCall
                .Resource("account/register")
                .Post(new AccountRecord(Username: username, Email: email, Password: password ));

            return (rsp.IsSuccessStatusCode)
                ? Results.Extensions.Html(await htmlRenderer.RenderHtml("Account/register_response.html",
                    new { Message = $"Registration succesfull, an email confirmation have been sent" }))
                : Results.Extensions.Html(await htmlRenderer.RenderHtml("Account/register_response.html",
                    new { Message = $"{ rsp.Content.ReadAsStringAsync().Result }" }));
        }).DisableAntiforgery();
    }
}