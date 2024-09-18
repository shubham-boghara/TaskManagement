using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Data;
using TaskManagement.Dtos;
using TaskManagement.Models;

namespace TaskManagement.Controllers.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetTasks()
        {
            if (_context.Tasks == null)
            {
                return NotFound();
            }
            return await _context.Tasks.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetTasks(int id)
        {
            var findTasks = await _context.Tasks.FindAsync(id);
            if (findTasks == null)
            {
                return Ok(new { success = false });
            }

            return Ok(new { success = true, message = "Task get successfully!", data = findTasks });
        }

        // PUT: api/Tasks/5
        [HttpPut]
        public async Task<IActionResult> PutTasks(UpdateTasksDto updateTasksDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { success = false });
            }

            var findTasks = await _context.Tasks.FindAsync(updateTasksDto.TaskId);
            if (findTasks == null)
            {
                return Ok(new { success = false });
            }

            findTasks.Status = updateTasksDto.Status;
            findTasks.DueDate = updateTasksDto.DueDate;
            findTasks.CreatedDate = DateTime.Now;
            findTasks.CreatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            findTasks.PriorityId = updateTasksDto.PriorityId;
            findTasks.Description = updateTasksDto.Description;
            findTasks.Title = updateTasksDto.Title;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Task updated successfully!", data = findTasks });
        }

        // POST: api/Tasks
        [HttpPost]
        public async Task<ActionResult> PostTasks([FromBody] CreateTasksDto createTasksDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { success = false });
            }

            var newTasks = new Tasks();
            newTasks.Status = createTasksDto.Status;
            newTasks.DueDate = createTasksDto.DueDate;
            newTasks.CreatedDate = DateTime.Now;
            newTasks.CreatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            newTasks.PriorityId = createTasksDto.PriorityId;
            newTasks.Description = createTasksDto.Description;
            newTasks.Title = createTasksDto.Title;


            await _context.Tasks.AddAsync(newTasks);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Task created successfully!", data = newTasks });
        }

        // DELETE: api/Tasks/remove/5
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> DeleteTasks(int id)
        {
            var findTasks = await _context.Tasks.FindAsync(id);
            if (findTasks == null)
            {
                return Ok(new { success = false });
            }

            _context.Remove(findTasks);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Task deleted successfully!" });
        }

        // GET: api/Tasks/AllList
        [HttpPost]
        [Route("alllist")]
        public async Task<ActionResult> GetPagedTasks()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            
            int pageSize = length != null ? int.Parse(length) : 0;
            int skip = start != null ? int.Parse(start) : 0;

            
            var taskData = _context.Vm_Tasks
                                   // .Include(t => t.Priority)
                                   .AsQueryable();

          
            if (!string.IsNullOrEmpty(searchValue))
            {
                if (DateTime.TryParse(searchValue, out DateTime dueDateFilter))
                {
                    taskData = taskData.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDateFilter.Date);
                }
                else
                {
                    taskData = taskData.Where(t => t.Title.Contains(searchValue) || t.Description.Contains(searchValue));
                }
            }

            
            switch (sortColumnIndex)
            {
                case "0": // Title
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Title) : taskData.OrderByDescending(t => t.Title);
                    break;
                case "1": // Status
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Status) : taskData.OrderByDescending(t => t.Status);
                    break;
                case "2": // Priority
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Name) : taskData.OrderByDescending(t => t.Name);
                    break;
                case "3": 
                    if (sortDirection == "asc")
                        taskData = taskData.OrderBy(t => t.DueDate ?? DateTime.MaxValue); 
                    else
                        taskData = taskData.OrderByDescending(t => t.DueDate ?? DateTime.MinValue); 
                    break;
            }

           
            var filteredTasks = await taskData.Skip(skip).Take(pageSize).ToListAsync();

            
            var totalRecords = await _context.Tasks.CountAsync();
            var filteredRecords = taskData.Count();

            
            return Ok(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = filteredTasks.Select(t => new
                {
                    t.TaskId,
                    t.Title,
                    t.Status,
                    Priority = t?.Name ?? "None",
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("dd-MM-yyyy") : "" 
                })
            });
        }


        // POST: api/Tasks/Assign/User
        [HttpPost]
        [Route("assign/user")]
        public async Task<ActionResult> PostTasksAssign([FromBody] CreateTaskAssignmentsDto  createTaskAssignmentsDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { success = false });
            }

            var newTaskAssignments = new TaskAssignments();
            newTaskAssignments.TaskId = createTaskAssignmentsDto.TaskId;
            newTaskAssignments.AssignedToUserId = createTaskAssignmentsDto.AssignedToUserId;
            newTaskAssignments.AssignedDate = DateTime.Now;
            newTaskAssignments.AssignedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            await _context.TaskAssignments.AddAsync(newTaskAssignments);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "TaskAssign created successfully!", data = newTaskAssignments });
        }


        // DELETE: api/Tasks/Assign/User/Remove
        [HttpDelete("{id}")]
        [Route("assign/user/remove")]
        public async Task<IActionResult> DeleteTasksAssign(int id)
        {
            var findTaskAssignments = await _context.TaskAssignments.FindAsync(id);
            if (findTaskAssignments == null)
            {
                return Ok(new { success = false });
            }

            _context.Remove(findTaskAssignments);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "TaskAssign deleted successfully!" });
        }

        // GET: api/Tasks/Me/List
        [HttpPost]
        [Route("me/list")]
        public async Task<ActionResult> GetPagedMyTasks()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            
            int pageSize = length != null ? int.Parse(length) : 0;
            int skip = start != null ? int.Parse(start) : 0;

           
            var taskData = _context.Vm_TaskAssignmentsWithTask.Where(c => c.AssignedToUserId == userId)
                                   // .Include(t => t.Priority)
                                   .AsQueryable();

           
            if (!string.IsNullOrEmpty(searchValue))
            {
                if (DateTime.TryParse(searchValue, out DateTime dueDateFilter))
                {
                    taskData = taskData.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDateFilter.Date);
                }
                else
                {
                    taskData = taskData.Where(t => t.Title.Contains(searchValue) || t.Description.Contains(searchValue));
                }
            }

          
            switch (sortColumnIndex)
            {
                case "0": // Title
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Title) : taskData.OrderByDescending(t => t.Title);
                    break;
                case "1": // Status
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Status) : taskData.OrderByDescending(t => t.Status);
                    break;
                case "2": // Priority
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Name) : taskData.OrderByDescending(t => t.Name);
                    break;
                case "3": 
                    if (sortDirection == "asc")
                        taskData = taskData.OrderBy(t => t.DueDate ?? DateTime.MaxValue); 
                    else
                        taskData = taskData.OrderByDescending(t => t.DueDate ?? DateTime.MinValue); 
                    break;
            }

          
            var filteredTasks = await taskData.Skip(skip).Take(pageSize).ToListAsync();

            var totalRecords = await _context.Tasks.CountAsync();
            var filteredRecords = taskData.Count();

           
            return Ok(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = filteredTasks.Select(t => new
                {
                    t.TaskId,
                    t.Title,
                    t.Status,
                    Priority = t?.Name ?? "None",
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("dd-MM-yyyy") : "" 
                })
            });
        }


        // GET: api/Tasks/Me/Created/List
        [HttpPost]
        [Route("me/created/list")]
        public async Task<ActionResult> GetPagedMyCreatedTasks()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var start = Request.Form["start"].FirstOrDefault();
            var length = Request.Form["length"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

            
            int pageSize = length != null ? int.Parse(length) : 0;
            int skip = start != null ? int.Parse(start) : 0;

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           
            var taskData = _context.Vm_Tasks.Where(c => c.CreatedByUserId == userId)
                                   // .Include(t => t.Priority)
                                   .AsQueryable();

           
            if (!string.IsNullOrEmpty(searchValue))
            {
                if (DateTime.TryParse(searchValue, out DateTime dueDateFilter))
                {
                    taskData = taskData.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDateFilter.Date);
                }
                else
                {
                    taskData = taskData.Where(t => t.Title.Contains(searchValue) || t.Description.Contains(searchValue));
                }
            }

           
            switch (sortColumnIndex)
            {
                case "0": // Title
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Title) : taskData.OrderByDescending(t => t.Title);
                    break;
                case "1": // Status
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Status) : taskData.OrderByDescending(t => t.Status);
                    break;
                case "2": // Priority
                    taskData = sortDirection == "asc" ? taskData.OrderBy(t => t.Name) : taskData.OrderByDescending(t => t.Name);
                    break;
                case "3": 
                    if (sortDirection == "asc")
                        taskData = taskData.OrderBy(t => t.DueDate ?? DateTime.MaxValue); 
                    else
                        taskData = taskData.OrderByDescending(t => t.DueDate ?? DateTime.MinValue); 
                    break;
            }

           
            var filteredTasks = await taskData.Skip(skip).Take(pageSize).ToListAsync();

            
            var totalRecords = await _context.Tasks.CountAsync();
            var filteredRecords = taskData.Count();

           
            return Ok(new
            {
                draw = draw,
                recordsTotal = totalRecords,
                recordsFiltered = filteredRecords,
                data = filteredTasks.Select(t => new
                {
                    t.TaskId,
                    t.Title,
                    t.Status,
                    Priority = t?.Name ?? "None",
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("dd-MM-yyyy") : "" 
                })
            });
        }

        // PUT: api/Tasks/Me/Update/Status
        [HttpPut]
        [Route("me/update/status")]
        public async Task<IActionResult> PutTasks(UpdateTasksStatusDto  updateTasksStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new { success = false });
            }

            var findTasks = await _context.Tasks.FindAsync(updateTasksStatusDto.TaskId);
            if (findTasks == null)
            {
                return Ok(new { success = false });
            }

            findTasks.Status = updateTasksStatusDto.Status;

            var taskUpdates = new TaskUpdates()
            {
                TaskId = updateTasksStatusDto.TaskId,
                UpdatedByUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                Status = updateTasksStatusDto.Status,
                UpdatedDate = DateTime.Now,
                Notes = string.Empty
            };

            await _context.TaskUpdates.AddAsync(taskUpdates);

            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Task status updated successfully!", data = findTasks });
        }

        // GET: api/Tasks/Taskpriority
        [HttpGet]
        [Route("taskpriority")]
        public async Task<IActionResult> GetTaskPriority()
        {
            var taskPriority = await _context.TaskPriority.Select(c => new { c.PriorityId, c.Name}).ToListAsync();

            return Ok(new { success = true, message = "", data = taskPriority });
        }

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> GetUserList()
        {
            var users = await _context.Users.Select(c => new {c.Id, c.UserName}).ToListAsync();

            return Ok(new { success = true, data = users });
        }

        private bool TasksExists(int id)
        {
            return (_context.Tasks?.Any(e => e.TaskId == id)).GetValueOrDefault();
        }
    }
}
