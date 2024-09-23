using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;
using Test_Crud_Operation.service.Iservice;

namespace Test_Crud_Operation.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IempService _iempService;
        private readonly EmployeeDb _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(IempService iempService, EmployeeDb employeeDb, IWebHostEnvironment webHostEnvironment)
        {
            _iempService = iempService;
            _context = employeeDb;
            _hostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var employeeList = _context.Employees.Include(e => e.Department).Include(e=>e.Images).ToList();
            return View(employeeList);
        }

        public IActionResult Upsert(int? id)
        {
            var employeeVM = new EmployeeVM()
            {
                Employee = new Employee(),
                DepartmentList = _context.Departments.Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                }).ToList()
            };

            if (id == null)
                return View(employeeVM); // Create view with EmployeeVM

            // Edit
            var employee = _iempService.GetEmployee(id.GetValueOrDefault());
            if (employee == null)
                return NotFound();
            employee.Images = _context.EmployeeImages.Where(ei => ei.EmployeeId == employee.Id).ToList();

            employeeVM.Employee = employee; // Assign the existing employee to the VM
            return View(employeeVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(EmployeeVM employeeVM)
        {
            if (employeeVM.Employee == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                // Ensure DepartmentList is populated in case of errors
                employeeVM.DepartmentList = _context.Departments.Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                }).ToList();
                return View(employeeVM);
            }

            // Check for duplicates
            if (_iempService.IsDuplicateEmployee(employeeVM.Employee.Name, employeeVM.Employee.Id))
            {
                ModelState.AddModelError("", "The name is already saved.");
                employeeVM.DepartmentList = _context.Departments.Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.Id.ToString(),
                }).ToList();
                return View(employeeVM);
            }

            if (employeeVM.Employee.Id == 0)
            {
                _iempService.CreateEmployee(employeeVM.Employee);
            }
            else
            {
                _iempService.UpdateEmployee(employeeVM.Employee);
            }

            _context.SaveChanges();

            //var imageToDelete =   HttpContext.Request.Form["deleteImages"].ToList();
            //if (!string.IsNullOrEmpty(deleteImages))
            //{
            //    var deleteImageIds = deleteImages.Split(',').Select(id => int.Parse(id)).ToList();
            //    foreach (var imageId in deleteImageIds)
            //    {
            //        var image = _context.EmployeeImages.FirstOrDefault(e => e.Id == imageId);
            //        if (image != null)
            //        {
            //            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
            //            if (System.IO.File.Exists(imagePath))
            //            {
            //                System.IO.File.Delete(imagePath);
            //            }
            //            _context.EmployeeImages.Remove(image);
            //        }
            //    }
            //    _context.SaveChanges();
            //}

            var webRootPath = _hostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var uploads = Path.Combine(webRootPath, "images/employee");
            if (files.Count > 0)
            {
                //Remove existing images if editing an employee
                if (employeeVM.Employee.Id != 0)
                {
                    var existingImages = _context.EmployeeImages.Where(ei => ei.EmployeeId == employeeVM.Employee.Id).ToList();
                    foreach (var image in existingImages)
                    {
                        var imagePath = Path.Combine(webRootPath, image.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        _context.EmployeeImages.Remove(image);
                    }
                    _context.SaveChanges();
                }

                foreach (var file in files)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(file.FileName);
                    var filePath = Path.Combine(uploads, fileName + extension);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    var imageUrl = Path.Combine("images/employee", fileName + extension);
                    var employeeImage = new EmployeeImage
                    {
                        EmployeeId = employeeVM.Employee.Id,
                        ImageUrl = imageUrl
                    };

                    _context.EmployeeImages.Add(employeeImage);
                }
            }
         


            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        public IActionResult DeleteImage(int id)
        {
            var image = _context.EmployeeImages.Find(id);
            if (image == null)
                return Json(new { success = false });
            _context.EmployeeImages.Remove(image);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        public IActionResult Delete(int id)
        {
            var employeeInDb = _iempService.GetEmployee(id);
            if (employeeInDb == null)
                return NotFound();
            var employeeImages = _context.EmployeeImages.Where(ei => ei.EmployeeId == employeeInDb.Id).ToList();

            foreach (var image in employeeImages)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('\\'));
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);   
                }
            }
            _context.EmployeeImages.RemoveRange(employeeImages);
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
