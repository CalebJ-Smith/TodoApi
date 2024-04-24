using TodoApi.Models;

namespace TodoApi.DataAccessLayer
{
    public class InMemoryTodoListItemRepository : ITodoListItemRepository
    {
        private ITodoListRepository listRepository;
        ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        IDictionary<long, IDictionary<long, TodoListItem>> listIdToItems;
        IDictionary<long, TodoListItem> itemIdToItem;
        private long nextItemId = 0;

        public InMemoryTodoListItemRepository(ITodoListRepository listRepository,
            IDictionary<long, IDictionary<long, TodoListItem>> listIdToItems,
            IDictionary<long, TodoListItem> itemIdToItem)
        {
            this.listRepository = listRepository;
            this.listIdToItems = listIdToItems;
            this.itemIdToItem = itemIdToItem;
        }

        public bool DeleteTodoListItem(long itemId)
        {
            Lock.EnterWriteLock();
            var existed = false;
            if (itemIdToItem.Remove(itemId, out var todoListItem))
            {
                existed = true;
                var listId = todoListItem.TodoList.ListId;
                listIdToItems.Remove(listId);
            }
            Lock.ExitWriteLock();
            return existed;
        }

        public void DeleteItemsOfTodoList(long listId)
        {
            Lock.EnterWriteLock();
            listIdToItems.Remove(listId, out var itemsToDelete);
            foreach (var itemId in itemsToDelete?.Keys ?? [])
            {
                itemIdToItem.Remove(itemId);
            }
            Lock.ExitWriteLock();
        }

        public void Dispose()
        {
            // TODO: figure out if we actually need to do something here
        }

        public ICollection<TodoListItem> GetAll(long listId)
        {
            Lock.EnterReadLock();
            ICollection<TodoListItem> items = [];
            if (listIdToItems.TryGetValue(listId, out var itemsDict))
            {
                items = itemsDict.Values;
            }
            Lock.ExitReadLock();
            return items;
        }

        public TodoListItem? GetOne(long itemId)
        {
            Lock.EnterReadLock();
            var item = itemIdToItem[itemId];
            Lock.ExitReadLock();
            return item;
        }

        // Requires that listId already exists
        public long InsertTodoListItem(long listId, string name, bool done)
        {
            long createdItemId;
            var list = listRepository.GetOne(listId);
            if (list == null)
            {
                throw new KeyNotFoundException($"listId {listId} not found; required to create a list item");
            }
            Lock.EnterUpgradeableReadLock();
            createdItemId = nextItemId;
            var item = new TodoListItem { IsComplete = done, ItemId = createdItemId, Description = name, TodoList = list };

            Lock.EnterWriteLock();
            itemIdToItem.Add(nextItemId, item);
            if (listIdToItems.TryGetValue(listId, out var items))
            {
                items.Add(nextItemId, item);
            }
            else
            {
                listIdToItems.Add(listId, new Dictionary<long, TodoListItem> { { nextItemId, item } });
            }
            nextItemId += 1;
            Lock.ExitWriteLock();
            Lock.ExitUpgradeableReadLock();
            return createdItemId;
        }

        public void Save()
        {
            // Not applicable for an in-memory database
        }

        public bool UpdateTodoListItem(long itemId, string description, bool isComplete)
        {
            Lock.EnterUpgradeableReadLock();
            var updated = false;
            if (itemIdToItem.TryGetValue(itemId, out var existingItem))
            {
                updated = true;
                Lock.EnterWriteLock();
                existingItem.Description = description;
                existingItem.IsComplete = isComplete;
                Lock.ExitWriteLock();
            }
            Lock.ExitUpgradeableReadLock();
            return updated;
        }
    }
}
