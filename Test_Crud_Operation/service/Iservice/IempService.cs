using Test_Crud_Operation.Models;

namespace Test_Crud_Operation.service.Iservice
{
    public interface IempService
    {

        ICollection<Employee> GetEmployees();
        bool IsDuplicateEmployee(string name,  int? id = null);
        Employee GetEmployee(int EmployeeId);
        bool EmployeeExist(int EmployeeId);
        bool CreateEmployee(Employee employee);
        bool UpdateEmployee(Employee employee);
        bool DeleteEmployee(Employee employee);
        bool Save();
    }
}
