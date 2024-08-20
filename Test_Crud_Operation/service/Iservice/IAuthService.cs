using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;

namespace Test_Crud_Operation.service.Iservice
{
    public interface IAuthService
    {
        Task<ApplicationUser> Register(RegisterViewModel registerViewModel);
        Task<ApplicationUser> Login(LoginViewModel loginViewModel);
    }
}
