using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Runtime.Intrinsics.X86;
using System;
using TodoApi.DataAccessLayer;
using TodoApi.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListItemController : ControllerBase
    {
        private string owner;
        private ITodoListRepository _listRepository;
        private ITodoListItemRepository _itemRepository;
        public TodoListItemController(ITodoListRepository todoListRepository, ITodoListItemRepository todoListItemRepository) {
            // var session = httpContextAccessor.HttpContext?.Session;
            owner = "caleb";// session?.GetString("user") ?? session?.Id ?? "";
            _listRepository = todoListRepository;
            _itemRepository = todoListItemRepository;
        }

        // GET api/<TodoListItemController>/5
        [HttpGet("{listId}")]
        [ProducesResponseType(typeof(IEnumerable<TodoListItem>), 200)]
        public IActionResult GetAllOfList(int listId)
        {
            var list = _listRepository.GetOne(listId);
            if (list == null) {
                return NotFound();
            }
            if (list?.Owner != owner) {
                return Forbid();
            }
            return Ok(_itemRepository.GetAll(listId));
        }

        // POST api/<TodoListItemController>/5
        [HttpPost("{itemId}")]
        [ProducesResponseType(typeof(bool), 200)]
        public IActionResult Post(long itemId, [FromQuery] string description, [FromQuery] bool done) // update
        {
            var existingItem = _itemRepository.GetOne(itemId);
            if (existingItem == null)
            {
                return NotFound();
            }
            if (existingItem.TodoList.Owner != owner) {
                return Forbid();
            }
            return Ok(_itemRepository.UpdateTodoListItem(itemId, description, done));
        }

        // PUT api/<TodoListItemController>/5
        [HttpPut("{listId}")] 
        [ProducesResponseType(typeof(long), 200)]
        public IActionResult Put(long listId, [FromQuery] string name, [FromQuery] bool done) // create
        {

            var existingList = _listRepository.GetOne(listId);
            if (existingList == null)
            {
                return NotFound();
            }
            if (existingList.Owner != owner) {
                return Forbid();
            }
            // don't need to catch the exception because we check that condition earlier
            return Ok(_itemRepository.InsertTodoListItem(listId, name, done));
        }


        // DELETE api/<TodoListItemController>/5
        [HttpDelete("{itemId}")]
        [ProducesResponseType(typeof(bool), 200)]
        public IActionResult Delete(long itemId)
        {
            
            var existingItem = _itemRepository.GetOne(itemId);
            if (existingItem == null)
            {
                return NotFound();
            }
            if (existingItem.TodoList.Owner != owner) {
                return Forbid();
            }
            return Ok(_itemRepository.DeleteTodoListItem(itemId));
        }

    }
}
