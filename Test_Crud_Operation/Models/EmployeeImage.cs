namespace Test_Crud_Operation.Models
{
    public class EmployeeImage
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string ImageUrl { get; set; }
    }
}
