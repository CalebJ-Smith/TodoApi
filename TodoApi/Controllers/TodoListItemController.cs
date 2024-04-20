using Microsoft.AspNetCore.Mvc;
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
        public TodoListItemController(IHttpContextAccessor httpContextAccessor, ITodoListRepository todoListRepository, ITodoListItemRepository todoListItemRepository) {
            var session = httpContextAccessor.HttpContext?.Session;
            owner = session?.GetString("user") ?? session?.Id ?? "";
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

        // POST api/<TodoListItemController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<TodoListItemController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<TodoListItemController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
