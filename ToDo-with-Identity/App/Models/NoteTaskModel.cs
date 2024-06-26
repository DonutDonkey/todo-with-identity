namespace Tasker.App.Models;

public enum NoteTaskType { NoCategory, Daily, Monthly, Yearly }

public record NoteTaskModel(int Id, bool Completed, string Description, NoteTaskType TaskType) {}