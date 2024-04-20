namespace TodoApi.Models
{
    public class TodoList 
    {
        public long ListId { get; set; }
        public string Owner { get; set; }
        public string Title { get; set; }
        public ICollection<TodoListItem> Items { get; set; }
    }
}
