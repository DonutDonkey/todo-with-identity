using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Tasker.App;
using Tasker.App.DbContext;
using Tasker.App.Controllers;
using Tasker.Client;
using Tasker.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AccountDb>(options => options.UseInMemoryDatabase("Accounts"));

builder.Services.RegisterConfiguration();
builder.Services.RegisterServiceEndpoints();
builder.Services.RegisterPageEndpoints();

var app = builder.Build();
var apiVersionSet = app.MapGroup("api");

app.RegisterEndpoints(apiVersionSet);
app.RegisterPageEndpoints(null);

app.MapGet("/", () => Results.Redirect("/Home"));

TaskerCall.Configure();

app.Run();

public static class TaskerConfigurator {
    /// <summary>
    /// Config per project to register dependencies, and add configuration
    /// </summary>
    /// <param name="services"> <see cref="IServiceCollection" /> of the current <see cref="WebApplication" /> builder.</param>
    /// <returns><see cref="IServiceCollection" /></returns>
    public static IServiceCollection RegisterConfiguration(this IServiceCollection services) {
        services.AddSingleton<Logger>();
        services.AddSingleton<HtmlService>();

        return services;
    }

    public static IServiceCollection RegisterServiceEndpoints(this IServiceCollection services) {
        services.TryAddEnumerable(typeof(Program).Assembly.DefinedTypes
            .Where(type => type is {IsAbstract: false, IsInterface: false} && type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
            .ToArray());

        return services;
    }

    public static IServiceCollection RegisterPageEndpoints(this IServiceCollection services) {
        services.TryAddEnumerable(typeof(Program).Assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } && type.IsAssignableTo(typeof(IPage)))
            .Select(type => ServiceDescriptor.Transient(typeof(IPage), type))
            .ToArray());

        return services;
    }

    public static IApplicationBuilder RegisterEndpoints(this WebApplication app, RouteGroupBuilder? groupBuilder) {
        foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
            endpoint.Register(groupBuilder is null ? app : groupBuilder);
        return app;
    }

    public static IApplicationBuilder RegisterPageEndpoints(this WebApplication app, RouteGroupBuilder? groupBuilder) {
        foreach (var page in app.Services.GetRequiredService<IEnumerable<IPage>>())
            page.Register(groupBuilder is null ? app : groupBuilder);
        return app;
    }
}

public static class TaskerCall {
    public static string? URL { get; private set; }

    public static void Configure() {
        if (URL is not null) return;

        var launchSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "launchSettings.json");
        if (!File.Exists(launchSettingsPath)) return;

        var launchSettingsContent = File.ReadAllText(launchSettingsPath);
        using JsonDocument document = JsonDocument.Parse(launchSettingsContent);
        JsonElement root = document.RootElement;

        if (!root.TryGetProperty("profiles", out JsonElement profiles)) return;
        if (!profiles.TryGetProperty("http", out JsonElement http)) return;

        URL = http.TryGetProperty("applicationUrl", out JsonElement applicationUrl)
            ? applicationUrl.GetString() + "/api/"
            : null;
    }

    public static HttpClient Resource(string url) => new HttpClient { BaseAddress = new Uri(URL + url) };

    public static async Task<HttpResponseMessage> Get(this HttpClient http) => await http.GetAsync(http.BaseAddress);
    
    public static async Task<T> Get<T>(this HttpClient http) {
        var response = await http.GetAsync(http.BaseAddress);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        return result;
    }
    
    public static async Task<HttpResponseMessage> Post(this HttpClient http, object? obj) => await http.PostAsJsonAsync(http.BaseAddress, obj ?? new {});

    public static async Task<T> Post<T>(this HttpClient http, object? obj) {
        var response = await http.PostAsJsonAsync(http.BaseAddress, obj ?? new {});

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

        return result;
    }
}