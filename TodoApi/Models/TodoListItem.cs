namespace TodoApi.Models
{

    public class TodoListItem
    {
        public long ItemId { get; set; }
        public TodoList TodoList { get; set; }
        public string? Name { get; set; }
        public bool IsComplete { get; set; }
    }
}
