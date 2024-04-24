using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TodoApi.Controllers;
using TodoApi.DataAccessLayer;
using TodoApi.Models;

namespace UnitTests.Controllers
{
    public class TestTodoListController
    {
        [Fact]
        public void GetOne_ReturnsOne_WhenMultipleUsers()
        {
            // Arrange
            var title = "My first list";
            var mockHttpContext = new Mock<IHttpContextAccessor>();
            var user1bytes = System.Text.Encoding.Default.GetBytes("user1");
            mockHttpContext.Setup(context => context.HttpContext.Session.TryGetValue("user", out user1bytes))
                .Returns(true);
            var mockRepo = new Mock<ITodoListRepository>();
            mockRepo.Setup(repo => repo.GetOne(1))
                .Returns(new TodoList { ListId = 1, Owner = "user1", Title = title });
            var controller = new TodoListController(mockHttpContext.Object, mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var asOkObj = Assert.IsType<OkObjectResult >(result);
            var asList = Assert.IsType<TodoList>(asOkObj.Value);
            Assert.Equal(title, asList.Title);
            Assert.Equal(1, asList.ListId);
        }
    }
}