namespace Tasker.App.Controllers;

public interface ILogger {
    public void Log<T>(string msg);
}