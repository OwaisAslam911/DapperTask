using DapperTask.Repositories;
using DapperTask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace DapperTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly OrganizationRepository _organizationRepository;
        private readonly DepartmentRepository _departmentRepository;
        public HomeController(ILogger<HomeController> logger, OrganizationRepository organizationRepository,
            DepartmentRepository departmentRepository)
        {
            _logger = logger;
            _organizationRepository = organizationRepository;
            _departmentRepository = departmentRepository;
        }

        public async Task<IActionResult> Index()
        {
            var organizations = await _organizationRepository.GetAllAsync();  // Fetch organizations
            return View(organizations);  // Pass organizations to the view
        }
        public IActionResult Department(int departmentId)
        {
            var department = _organizationRepository.GetById(departmentId); // Retrieve a specific department
            var organizations = _organizationRepository.GetAllAsync(); // Retrieve all organizations

            var viewModel = new DepartmentViewModel
            {
                departments = department,
                organizations = (IEnumerable<Organizations>)organizations
            };

            return View(viewModel);
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
