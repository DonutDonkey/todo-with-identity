using ToDo_with_Identity.Client.Services;

namespace ToDo_with_Identity.Client;

public class AccountUi(HtmlService htmlRenderer) : IPage {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("/account"));
    
    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/", async () => {
            // Modify to hold identity of an user
            var rsp = await TaskerCall.Resource("account").Get<bool>();
            
            return (rsp)
                ? Results.Extensions.Html(await htmlRenderer.RenderHtml("navbar_identity.html", null))
                : Results.Extensions.Html(await htmlRenderer.RenderHtml("navbar_no_identity.html", null));
        });
    }
}