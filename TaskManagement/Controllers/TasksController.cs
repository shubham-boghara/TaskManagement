using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Data;
using TaskManagement.Dtos;
using TaskManagement.Models;

namespace TaskManagement.Controllers
{
    [Authorize]
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public TasksController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IActionResult Index()
        {
            return View(new CreateTasksDto());
        }

        public IActionResult Details(int id) {

            return View(new UpdateTasksDto() { TaskId = id});
        }

        public IActionResult List() {
            return View();
        }

        public IActionResult MyTasks()
        {
            return View();
        }

        public IActionResult MyCreatedTask()
        {
            return View();
        }


       
    }
}
