using DapperTask.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using Dapper;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data.Common;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;


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
        public JsonResult AddDepartment(string DepartmentName)
        {
            // Server-side validation
            if (string.IsNullOrEmpty(DepartmentName))
            {
                return Json(new { success = false, message = "Department name cannot be empty." });
            }

            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Check if the department name already exists
                string checkQuery = "SELECT COUNT(*) FROM Departments WHERE DepartmentName = @DepartmentName";
                var exists = db.ExecuteScalar<int>(checkQuery, new { DepartmentName }) > 0;

                if (exists)
                {
                    return Json(new { success = false, message = "Department name already exists." });
                }

                // Insert new department
                string insertQuery = "INSERT INTO Departments (DepartmentName) VALUES (@DepartmentName); SELECT CAST(SCOPE_IDENTITY() AS int)";
                var newDepartmentId = db.QuerySingle<int>(insertQuery, new { DepartmentName });

                return Json(new { success = true, departmentId = newDepartmentId, departmentName = DepartmentName });
            }
        }

        [HttpPost]
        public JsonResult UpdateDepartment(int DepartmentId, string DepartmentName)
        {
            // Server-side validation
            if (string.IsNullOrEmpty(DepartmentName))
            {
                return Json(new { success = false, message = "Department name cannot be empty." });
            }

            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Check if the department name already exists (excluding the current department)
                string checkQuery = "SELECT COUNT(*) FROM Departments WHERE DepartmentName = @DepartmentName AND DepartmentId != @DepartmentId";
                var exists = db.ExecuteScalar<int>(checkQuery, new { DepartmentName, DepartmentId }) > 0;

                if (exists)
                {
                    return Json(new { success = false, message = "Department name already exists." });
                }

                // Update the department name
                string updateQuery = "UPDATE Departments SET DepartmentName = @DepartmentName WHERE DepartmentId = @DepartmentId";
                db.Execute(updateQuery, new { DepartmentId, DepartmentName });

                return Json(new { success = true });
            }
        }


        [HttpPost]
        public JsonResult DeleteDepartment(int DepartmentId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM Departments WHERE DepartmentId = @DepartmentId";
                db.Execute(deleteQuery, new { DepartmentId });

                return Json(new { success = true });
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
        public JsonResult AddPosition(Positions position)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"
                INSERT INTO Positions (PositionTitle) 
                VALUES (@PositionTitle);
                SELECT CAST(SCOPE_IDENTITY() as int);"; // Get the new position ID

                var positionId = db.ExecuteScalar<int>(insertQuery, position); // Insert and get the new ID

                return Json(new { success = true, positionId = positionId });
            }
        }


        [HttpPost]
        public JsonResult UpdatePosition(int positionId, string positionName)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
                UPDATE Positions 
                SET PositionTitle = @PositionName
                WHERE PositionId = @PositionId";

                db.Execute(updateQuery, new { PositionId = positionId, PositionName = positionName });

                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult DeletePosition(int positionId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM Positions WHERE PositionId = @PositionId";
                db.Execute(deleteQuery, new { PositionId = positionId });

                return Json(new { success = true });
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
        public IActionResult PositionMapping()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Fetch departments
                string organizationQuery = "SELECT OrganizationId, OrganizationName FROM Organizations";
                var organizations = db.Query<Organizations>(organizationQuery).ToList(); 

                string departmentQuery = "SELECT DepartmentId, DepartmentName FROM Departments";
                var departments = db.Query<Departments>(departmentQuery).ToList();

                // Fetch positions
                string positionQuery = "SELECT PositionId, PositionTitle FROM Positions";
                var positions = db.Query<Positions>(positionQuery).ToList();

                var mappings = db.Query<PositionMappingViewModel>(@"
                    SELECT pm.PostMapId, pm.OrganizationId, o.OrganizationName, pm.DepartmentId, d.DepartmentName, pm.PositionId, p.PositionTitle
                    FROM PositionMapping pm
                    JOIN Organizations o ON pm.OrganizationId = o.OrganizationId
                    JOIN Departments d ON pm.DepartmentId = d.DepartmentId
                    JOIN Positions p ON pm.PositionId = p.PositionId").ToList();

                ViewBag.Organizations = organizations;
                ViewBag.Departments = departments;
                ViewBag.Positions = positions;
                ViewBag.Mappings = mappings;
                

                return View();
            }
        }

        [HttpPost]
        public IActionResult AddPositionMapping(PositionMappingViewModel model)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string insertQuery = @"
            INSERT INTO PositionMappings (OrganizationId, DepartmentId, PositionId) 
            VALUES (@OrganizationId, @DepartmentId, @PositionId)";

                db.Execute(insertQuery, model);
            }

            return RedirectToAction("PositionMapping");
        }

        [HttpPost]
        public IActionResult UpdatePositionMapping(PositionMappingViewModel model)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
            UPDATE PositionMappings
            SET OrganizationId = @OrganizationId, DepartmentId = @DepartmentId, PositionId = @PositionId
            WHERE PostMapId = @PostMapId";

                db.Execute(updateQuery, model);
            }

            return Json(new { success = true });
        }
        // POST: Update Position Mapping
        [HttpPost]
        public JsonResult UpdatePositionMapping(PositionMapping positionMapping)
        {
            if (positionMapping == null || positionMapping.PostMapId == 0)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string updateQuery = @"
                    UPDATE PositionMappings 
                    SET OrganizationId = @OrganizationId, DepartmentId = @DepartmentId, PositionId = @PositionId 
                    WHERE PostMapId = @PostMapId";

                db.Execute(updateQuery, positionMapping);
                return Json(new { success = true });
            }
        }

        // POST: Delete Position Mapping
        [HttpPost]
        public JsonResult DeletePositionMapping(int postMapId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM PositionMappings WHERE PostMapId = @PostMapId";
                db.Execute(deleteQuery, new { PostMapId = postMapId });
                return Json(new { success = true });
            }
        }


        
        public async Task<IActionResult> Filter()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var organizations = await db.QueryAsync<Organizations>("SELECT * FROM Organizations");
                var departments = await db.QueryAsync<Departments>("SELECT * FROM Departments");
                var positions = await db.QueryAsync<Positions>("SELECT * FROM Positions");

                // Pass data to the view using ViewBag
                ViewBag.Organizations = new SelectList(organizations, "OrganizationId", "OrganizationName");
                ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");
                ViewBag.Positions = new SelectList(positions, "PositionId", "PositionTitle");

                return View();
            }
        }

        // Action to filter employees based on selected criteria
        [HttpGet]
        public async Task<IActionResult> FilterEmployees(int? selectedOrganization, int? selectedDepartment, int? selectedPosition)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Join Employees with Organizations, Departments, and Positions to get names
                var sql = @"SELECT e.EmployeeId, e.EmployeeName, e.Salary, e.Phone, e.Email, 
                           o.OrganizationName, d.DepartmentName, p.PositionTitle
                    FROM Employees e
                    LEFT JOIN Organizations o ON e.OrganizationId = o.OrganizationId
                    LEFT JOIN Departments d ON e.DepartmentId = d.DepartmentId
                    LEFT JOIN Positions p ON e.PositionId = p.PositionId
                    WHERE 1 = 1";

                // Adding conditions based on selected filters
                var parameters = new DynamicParameters();

                if (selectedOrganization.HasValue)
                {
                    sql += " AND e.OrganizationId = @OrganizationId";
                    parameters.Add("OrganizationId", selectedOrganization);
                }

                if (selectedDepartment.HasValue)
                {
                    sql += " AND e.DepartmentId = @DepartmentId";
                    parameters.Add("DepartmentId", selectedDepartment);
                }

                if (selectedPosition.HasValue)
                {
                    sql += " AND e.PositionId = @PositionId";
                    parameters.Add("PositionId", selectedPosition);
                }

                var employees = await db.QueryAsync<EmployeeViewModel>(sql, parameters);
                return Json(employees);
            }
        }


        // Action to get departments by organization
        [HttpGet]
        public async Task<IActionResult> GetDepartmentsByOrganization(int organizationId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var departments = await db.QueryAsync<Departments>(
                    "SELECT * FROM Departments WHERE OrganizationId = @OrganizationId",
                    new { OrganizationId = organizationId });

                return Json(departments);
            }
        }

        // Action to get positions by department
        [HttpGet]
        public async Task<IActionResult> GetPositionsByDepartment(int departmentId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var positions = await db.QueryAsync<Positions>(
                    "SELECT * FROM Positions WHERE DepartmentId = @DepartmentId",
                    new { DepartmentId = departmentId });

                return Json(positions);
            }
        }
    



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}