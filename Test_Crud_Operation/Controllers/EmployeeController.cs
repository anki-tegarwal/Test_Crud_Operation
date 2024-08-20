using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.service.Iservice;

namespace Test_Crud_Operation.Controllers
{
    
    public class EmployeeController : Controller
    {
        private readonly IempService _iempService;
        public EmployeeController(IempService iempService)
        {
            _iempService = iempService;
        }

        public IActionResult Index()
        {
            var employeeList = _iempService.GetEmployees();
            return View(employeeList);
        }

        public IActionResult Upsert(int? id)
        {
            Employee employee = new Employee();
            if (id == null) return View(employee);//create
            //edit
            employee = _iempService.GetEmployee(id.GetValueOrDefault());
            if (employee == null) return NotFound();
            return View(employee);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Employee employee)
        {
            if (employee == null) return NotFound();
            // Check for duplicates
            if (_iempService.IsDuplicateEmployee(employee.Name,  employee.Id))
            {
                ModelState.AddModelError("", "The name is already saved.");
                return View(employee);
            }
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }
                    }
                    employee.imageUrl = p1;
                }
                else
                {
                    var employeeInDb = _iempService.GetEmployee(employee.Id);
                    employee.imageUrl = employeeInDb.imageUrl;
                }
            }
            if (!ModelState.IsValid) return View(employee);
            if (employee.Id == 0)
                _iempService.CreateEmployee(employee);
            else
                _iempService.UpdateEmployee(employee);
            _iempService.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int id)
        {
            var employeeInDb = _iempService.GetEmployee(id);
            if (employeeInDb == null) return NotFound();
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
