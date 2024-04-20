
using TodoApi.Models;

namespace TodoApi.DataAccessLayer
{
    public interface ITodoListRepository : IDisposable
    {
        IEnumerable<TodoList> GetAllByOwner(string owner);
        TodoList? GetOne(long listId);
        TodoList InsertTodoList(string owner, string title);
        void UpdateTodoList(TodoList todoList);
        // returns true if something was deleted
        bool DeleteTodoList(long listId);
        void Save();
    }
}
