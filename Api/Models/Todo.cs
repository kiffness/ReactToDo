namespace Api.Models;

public class Todo
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public DateTime DateCreated { get; set; } = DateTime.Now;
    public DateTime? DateCompleted { get; set; }
    public bool IsComplete { get; set; } = false;
}
