using System.Text.Json;
using Microsoft.Extensions.DependencyInjection.Extensions;

using ToDo_with_Identity;
using ToDo_with_Identity.App;
using ToDo_with_Identity.App.Controllers;
using ToDo_with_Identity.App.Services;
using ToDo_with_Identity.Client;
using ToDo_with_Identity.Client.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.RegisterConfiguration();
builder.Services.RegisterServiceEndpoints();
builder.Services.RegisterPageEndpoints();

const string BASE_APP_URL = "http://localhost:5144";
TaskerCall.URI = BASE_APP_URL + "/api/";

var app = builder.Build();

var apiVersionSet = app.MapGroup("api");

app.MapGet("/", () => Results.Redirect("/Home"));

app.RegisterEndpoints(apiVersionSet);
app.RegisterPageEndpoints(null);

app.Run(BASE_APP_URL);


namespace ToDo_with_Identity {
    public static class TaskerConfigurator
    {
        //In case scale grows, use this instead
    
        //     ServiceDescriptor[] serviceDescriptors = assembly
        //         .DefinedTypes
        //         .Where(type => type is { IsAbstract: false, IsInterface: false } &&
        //                        type.IsAssignableTo(typeof(IEndpoint)))
        //         .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type))
        //         .ToArray();
        //
        //     service.TryAddEnumerable(serviceDescriptors);
        //     
        //     return service;
    
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
            services.AddTransient<IEndpoint, TasksService>();
            services.AddTransient<IEndpoint, AccountService>();
            return services;
        }    
    
        public static void RegisterPageEndpoints(this IServiceCollection services) =>
            services.TryAddEnumerable(typeof(Program).Assembly
                .DefinedTypes
                .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                               type.IsAssignableTo(typeof(IPage)))
                .Select(type => ServiceDescriptor.Transient(typeof(IPage), type))
                .ToArray()
            );

        public static IApplicationBuilder RegisterEndpoints(this WebApplication app, RouteGroupBuilder? groupBuilder) {
            foreach (var endpoint in app.Services.GetRequiredService<IEnumerable<IEndpoint>>())
                endpoint.Register(groupBuilder is null ? app : groupBuilder);
            return app;
        }
    
        public static void RegisterPageEndpoints(this WebApplication app, RouteGroupBuilder? groupBuilder) =>
            app.Services.GetRequiredService<IEnumerable<IPage>>()
                .ToList()
                .ForEach(page => page.Register(groupBuilder is null ? app : groupBuilder));
    }
}

public static class TaskerCall {
    public static string URI {get; set;}
    
    public static HttpClient Resource(string url) {
        var httpClient = new HttpClient { BaseAddress = new Uri(URI + url) };
        return httpClient;
    }
    
    public static async Task<T> Get<T>(this HttpClient http){
        var response = await http.GetAsync(http.BaseAddress);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});
        
        return result;
    }
    
    public static async Task<T> Post<T>(this HttpClient http, object? obj) {
        var response = await http.PostAsJsonAsync(http.BaseAddress, obj ?? new {});

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

        return result;
    }
    
    public static async Task<HttpResponseMessage> Post(this HttpClient http, object? obj) {
        var response = await http.PostAsJsonAsync(http.BaseAddress, obj ?? new {});

        return response;
    }
}