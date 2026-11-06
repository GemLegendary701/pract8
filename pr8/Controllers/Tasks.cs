using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pr8.Models;
using System.Security.Claims;
using Task = pr8.Models.Task;

namespace pr8.Controllers
{
    [ApiController]
    [Route("api/v1/tasks")]
    [Authorize]
    public class Tasks : ControllerBase
    {
        private readonly TaskManagerDbContext _db;

        public Tasks(TaskManagerDbContext db)
        {
            _db = db;
        }

        public record TaskRequest(string Title, string Description);

        [HttpGet]
        public IActionResult GetTasks()
        {
            if (User.IsInRole("Admin"))
            {
                return Ok(_db.Tasks.ToList());
            }

            var login = User.Identity?.Name;
            var myTasks = _db.Tasks.Where(t => t.OwnerLogin == login).ToList();

            return Ok(myTasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            var task = _db.Tasks.Find(id);

            if (task == null)
                return NotFound("Задача не найдена");

            if (!User.IsInRole("Admin") && task.OwnerLogin != User.Identity?.Name)
                return Forbid();

            return Ok(task);
        }

        [HttpPost]
        public IActionResult CreateTask(TaskRequest request)
        {
            var login = User.Identity?.Name;

            var newTask = new Task
            {
                Title = request.Title,
                Description = request.Description,
                OwnerLogin = login,
                IsCompleted = false
            };

            _db.Tasks.Add(newTask);
            _db.SaveChanges();

            return CreatedAtAction(nameof(GetTask), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, TaskRequest request)
        {
            var task = _db.Tasks.Find(id);

            if (task == null)
                return NotFound("Задача не найдена");

            if (!User.IsInRole("Admin") && task.OwnerLogin != User.Identity?.Name)
                return Forbid();

            task.Title = request.Title;
            task.Description = request.Description;

            _db.SaveChanges();

            return Ok(task);
        }
    }
}