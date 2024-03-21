using System.Reflection;

namespace ToDo_with_Identity.Client.Services;

public class HtmlService {
    private string Img => Path.Combine(Directory.GetCurrentDirectory(), "Client", "Static", "img");
    private string Layout => Path.Combine(Directory.GetCurrentDirectory(), "Client", "Static", "index.html");

    public async Task<string> GetCss(string cssFilePath) =>
        await File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Client", "Static", 
            "Styles", cssFilePath));
    
    public async Task<string> GetSvg(string svgFilePath) => await File.ReadAllTextAsync(Path.Combine(Img, svgFilePath));
    
    public async Task<string> RenderHtml(string htmlFilePath, object? data) => 
        Render(await ReadFile(htmlFilePath), data);
    
    public async Task<string> RenderTemplate(string templatePath, object? data) {
        string layout = await ReadFile(Layout);
        string content = string.Empty;
        
        if (templatePath != string.Empty)
            content = await ReadFile(templatePath);
        
        content = layout.Replace("{{Content}}", content);
        
        return Render(content, data);
    }

    private Task<string> ReadFile(string htmlFilePath) {
        try {
            return File.ReadAllTextAsync(Path.Combine(Directory.GetCurrentDirectory(), "Client", "Static", htmlFilePath));
        } catch (Exception ex) {
            return Task.FromResult($"<p>Rendering error : {ex.Message}</p>");
        }
    }

    private string Render(string content, object? data, string prefix = "") {
        if (data is null) return content;
        
        foreach (var property in data.GetType().GetProperties())
            content = BindData(content, data, prefix, property);
        
        return content;
    }

    private string BindData(string content, object data, string prefix, PropertyInfo property) {
        string token = "{{" + prefix + property.Name + "}}";
        object? value = property.GetValue(data) ?? string.Empty;

        if (!property.PropertyType.IsValueType && property.PropertyType != typeof(string))
            content = Render(content, value, property.Name + ".");
        else
            content = content.Replace(token, value.ToString());
        return content;
    }
}