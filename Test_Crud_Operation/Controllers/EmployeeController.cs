using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;
using Test_Crud_Operation.service.Iservice;

namespace Test_Crud_Operation.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IempService _iempService;
        private readonly EmployeeDb _context;

        public EmployeeController(IempService iempService, EmployeeDb employeeDb)
        {
            _iempService = iempService;
            _context = employeeDb;
        }

        public IActionResult Index()
        {
            var employeeList = _context.Employees.Include(e=>e.Department).ToList();
            return View(employeeList);
        }

        public IActionResult Upsert(int? id)
        {
            EmployeeVM employeeVM = new EmployeeVM()
            {
                Employee = new Employee(),
                DepartmentList = _context.Departments.Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                }).ToList()  // Added ToList() to finalize the query
            };

            if (id == null)
                return View(employeeVM); //create view with EmployeeVM

            //edit
            var employee = _iempService.GetEmployee(id.GetValueOrDefault());
            if (employee == null)
                return NotFound();

            employeeVM.Employee = employee; // Assign the existing employee to the VM
            return View(employeeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(EmployeeVM employeeVM)
        {
            if (employeeVM.Employee == null)
                return NotFound();

            // Check for duplicates
            if (_iempService.IsDuplicateEmployee(employeeVM.Employee.Name, employeeVM.Employee.Id))
            {
                ModelState.AddModelError("", "The name is already saved.");
                return View(employeeVM);
            }

            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    employeeVM.Employee.imageUrl = p1;
                }
                else
                {
                    var employeeInDb = _iempService.GetEmployee(employeeVM.Employee.Id);
                    if (employeeInDb != null)
                    {
                        employeeVM.Employee.imageUrl = employeeInDb.imageUrl;
                    }
                }

                if (employeeVM.Employee.Id == 0)
                {
                    employeeVM.Employee.DateJoined = DateTime.UtcNow;
                    _iempService.CreateEmployee(employeeVM.Employee);
                }
                else
                    _iempService.UpdateEmployee(employeeVM.Employee);

                _iempService.Save();
                return RedirectToAction(nameof(Index));
            }

            // If ModelState is invalid, return the view with the current VM
            employeeVM.DepartmentList = _context.Departments.Select(cl => new SelectListItem()
            {
                Text = cl.Name,
                Value = cl.Id.ToString(),
            }).ToList(); // Ensure DepartmentList is populated in case of errors

            return View(employeeVM);
        }

        public IActionResult Delete(int id)
        {
            var employeeInDb = _iempService.GetEmployee(id);
            if (employeeInDb == null)
                return NotFound();

            employeeInDb.IsDeleted = true;
            _iempService.DeleteEmployee(employeeInDb);
            _iempService.Save();
            return RedirectToAction(nameof(Index));
        }

        public void UndeleteEmployee(int id)
        {
            var employee = _iempService.GetEmployee(id);
            if (employee != null)
            {
                employee.IsDeleted = false;
                _iempService.Save();
            }
        }
    }
}
