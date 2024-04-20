using TodoApi.Models;

namespace TodoApi.DataAccessLayer
{
    public class InMemoryTodoListItemRepository : ITodoListItemRepository
    {
        private ITodoListRepository listRepository;
        ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        IDictionary<long, ICollection<TodoListItem>> listIdToItems;
        IDictionary<long, TodoListItem> itemIdToItem;

        InMemoryTodoListItemRepository(ITodoListRepository listRepository,
            IDictionary<long, ICollection<TodoListItem>> listIdToItems,
            IDictionary<long, TodoListItem> itemIdToItem)
        {
            this.listRepository = listRepository;
            this.listIdToItems = listIdToItems;
            this.itemIdToItem = itemIdToItem;
        }

        public bool DeleteTodoListItem(long id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public ICollection<TodoListItem> GetAll(long listId)
        {
            throw new NotImplementedException();
        }

        public TodoList GetOne(long itemId)
        {
            throw new NotImplementedException();
        }

        public void InsertTodoListItem(long listId, TodoListItem item)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void UpdateTodoListItem(long itemId, TodoListItem item)
        {
            throw new NotImplementedException();
        }
    }
}
