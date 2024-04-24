using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApi.DataAccessLayer;
using TodoApi.Models;
using static TodoApi.DataAccessLayer.InMemoryTodoListRepository;

namespace UnitTests.DataAccessLayer
{
    public class TestInMemoryTodoListRepository
    {
        [Fact]
        public void GetAll_ReturnsOneList_WhenMultipleUsers()
        {
            // Arrange
            var myList = new TodoListWrapper(new TodoList{ Owner = "user1", ListId = 0 });
            var theirList = new TodoListWrapper(new TodoList{ Owner = "user2", ListId = 1 });
            var mockListIdToList = new Dictionary<long, TodoListWrapper> { { 0, myList }, { 1, theirList } };
            var mockOwnerToLists = new Dictionary<string, IDictionary<long, TodoListWrapper>>
            {
                {
                    "user1", new Dictionary<long, TodoListWrapper> { { 0, myList} }
                },
                {
                    "user2", new Dictionary<long, TodoListWrapper> { { 1, theirList } }
                }
            };
            var listRepo = new InMemoryTodoListRepository(mockListIdToList, mockOwnerToLists);

            // Act
            var result = listRepo.GetAllByOwner("user1");
            // Assert
            Assert.Single(result);
            Assert.Equal(0, result.First().ListId);
        }

        [Fact]
        public void GetAll_ReturnsNone_WhenOtherUsersButNoneOfMe()
        {
            // Arrange
            var theirList = new TodoListWrapper(new TodoList { Owner = "user2", ListId = 1 });
            var mockListIdToList = new Dictionary<long, TodoListWrapper> { { 1, theirList } };
            var mockOwnerToLists = new Dictionary<string, IDictionary<long, TodoListWrapper>>
            {
                {
                    "user2", new Dictionary<long, TodoListWrapper> { { 1, theirList } }
                }
            };
            var listRepo = new InMemoryTodoListRepository(mockListIdToList, mockOwnerToLists);

            // Act
            var result = listRepo.GetAllByOwner("user1");
            // Assert
            Assert.Empty(result);
        }

        // TODO: Write more tests....
    }
}
