using System.Net;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.service.Iservice;

namespace Test_Crud_Operation.service
{
    public class EmpService : IempService
    {
        private readonly EmployeeDb _context;
        public EmpService(EmployeeDb context)
        {
            _context = context;
        }

        public bool CreateEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            return Save();
        }

        public bool DeleteEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
            return Save();
        }

        public bool EmployeeExist(int EmployeeId)
        {
            return _context.Employees.Any(e => e.Id == EmployeeId);
        }

        public Employee GetEmployee(int EmployeeId)
        {
            return _context.Employees.Find(EmployeeId);

        }

        public ICollection<Employee> GetEmployees()
        {
            return _context.Employees.ToList();
        }

       

        public bool IsDuplicateEmployee(string name, int? id = null)
        {
            if (id == null)
            {
                return _context.Employees.Any(e => (e.Name == name) && !e.IsDeleted);
            }
            else
            {
                return _context.Employees.Any(e => (e.Name == name ) && e.Id != id && !e.IsDeleted);
            }
        }

        public bool Save()
        {
            return _context.SaveChanges() == 1 ? true : false;
        }

        public bool UpdateEmployee(Employee employee)
        {
            _context.ChangeTracker.Clear();
            _context.Employees.Update(employee);
            return Save();
        }
    }
}
