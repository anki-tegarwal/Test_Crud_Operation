using Test_Crud_Operation.Migrations;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;

namespace Test_Crud_Operation.service.Iservice
{
    public interface IAuthService
    {
        Task<ApplicationUser> Register(RegisterViewModel registerViewModel);
        Task<ApplicationUser> Login(LoginViewModel loginViewModel);
        Task<ApplicationUser> GetById(int id);
        string Authenticate(string username, string password);
    }
}
