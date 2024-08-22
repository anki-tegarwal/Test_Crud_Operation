using Microsoft.AspNetCore.Mvc;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Models;

namespace Test_Crud_Operation.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly EmployeeDb _employeeDb;
        public DepartmentController(EmployeeDb employeeDb)
        {
            _employeeDb = employeeDb;
        }

        public IActionResult Index()
        {
            var departments = _employeeDb.Departments.ToList();
            return View(departments);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Department department)
        {
            if (!ModelState.IsValid) return View(department);
            _employeeDb.Add(department);
            _employeeDb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
