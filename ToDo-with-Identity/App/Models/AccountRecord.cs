namespace Tasker.App.Models;

public record AccountRecord(string Username, string Email, string Password) {
    public int Id { get; init; }
    public string? Identity { get; set; }
}