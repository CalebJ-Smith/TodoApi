using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using TodoApi.Models;

namespace TodoApi.DataAccessLayer
{
    public class InMemoryTodoListRepository : ITodoListRepository
    {
        public class TodoListWrapper
        {
            public TodoList Inner { get; set; }
            public ReaderWriterLockSlim Lock { get; set; }
            public TodoListWrapper(TodoList toWrap) {
                Inner = toWrap;
                Lock = new ReaderWriterLockSlim();
            }
        }

        internal ReaderWriterLockSlim repoLock = new ReaderWriterLockSlim(); // necessary (instead of just two ConcurrentDictionary's)
                                                                             // to keep coordinated state between the two dictionaries consistent
        internal IDictionary<long, TodoListWrapper> listIdToList;                      // listId to all lists, irrespective of owner
        internal IDictionary<string, IDictionary<long, TodoListWrapper>> ownerToLists; // owner to the lists that owner owns
        private long nextId = 0;

        public InMemoryTodoListRepository(IDictionary<long, TodoListWrapper> listIdToList,
            IDictionary<string, IDictionary<long,  TodoListWrapper>> ownerToLists)
        {
            this.listIdToList = listIdToList;
            this.ownerToLists = ownerToLists;
        }

        public bool DeleteTodoList(long listId)
        {
            repoLock.EnterWriteLock();

            var listExisted = false;
            if (listIdToList.TryGetValue(listId, out var existingList))
            {
                listExisted = true;
                if (ownerToLists.TryGetValue(existingList.Inner.Owner, out var ownersLists))
                {
                    ownersLists.Remove(listId);
                }
                // however, don't remove the owner mapping 
                listIdToList.Remove(listId);
            }

            repoLock.ExitWriteLock();
            return listExisted;
        }

        public void Dispose()
        {
            // not applicable
        }

        public IEnumerable<TodoList> GetAllByOwner(string owner)
        {
            IEnumerable<TodoList> ownerLists = [];
            repoLock.EnterReadLock();
            if (ownerToLists.TryGetValue(owner, out var wrapperList))
            {
                ownerLists = wrapperList.Select(a => a.Value.Inner);
            }
            repoLock.ExitReadLock();
            return ownerLists;
        }

        public TodoList? GetOne(long listId)
        {
            TodoList? toRet = null; 
            repoLock.EnterReadLock();
            if(listIdToList.TryGetValue(listId, out var val))
            {
                toRet = val.Inner;
            }
            repoLock.ExitReadLock();
            return toRet;
        }

        public TodoList InsertTodoList(string owner, string title)
        {
            repoLock.EnterWriteLock();
            var newList = new TodoList { Owner = owner, Title = title , ListId = nextId};
            var newWrapper = new TodoListWrapper(newList);
            if (ownerToLists.TryGetValue(owner, out var listsForOwner)) // Upsert ownerToLists[owner][nextId] = newWrapper;
            {
                listsForOwner.Add(nextId, newWrapper);
            } else
            {
                ownerToLists.Add(owner, new Dictionary<long, TodoListWrapper>
                {
                    { nextId, newWrapper }
                });

            }
            listIdToList.Add(nextId, newWrapper);

            nextId += 1;
            repoLock.ExitWriteLock();
            return newList;
        }

        public void Save()
        {
            // InMemoryTodoListRepository is not persistant; don't need to do anything
        }

        public void UpdateTodoList(TodoList todoList)
        {
            repoLock.EnterReadLock();
            if (listIdToList.TryGetValue(todoList.ListId, out var list) ){
                list.Lock.EnterWriteLock();
                repoLock.ExitReadLock();// hand-over-hand locking
                list.Inner = todoList;
                list.Lock.ExitWriteLock();
            }
            else
            {
                repoLock.ExitReadLock();
            }
        }
    }
}
