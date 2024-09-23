using Test_Crud_Operation.Migrations;

namespace Test_Crud_Operation.service.Iservice
{
    public interface IjwtUtils
    {
        public string GenerateToken(Models.ApplicationUser user);
        public int? ValidateToken(string token);
    }
}
