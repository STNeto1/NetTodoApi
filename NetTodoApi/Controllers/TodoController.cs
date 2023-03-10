using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTodoApi.DTO.TodoItem;
using NetTodoApi.Models;

namespace NetTodoApi.Controllers
{
    [Route("api/todos")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public TodoController(DatabaseContext context)
        {
            _context = context;
        }

        // GET: api/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/todos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(Guid id)
        {
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/todos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(Guid id, UpdateTodoItem todoItem)
        {
            var existingTodoItem = await _context.TodoItems.FindAsync(id);
            if (existingTodoItem == null)
            {
                return NotFound();
            }

            existingTodoItem.Name = todoItem.Name;
            existingTodoItem.IsCompleted = todoItem.IsCompleted;

            try
            {
                _context.TodoItems.Update(existingTodoItem);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/todos
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(CreateTodoItem todoItem)
        {
            if (_context.TodoItems == null)
            {
                return Problem("Entity set 'DatabaseContext.TodoItems'  is null.");
            }

            var existing = _context.TodoItems.Count();
            _context.TodoItems.Add(new TodoItem
            {
                Name = todoItem.Name,
                IsCompleted = false
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTodoItem", new {id = existing + 1}, todoItem);
        }

        // DELETE: api/todos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoItem(Guid id)
        {

            
            if (_context.TodoItems == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TodoItemExists(Guid id)
        {
            return (_context.TodoItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}