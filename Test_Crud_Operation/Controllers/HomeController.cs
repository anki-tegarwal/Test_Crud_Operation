using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Test_Crud_Operation.Models;
using Test_Crud_Operation.Models.ViewModel;

namespace Test_Crud_Operation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var random = new Random();
            var departments = new List<string>
    {
        "Technology",
        "Sales",
        "Marketing",
        "Human Resource",
        "Research And Development",
        "Accounting",
        "Support",
        "Logistics"
    };
            var months = new List<string>
    {
        "January","February","March","April","May","June","July","August","September","October","November","December"
    };

            var model = new List<ReportViewModel>();

            foreach (var dept in departments)
            {
                foreach (var month in months)
                {
                    model.Add(new ReportViewModel
                    {
                        DimensionOne = dept,
                        Quantity = random.Next(0, 6),
                        Month = month
                    });
                }
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}