using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Migrations;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;
using Test_Crud_Operation.service.Iservice;

namespace Test_Crud_Operation.service
{
    public class AuthService : IAuthService
    {
        private readonly EmployeeDb _employeeDb;
        private readonly IConfiguration _configuration;
        private readonly JwtToken _token;
        private readonly IjwtUtils _utils;

        public AuthService(EmployeeDb employeeDb, IConfiguration configuration, IOptions<JwtToken> tokenOptions,IjwtUtils utils)
        {
            _employeeDb = employeeDb;
            _configuration = configuration;
            _token = tokenOptions.Value;
            _utils = utils;
        }

       

        public async Task<ApplicationUser> Login(LoginViewModel loginViewModel)
        {
            var user = await _employeeDb.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == loginViewModel.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginViewModel.Password, user.Password))
            {
                throw new Exception("Invalid credentials.");
            }

            // Generate JWT token
            var token = _utils.GenerateToken(user);
            user.Token = token;

            return user;
        }
      



        public async Task<ApplicationUser> Register(RegisterViewModel registerViewModel)
        {
            if (await _employeeDb.ApplicationUsers.AnyAsync(u => u.Email == registerViewModel.Email))
            {
                throw new Exception("User with this email already exists.");
            }

            var user = new ApplicationUser
            {
                Name = registerViewModel.Name,
                Email = registerViewModel.Email,
                PhoneNumber = registerViewModel.PhoneNumber,
                Password = BCrypt.Net.BCrypt.HashPassword(registerViewModel.Password) // Hash the password
            };

            _employeeDb.ApplicationUsers.Add(user);
            await _employeeDb.SaveChangesAsync();

            return user;
        }
        

       

        public async Task<ApplicationUser> GetById(int id)
        {
            return await _employeeDb.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public string Authenticate(string username, string password)
        {
            var user = _employeeDb.ApplicationUsers.SingleOrDefault(x => x.Email == username && x.Password == password);
            if (user == null)
                return null;

            // Generate JWT token
            return _utils.GenerateToken(user);
        }
    }
}
