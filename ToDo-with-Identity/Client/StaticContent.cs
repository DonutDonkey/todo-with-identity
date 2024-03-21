using ToDo_with_Identity.Client.Services;

namespace ToDo_with_Identity.Client;

public class StaticContent(HtmlService htmlRenderer) : IPage {
    public void Register(IEndpointRouteBuilder app) => Map(app.MapGroup("/content"));

    private void Map(IEndpointRouteBuilder grp) {
        grp.MapGet("/style/{item}", async (string item) => 
            Results.Content(await htmlRenderer.GetCss(item), "text/css")
        );
        
        grp.MapGet("/img/{item}", async (string item) => 
            Results.Content(await htmlRenderer.GetSvg(item), "image/svg+xml")
        );
    }
}