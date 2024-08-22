using Microsoft.EntityFrameworkCore;
using Test_Crud_Operation.Models;

namespace Test_Crud_Operation.Data
{
    public class EmployeeDb:DbContext
    {
        public EmployeeDb(DbContextOptions<EmployeeDb>options):base(options)
        {
        }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}
