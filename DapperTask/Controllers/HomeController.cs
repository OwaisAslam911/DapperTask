using DapperTask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.Common;


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

       
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Organizations";
                var items = db.Query<Organizations>(query);
                return View(items);
            }
        }
        [HttpPost]
        public IActionResult Index(Organizations organization)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Insert new organization into the database
                string insertQuery = @"
            INSERT INTO Organizations (OrganizationName, FoundedDate, Phone, Location) 
            VALUES (@OrganizationName, @FoundedDate, @Phone, @Location)";

                // Execute the insert query
                db.Execute(insertQuery, organization);

                // After insertion, fetch all organizations again and return to view
                string fetchQuery = "SELECT * FROM Organizations";
                var organizations = db.Query<Organizations>(fetchQuery).ToList();

                // Return updated list to the view
                return View(organizations);
            }
        }
        [HttpPost]
        public IActionResult UpdateOrganization(int OrganizationId, string OrganizationName, DateTime FoundedDate, string Phone, string Location)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
            UPDATE Organizations 
            SET OrganizationName = @OrganizationName, FoundedDate = @FoundedDate, Phone = @Phone, Location = @Location
            WHERE OrganizationId = @OrganizationId";

                db.Execute(updateQuery, new { OrganizationId, OrganizationName, FoundedDate, Phone, Location });

                // Return the updated list of organizations
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult DeleteOrganization(int OrganizationId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = @"DELETE FROM Organizations WHERE OrganizationId = @OrganizationId";

                // Execute the delete query
                db.Execute(deleteQuery, new { OrganizationId });

                // After deletion, return to the Index action to refresh the list
                return RedirectToAction("Index");
            }

        }


        [HttpGet]
        public IActionResult Departments()
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT DepartmentId, DepartmentName FROM Departments";
                var departments = db.Query<DapperTask.Models.DepartmentViewModel>(query).ToList();

                return View(departments); // Pass DepartmentViewModel to the view
            }
        }
        [HttpPost]
        public IActionResult Departments(DepartmentViewModel department)
        {
            if (ModelState.IsValid)
            {
                var connectionString = configuration.GetConnectionString("dbcs");

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    // Insert the new department into the Departments table
                    string insertQuery = @"
                INSERT INTO Departments (DepartmentName) 
                VALUES (@DepartmentName)";

                    // Execute the insert query with the department name
                    db.Execute(insertQuery, department);
                }

                // Redirect to the Departments page after successful submission
                return RedirectToAction("Departments");
            }
            return View(department);

        }

        [HttpPost]
        public IActionResult UpdateDepartment(int DepartmentId, string DepartmentName)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
            UPDATE Departments
            SET DepartmentName = @DepartmentName
            WHERE DepartmentId = @DepartmentId";

                db.Execute(updateQuery, new { DepartmentId, DepartmentName });

                // After updating, return to the list of departments
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public IActionResult DeleteDepartment(int DepartmentId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = @"DELETE FROM Departments WHERE DepartmentId = @DepartmentId";

                // Execute the delete query
                db.Execute(deleteQuery, new { DepartmentId });

                // After deletion, return to the Index action to refresh the list
                return RedirectToAction("Departments");
            }

        }


        public IActionResult Positions()
        {

            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT PositionId, PositionTitle FROM Positions";
                var positions = db.Query<DapperTask.Models.PositionsViewModel>(query).ToList();

                return View(positions); 
            }
        }
        [HttpPost]
        public IActionResult Positions(PositionsViewModel positions)
        {

            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"
                INSERT INTO Positions (PositionTitle) 
                VALUES (@PositionTitle)";

               
                db.Execute(insertQuery, positions);
            }
            return RedirectToAction("Positions");
        }

        [HttpPost]
        public IActionResult UpdatePosition(int PositionId, string PositionTitle)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
            UPDATE Positions
            SET PositionTitle = @PositionTitle
            WHERE PositionId = @PositionId";

                db.Execute(updateQuery, new { PositionId, PositionTitle });

                // After updating, return to the list of positions
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public IActionResult DeletePosition(int PositionId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = @"DELETE FROM Positions WHERE PositionId = @PositionId";

                
                db.Execute(deleteQuery, new { PositionId });
                return RedirectToAction("Positions");
            }

        }
        [HttpGet]
        public IActionResult Employees()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string query = @"
            SELECT e.EmployeeId, e.EmployeeName, e.Email, e.Phone, e.Salary,
                   p.PositionTitle, d.DepartmentName, o.OrganizationName 
            FROM Employees e
            INNER JOIN Positions p ON e.PositionId = p.PositionId
            INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
            INNER JOIN Organizations o ON e.OrganizationId = o.OrganizationId";
                var items = db.Query<EmployeeViewModel>(query);

                ViewBag.Positions = db.Query<Positions>("SELECT * FROM Positions").ToList();
                ViewBag.Departments = db.Query<Departments>("SELECT * FROM Departments").ToList();
                ViewBag.Organizations = db.Query<Organizations>("SELECT * FROM Organizations").ToList();

                return View(items);
            }
        }
        [HttpPost]
        public IActionResult Employees(EmployeeViewModel emp)
        {
            if (ModelState.IsValid) { 

            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                    
             string insertQuery = @"
            INSERT INTO Employees (EmployeeName, Phone, Email, PositionId, OrganizationId, DepartmentId, Address, Salary)
            VALUES (@EmployeeName, @Phone, @Email, @PositionId, @OrganizationId, @DepartmentId, @Address, @Salary)";
                    db.Execute(insertQuery, emp);
            }
        
        }
            return RedirectToAction("Employees");

        }

        [HttpPost]
        public IActionResult UpdateEmployee(int EmployeeId, string EmployeeName, string Email, string Phone, decimal Salary, int PositionId, int DepartmentId, int OrganizationId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
            UPDATE Employees
            SET 
                EmployeeName = @EmployeeName, 
                Email = @Email, 
                Phone = @Phone, 
                Salary = @Salary, 
                PositionId = @PositionId, 
                DepartmentId = @DepartmentId, 
                OrganizationId = @OrganizationId
            WHERE EmployeeId = @EmployeeId";

                db.Execute(updateQuery, new
                {
                    EmployeeId,
                    EmployeeName,
                    Email,
                    Phone,
                    Salary,
                    PositionId,
                    DepartmentId,
                    OrganizationId
                });


                // After updating, return to the employee list
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public IActionResult DeleteEmployee(int EmployeeId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = @"DELETE FROM Employees WHERE EmployeeId = @EmployeeId";


                db.Execute(deleteQuery, new { EmployeeId });
                return RedirectToAction("Employees");
            }

        }



        [HttpGet]
        public IActionResult OrganizationMapping()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var organizations = db.Query<Organizations>("SELECT * FROM Organizations").ToList();
                var departments = db.Query<Departments>("SELECT * FROM Departments").ToList();
                var mappings = db.Query<OrganizationMapping, Organizations, Departments, OrganizationMapping>(
    @"SELECT om.OrganizationId, om.DepartmentId, 
             o.OrganizationId AS OrgId, o.OrganizationName, 
             d.DepartmentId AS DeptId, d.DepartmentName 
      FROM OrganizationMapping om
      INNER JOIN Organizations o ON om.OrganizationId = o.OrganizationId
      INNER JOIN Departments d ON om.DepartmentId = d.DepartmentId",
    (om, org, dep) =>
    {
        om.Organizations = org;
        om.Departments = dep;
        return om;
    },splitOn: "OrgId,DeptId").ToList();
                ViewBag.Organizations = organizations;
                ViewBag.Departments = departments;
                return View(mappings);
            }
        }

        [HttpPost]
        public IActionResult OrganizationMapping(int OrganizationId, int DepartmentId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO OrganizationMapping (OrganizationId, DepartmentId) VALUES (@OrganizationId, @DepartmentId)";
                db.Execute(insertQuery, new { OrganizationId, DepartmentId });
            }

            return RedirectToAction("OrganizationMapping");
        }


        [HttpGet]
        public IActionResult PositionMapping()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Fetch departments
                string departmentQuery = "SELECT DepartmentId, DepartmentName FROM Departments";
                var departments = db.Query<Departments>(departmentQuery).ToList();

                // Fetch positions
                string positionQuery = "SELECT PositionId, PositionTitle FROM Positions";
                var positions = db.Query<Positions>(positionQuery).ToList();

                // Pass both lists to the view
                ViewBag.Departments = departments;
                ViewBag.Positions = positions;

                // You may also want to load existing mappings to display under the form
                string mappingQuery = @"
            SELECT d.DepartmentName, p.PositionTitle
            FROM PositionMapping m
            INNER JOIN Departments d ON m.DepartmentId = d.DepartmentId
            INNER JOIN Positions p ON m.PositionId = p.PositionId";
                var mappings = db.Query(mappingQuery).ToList();
                ViewBag.Mappings = mappings;

                return View();
            }
        }

        [HttpPost]
        public IActionResult PositionMapping(PositionMapping mapping)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"
            INSERT INTO PositionMapping (DepartmentId, PositionId) 
            VALUES (@DepartmentId, @PositionId)";

                db.Execute(insertQuery, mapping);
            }

            // Redirect to reload the view with updated data
            return RedirectToAction("PositionMapping");
        }

        public async Task<IActionResult> Filter()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                var organizations = await db.QueryAsync<Organizations>("SELECT * FROM Organizations");
                var departments = await db.QueryAsync<Departments>("SELECT * FROM Departments");
                var positions = await db.QueryAsync<Positions>("SELECT * FROM Positions");

                var viewModel = new FilterViewModel
                {
                    Organizations = organizations,
                    Departments = departments,
                    Positions = positions
                };
                return View(viewModel);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Filter(FilterViewModel filter)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sql = @"SELECT e.EmployeeId, e.EmployeeName, e.Salary, e.Phone, e.Email, 
                       o.OrganizationName, d.DepartmentName, p.PositionTitle
                       FROM Employees e
                       JOIN Organizations o ON e.OrganizationId = o.OrganizationId
                       JOIN Departments d ON e.DepartmentId = d.DepartmentId
                       JOIN Positions p ON e.PositionId = p.PositionId
                       WHERE 1 = 1"; // Base query

                var parameters = new DynamicParameters();

                // Add conditionally to SQL query based on filter values
                if (filter.SelectedOrganization.HasValue)
                {
                    sql += " AND e.OrganizationId = @OrganizationId";
                    parameters.Add("OrganizationId", filter.SelectedOrganization.Value);
                }

                if (filter.SelectedDepartment.HasValue)
                {
                    sql += " AND e.DepartmentId = @DepartmentId";
                    parameters.Add("DepartmentId", filter.SelectedDepartment.Value);
                }

                if (filter.SelectedPosition.HasValue)
                {
                    sql += " AND e.PositionId = @PositionId";
                    parameters.Add("PositionId", filter.SelectedPosition.Value);
                }

                // Execute query and get filtered employee data
                var employees = await db.QueryAsync<EmployeeViewModel>(sql, parameters);

                // Initialize Employees as an empty list if no data
                var employeesList = employees?.ToList() ?? new List<EmployeeViewModel>();

                // Get updated lists for dropdowns (Organizations, Departments, Positions)
                var organizations = await db.QueryAsync<Organizations>("SELECT * FROM Organizations");
                var departments = await db.QueryAsync<Departments>("SELECT * FROM Departments");
                var positions = await db.QueryAsync<Positions>("SELECT * FROM Positions");

                // Populate the view model with the filtered data
                var viewModel = new FilterViewModel
                {
                    SelectedOrganization = filter.SelectedOrganization,
                    SelectedDepartment = filter.SelectedDepartment,
                    SelectedPosition = filter.SelectedPosition,
                    Organizations = organizations,
                    Departments = departments,
                    Positions = positions,
                    Employees = employeesList // Ensure employees are always initialized
                };

                return View("Index", viewModel);
            }
        }


        [HttpGet]
        public IActionResult GetDepartmentsByOrganization(int organizationId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Query for departments based on organizationId
                var departments = db.Query<Departments>(
                    "SELECT * FROM Departments WHERE OrganizationId = @OrganizationId",
                    new { OrganizationId = organizationId }).ToList();

                return Json(departments); // Return departments as JSON
            }
        }

        [HttpGet]
        public IActionResult GetPositionsByDepartment(int departmentId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var positions = db.Query<Positions>(
                    "SELECT * FROM Positions WHERE DepartmentId = @DepartmentId",
                    new { DepartmentId = departmentId }).ToList();

                return Json(positions); // Return positions as JSON
            }
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}