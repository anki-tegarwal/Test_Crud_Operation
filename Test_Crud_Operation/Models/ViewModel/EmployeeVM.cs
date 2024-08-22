using Microsoft.AspNetCore.Mvc.Rendering;

namespace Test_Crud_Operation.Models.ViewModel
{
    public class EmployeeVM
    {
        public Employee Employee { get; set; }
        public IEnumerable<SelectListItem> DepartmentList { get; set; }
    }
}
