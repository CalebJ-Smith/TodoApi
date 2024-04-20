using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TodoApi.DataAccessLayer;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListController : ControllerBase
    {
        private string owner;
        private ITodoListRepository todoListRepository;
        TodoListController(IHttpContextAccessor httpContextAccessor, ITodoListRepository todoListRepository) {
            var session = httpContextAccessor.HttpContext?.Session;
            owner = session?.GetString("user") ?? session?.Id ?? ""; // TODO: instead of defaulting to "", return 401 Unauthorized
            this.todoListRepository = todoListRepository;
        }
        // GET: api/<TodoListController>
        // gets all TodoLists owned by the owner
        [HttpGet]
        public IEnumerable<TodoList> Get()
        {
            return todoListRepository.GetAllByOwner(owner);
        }

        // GET api/<TodoListController>/5
        [HttpGet("{listId}")]
        [ProducesResponseType(typeof(TodoList), 200)]
        public IActionResult Get(int listId)
        {
            var l = todoListRepository.GetOne(listId);
            if (l == null)
            {
                return NoContent();
            }
            if (l.Owner != owner)
            {
                return Forbid();
            }
            return Ok(l);
        }

        // POST api/<TodoListController>
        [HttpPost]
        public TodoList Post([FromBody] string listName)
        {
            return todoListRepository.InsertTodoList(owner, listName);
        }

        // PUT api/<TodoListController>/5
        [HttpPut("{listId}")]
        public IActionResult Put(int listId, [FromBody] string listName)
        {
            var l = todoListRepository.GetOne(listId);
            if (l == null)
            {
                return StatusCode(405, "Method Not Allowed: Use POST for creation");
            }
            if (l.Owner != owner)
            {
                return Forbid();
            }
            // Can't change owner or listId, so title is the only thing that can change
            l.Title = listName;
            
            return Ok(l); // TODO: Return 201

        }

        // DELETE api/<TodoListController>/5
        [HttpDelete("{listId}")]
        [ProducesResponseType(typeof(bool), 200)]
        public IActionResult Delete(int listId)
        {
            var l = todoListRepository.GetOne(listId);
            if (l != null && l.Owner != owner)
            {
                return Forbid();
            }
            return Ok(todoListRepository.DeleteTodoList(listId));
        }
    }
}
