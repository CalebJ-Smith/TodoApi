using TodoApi.Models;

namespace TodoApi.DataAccessLayer
{
    public interface ITodoListItemRepository : IDisposable
    {
        ICollection<TodoListItem> GetAll(long listId);
        TodoListItem? GetOne(long itemId);
        // returns the itemId
        long InsertTodoListItem(long listId, string name, bool done);
        // returns true if something was updated
        bool UpdateTodoListItem(long itemId, string description, bool isComplete);
        // returns true if something was deleted
        bool DeleteTodoListItem(long itemId);
        void Save();
    }
}
