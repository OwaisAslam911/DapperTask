
using DapperTask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;


namespace DapperTask.Controllers
{
    public class HomeController : Controller
    {
        private ILogger<HomeController> _logger;
        private readonly IConfiguration configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            // Check if the connection string is retrieved successfully
            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, "Connection string is not initialized.");
            }
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Organizations";
                var items = db.Query<Organizations>(query);
                return View(items);
            }
        }
        public IActionResult Department()
        {
          
            return View();
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
