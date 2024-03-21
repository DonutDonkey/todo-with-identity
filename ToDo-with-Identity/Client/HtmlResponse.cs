using System.Text;
using System.Net.Mime;

namespace ToDo_with_Identity.Client;

public static partial class ResultExtensions {
    public static IResult Html(this IResultExtensions resultExtensions, string html) {
        ArgumentNullException.ThrowIfNull(resultExtensions);
        
        return new HtmlResponse(html);
    }
}

public class HtmlResponse(string html) : IResult {
    public Task ExecuteAsync(HttpContext httpContext) {
        httpContext.Response.ContentType = MediaTypeNames.Text.Html;
        httpContext.Response.ContentLength = Encoding.UTF8.GetByteCount(html);
        return httpContext.Response.WriteAsync(html);
    }
}