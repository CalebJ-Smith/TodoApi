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

        internal ReaderWriterLockSlim repoLock = new ReaderWriterLockSlim();
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
                ownerToLists[existingList.Inner.Owner].Remove(listId);
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
            repoLock.EnterReadLock();
            var ownerLists = ownerToLists[owner].Select(a => a.Value.Inner);
            repoLock.ExitReadLock();
            return ownerLists;
        }

        public TodoList? GetOne(long listId)
        {
            repoLock.EnterReadLock();
            var toRet = listIdToList[listId]?.Inner;
            repoLock.ExitReadLock();
            return toRet;
        }

        public TodoList InsertTodoList(string owner, string title)
        {
            repoLock.EnterWriteLock();
            var newList = new TodoList { Owner = owner, Title = title , ListId = nextId};
            var newWrapper = new TodoListWrapper(newList);
            ownerToLists[owner][nextId] = newWrapper;
            listIdToList[nextId] = newWrapper;

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
            listIdToList[todoList.ListId].Lock.EnterWriteLock();
            listIdToList[todoList.ListId].Inner = todoList;
            listIdToList[todoList.ListId].Lock.ExitWriteLock();
        }
    }
}
