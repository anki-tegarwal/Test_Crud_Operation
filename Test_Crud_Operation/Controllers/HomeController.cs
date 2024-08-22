using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;

namespace Test_Crud_Operation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EmployeeDb _context;

        public HomeController(ILogger<HomeController> logger,EmployeeDb context)
        {
            _logger = logger;
            _context = context;
        }
        
        public IActionResult Index()
        {
            ViewBag.Departments = _context.Departments
           .OrderBy(d => d.Name)
           .Select(d => d.Name)
           .ToList();

            return View();
        }


        [HttpGet]
        public async Task<JsonResult> GetEmployeeDataByDepartment(string department)
        {
            if (string.IsNullOrEmpty(department))
            {
                return Json(new { error = "Department is required" });
            }

            var data = await _context.Employees
                .Where(e => e.Department.Name == department) 
                .GroupBy(e => new {e.DateJoined.Year, e.DateJoined.Month}) 
                .Select(g => new
                {
                    month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM, yyyy"), 
                    count = g.Count() 
                })
                .ToListAsync();

            return Json(data);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}