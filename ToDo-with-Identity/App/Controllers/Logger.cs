namespace Tasker.App.Controllers;

public class Logger : ILogger {
    public void Log<T>(string msg) => Console.WriteLine($"{typeof(T).ToString()} : {msg}");
}