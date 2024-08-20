using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Test_Crud_Operation.Data;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;
using Test_Crud_Operation.service.Iservice;

namespace Test_Crud_Operation.service
{
    public class AuthService : IAuthService
    {
        private readonly EmployeeDb _employeeDb;
        private readonly IConfiguration _configuration;

        public AuthService(EmployeeDb employeeDb, IConfiguration configuration)
        {
            _employeeDb = employeeDb;
            _configuration = configuration;
        }

      

        public async Task<ApplicationUser> Login(LoginViewModel loginViewModel)
        {
            var user = await _employeeDb.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == loginViewModel.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginViewModel.Password, user.Password))
            {
                throw new Exception("Invalid credentials.");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user);
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
        private string GenerateJwtToken(ApplicationUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtToken:secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
