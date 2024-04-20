using TodoApi.Models;

namespace TodoApi.DataAccessLayer
{
    public interface ITodoListItemRepository : IDisposable
    {
        ICollection<TodoListItem> GetAll(long listId);
        TodoList GetOne(long itemId);
        void InsertTodoListItem(long listId, TodoListItem item);
        void UpdateTodoListItem(long itemId, TodoListItem item);
        // returns true if something was deleted
        bool DeleteTodoListItem(long itemId);
        void Save();
    }
}
