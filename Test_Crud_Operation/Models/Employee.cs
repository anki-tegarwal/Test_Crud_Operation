using System.ComponentModel.DataAnnotations;

namespace Test_Crud_Operation.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public int Salary { get; set; }
        public bool IsDeleted { get; set; } = false;
        [Display(Name ="Picture")]
        public byte[]? imageUrl { get; set; }
    }
}
