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
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;




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

        

        public IActionResult Dashboard()
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Query to select the top 5 employees based on salary
                var Salaryquery = "SELECT TOP 5 EmployeeName, Salary FROM Employees ORDER BY Salary DESC;";
                var topEmployees = db.Query<DapperTask.Models.Employees>(Salaryquery).ToList();

                // Extract names and salaries to separate lists
                ViewBag.EmployeeNames = topEmployees.Select(e => e.EmployeeName).ToList();
                ViewBag.EmployeeSalaries = topEmployees.Select(e => e.Salary).ToList();
             


                
                var PositionQuery = "SELECT PositionId, COUNT(*) AS EmployeeCount FROM Employees GROUP BY PositionId;";
                var positionData = db.Query<dynamic>(PositionQuery).ToList();

                // Extract positions and employee counts into separate lists
                ViewBag.Positions = positionData.Select(p => (int)p.PositionId).ToList();
                ViewBag.EmployeeCounts = positionData.Select(p => (int)p.EmployeeCount).ToList();
                return View();

            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string query = "SELECT * from Organizations";
                var organizations = db.Query<DapperTask.Models.Organizations>(query).ToList();

                return View(organizations); // Pass DepartmentViewModel to the view
            }
        }
        [HttpPost]
        public JsonResult CreateOrganization(string OrganizationName, string FoundedDate, string Location, string Phone)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Check if the organization already exists
                string checkDeptQuery = "SELECT COUNT(*) FROM Organizations WHERE OrganizationName = @OrganizationName";
                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new { OrganizationName });

                if (existingCount > 0)
                {
                    // Return an error response if the organization already exists
                    return Json(new { success = false, message = "Organization already exists." });
                }

                // Prepare to insert the new organization
                string insertQuery = "INSERT INTO Organizations (OrganizationName, Phone, FoundedDate, Location) VALUES (@OrganizationName, @Phone, @FoundedDate, @Location); SELECT CAST(SCOPE_IDENTITY() AS int)";

                // Parse FoundedDate to DateTime
                DateTime foundedDateParsed;
                if (!DateTime.TryParse(FoundedDate, out foundedDateParsed))
                {
                    return Json(new { success = false, message = "Invalid Founded Date." });
                }

                // Insert the new organization and get its ID
                var newOrganizationId = db.QuerySingle<int>(insertQuery, new { OrganizationName, Phone, FoundedDate = foundedDateParsed, Location });

                // Return a success response with the new organization's details
                return Json(new { success = true, organizationId = newOrganizationId, organizationName = OrganizationName, phone = Phone, location = Location, foundedDate = foundedDateParsed.ToString("yyyy-MM-dd") });
            }
        }



        [HttpPost]
        public JsonResult UpdateOrganization(int OrganizationId, string OrganizationName, string Phone, DateTime FoundedDate, string Location)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string checkDeptQuery = "SELECT COUNT(*) FROM Organizations WHERE OrganizationName = @OrganizationName";
                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new { OrganizationName });

                if (existingCount > 0)
                {
                    // Return an error response if the organization already exists
                    return Json(new { success = false, message = "Organization already exists." });
                }
                // Construct your update SQL query here
                string updateQuery = "UPDATE Organizations SET OrganizationName = @OrganizationName, Phone = @Phone, FoundedDate = @FoundedDate, Location = @Location WHERE OrganizationId = @OrganizationId";

                var affectedRows = db.Execute(updateQuery, new { OrganizationId, OrganizationName, Phone, FoundedDate, Location });

                // Check if any rows were affected
                if (affectedRows > 0)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Organization not found or no changes made." });
                }
            }
        }

        public JsonResult deleteOrganization(int OrganizationId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString("dbcs");

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    //var check = "select * from PositionMapping Where OrganizationId = @OrganizationId";


                    string deleteQuery = "DELETE FROM Organizations WHERE OrganizationId = @OrganizationId";

                    db.Execute(deleteQuery, new { OrganizationId });

                    return Json(new { success = true });
                }
            }
            catch (SqlException x)
            {
                if (x.ErrorCode == 1452)
                {
                    return Json(new { success = false, message = "Organization is already in use" });
                }
                else { 
                return Json(new { success = false, message = x.Message });
                }
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
            if (string.IsNullOrEmpty(DepartmentName))
            {
                return Json(new { success = false, message = "Department name cannot be empty." });
            }

            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string checkDeptQuery = "SELECT COUNT(*) FROM Departments WHERE DepartmentName = @DepartmentName";
                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new { DepartmentName });

                if (existingCount > 0)
                {
                    // Return an error response if the department already exists
                    return Json(new { success = false, message = "Department already exists." });
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
            try
            {
                var connectionString = configuration.GetConnectionString("dbcs");

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string deleteQuery = "DELETE FROM Departments WHERE DepartmentId = @DepartmentId";
                    db.Execute(deleteQuery, new { DepartmentId });

                    return Json(new { success = true });
                }
            }
            catch (SqlException x)
            {
                if(x.ErrorCode == 1451)
                {
                    return Json(new { success = false, message ="THis department is in use " });
                }
                else
                {
                    return Json(new { success = false, message ="this department is in use" });

                }

                throw;
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
        public JsonResult AddPosition(string PositionTitle)
        {
            if (string.IsNullOrEmpty(PositionTitle))
            {
                return Json(new { success = false, message = "Position name cannot be empty." });
            }

            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {

                string checkDeptQuery = "SELECT COUNT(*) FROM Positions WHERE PositionTitle = @PositionTitle";
                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new { PositionTitle });

                if (existingCount > 0)
                {
                    // Return an error response if the department already exists
                    return Json(new { success = false, message = "Position already exists." });
                }

                // Insert new department
                string insertQuery = "INSERT INTO Positions (PositionTitle) VALUES (@PositionTitle); SELECT CAST(SCOPE_IDENTITY() AS int)";
                var newPositionId = db.QuerySingle<int>(insertQuery, new { PositionTitle });
                return Json(new { success = true, positionId = newPositionId, positionTitle = PositionTitle });

            }

        }


        [HttpPost]
        public JsonResult UpdatePosition(int positionId, string PositionTitle)
        {
            if (string.IsNullOrEmpty(PositionTitle))
            {
                return Json(new { success = false, message = "Position name cannot be empty." });
            }
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string checkDeptQuery = "SELECT COUNT(*) FROM Positions WHERE PositionTitle = @PositionTitle";
                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new { PositionTitle });

                if (existingCount > 0)
                {
                    // Return an error response if the department already exists
                    return Json(new { success = false, message = "Position already exists." });
                }


                string updateQuery = @"
                UPDATE Positions 
                SET PositionTitle = @PositionTitle
                WHERE PositionId = @PositionId";

                db.Execute(updateQuery, new { PositionId = positionId, PositionTitle = PositionTitle });

                return Json(new { success = true });
            }
        }

        [HttpPost]
        public JsonResult DeletePosition(int positionId)
        {
            try
            {
                var connectionString = configuration.GetConnectionString("dbcs");

                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    string deleteQuery = "DELETE FROM Positions WHERE PositionId = @PositionId";
                    db.Execute(deleteQuery, new { PositionId = positionId });

                    return Json(new { success = true });
                }
            }
            catch (SqlException x)
            {
                if(x.ErrorCode == 1451)
                {
                    return Json(new { success = false, message="This position is in use" });

                }
                else
                {

                    return Json(new { success = false, message="This position is in use" });
                }
                throw;
            }
      
        }
        public async Task<IActionResult> GetOrganizations()
        {
            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {
                string sql = "SELECT * FROM Organizations";
                var organizations = await db.QueryAsync<Organizations>(sql);
                ViewBag.Organizations = organizations;

                return View();
            }
        }

        [HttpGet]
        public JsonResult GetDepartments(int organizationId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Query to fetch departments based on the selected organization
                string query = @"
            SELECT d.DepartmentId, d.DepartmentName 
            FROM Departments d
            INNER JOIN PositionMapping pm ON d.DepartmentId = pm.DepartmentId
            WHERE pm.OrganizationId = @OrganizationId";

                var departments = db.Query<Departments>(query, new { OrganizationId = organizationId }).ToList();
                return Json(departments);
            }
        }


   [HttpGet]
public JsonResult GetPositions(int organizationId, int departmentId)
{
    var connectionString = configuration.GetConnectionString("dbcs");
    using (IDbConnection db = new SqlConnection(connectionString))
    {
        // Query to fetch positions based on the selected organization and department
        var query = @"
            SELECT p.PositionId, p.PositionTitle 
            FROM Positions p
            INNER JOIN PositionMapping pm ON p.PositionId = pm.PositionId
            WHERE pm.OrganizationId = @OrganizationId AND pm.DepartmentId = @DepartmentId";

        // Execute the query with the provided organizationId and departmentId
        var positions = db.Query<Positions>(query, new { OrganizationId = organizationId, DepartmentId = departmentId }).ToList();

        return Json(positions);
    }
}

        [HttpGet]
        public IActionResult GetEmployees()
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var query = db.Query<EmployeeViewModel>(
                         @"
                SELECT e.EmployeeId, e.EmployeeName, e.Phone, e.Email, e.Salary, e.PositionId , e.OrganizationId , e.DepartmentId,
                p.PositionTitle, d.DepartmentName, o.OrganizationName 
                FROM Employees e
                INNER JOIN Positions p ON e.PositionId = p.PositionId
                INNER JOIN Departments d ON e.DepartmentId = d.DepartmentId
                INNER JOIN Organizations o ON e.OrganizationId = o.OrganizationId").ToList();

                return Json(query);
            }
        }
        [HttpPost]
        public JsonResult AddEmployee(EmployeeViewModel addemp)
        {

            if (string.IsNullOrEmpty(addemp?.EmployeeName))
            {
                return Json(new { success = false, message = "Employee Name  cannot be empty." });
            }
            if (addemp.OrganizationId <= 0)
            {
                return Json(new { success = false, message = "Invalid Organization selected." });
            }

            // Validate DepartmentId
            if (addemp.DepartmentId <= 0)
            {
                return Json(new { success = false, message = "Invalid Department selected." });
            }

            // Validate PositionId
            if (addemp.PositionId <= 0)
            {
                return Json(new { success = false, message = "Invalid Position selected." });
            }


            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string checkDeptQuery = "SELECT COUNT(*) FROM Employees WHERE EmployeeName = @EmployeeName";

                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new
                {
                    EmployeeName = addemp.EmployeeName,
                  

                });

                if (existingCount > 0)
                {
                    // Return an error response if the department already exists
                    return Json(new { success = false, message = "Employee already exists." });
                }


                // Insert the new position mapping and get the inserted ID
                string insertQuery = @"
            INSERT INTO Employees (EmployeeName, Phone, Email, PositionId, OrganizationId, DepartmentId, Salary)
            VALUES (@EmployeeName, @Phone, @Email, @PositionId, @OrganizationId, @DepartmentId, @Salary)";


                //// Execute the insert and retrieve the new ID
                var newId = db.ExecuteScalar<int>(insertQuery, new
                {
                    employeeName = addemp.EmployeeName,
                    phone = addemp.Phone,
                    email = addemp.Email,
                    organizationId = addemp.OrganizationId,
                    departmentId = addemp.DepartmentId,
                    positionId = addemp.PositionId,
                    salary = addemp.Salary
                });



                return Json(new { success = true, emp = newId });
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

                //ViewBag.Positions = db.Query<Positions>("SELECT * FROM Positions").ToList();
                //ViewBag.Departments = db.Query<Departments>("SELECT * FROM Departments").ToList();
                ViewBag.Organizations = db.Query<Organizations>("SELECT * FROM Organizations").ToList();

                return View(items);
            }


        }





        [HttpPost]
        public JsonResult UpdateEmployee(int EmployeeId, string EmployeeName, string Email, string Phone, decimal Salary, int PositionId, int DepartmentId, int OrganizationId)
        {
            if (string.IsNullOrEmpty(EmployeeName))
            {
                return Json(new { success = false, message = "emp name cannot be empty." });
            }
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

                return Json(new { success = true });


            }
        }


        [HttpPost]
        public JsonResult DeleteEmployee(int employeeId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string deleteQuery = "DELETE FROM Employees WHERE EmployeeId = @EmployeeId";
                db.Execute(deleteQuery, new { EmployeeId = employeeId });

                return Json(new { success = true });
            }
        }



        

        public IActionResult PositionMapping()
        {
            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {

                var getOrganizations = db.Query<Organizations>("Select * from Organizations").ToList();
                ViewBag.OrganizationMapping = new SelectList(getOrganizations, "OrganizationId", "OrganizationName");

                var getDepartment = db.Query<Departments>("Select * from Departments").ToList();
                ViewBag.DepartmentMapping = new SelectList(getDepartment, "DepartmentId", "DepartmentName");

                var getPosition = db.Query<Positions>("Select * from Positions").ToList();
                ViewBag.PositionMapping = new SelectList(getPosition, "PositionId", "PositionTitle");

            }
            return View();
        }

        // This action returns data as JSON for AJAX calls
        [HttpGet]
        public IActionResult GetPositionMappings()
        {
            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {
                var mappings = db.Query<PositionMappingViewModel>(
                    @"SELECT pm.PostMapId, 
                     pm.OrganizationId, 
                     o.OrganizationName, 
                     pm.DepartmentId, 
                     d.DepartmentName, 
                     pm.PositionId, 
                     p.PositionTitle 
              FROM PositionMapping pm 
              JOIN Organizations o ON pm.OrganizationId = o.OrganizationId 
              JOIN Departments d ON pm.DepartmentId = d.DepartmentId 
              JOIN Positions p ON pm.PositionId = p.PositionId").ToList();

                return Json(mappings);
            }
        }

        public JsonResult DeleteMapping(int postMapId)
        {

            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))

            {

                var query = "DELETE FROM PositionMapping WHERE PostMapId = @postMapId";
                var affectedRows = db.Execute(query, new { PostMapId = postMapId });

                return Json(new { success = true });
            }
        }

        public JsonResult CreateMapping(PositionMappingViewModel map)
        {

            var connectionString = configuration.GetConnectionString("dbcs");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Prepare the query to check for existing mappings
                string checkDeptQuery = "SELECT COUNT(*) FROM PositionMapping WHERE PositionId = @PositionId AND DepartmentId = @DepartmentId AND OrganizationId = @OrganizationId";

                // Pass individual properties from map as parameters
                var existingCount = db.ExecuteScalar<int>(checkDeptQuery, new
                {
                    PositionId = map.PositionId,
                    DepartmentId = map.DepartmentId,
                    OrganizationId = map.OrganizationId
                });

                if (existingCount > 0)
                {
                    // Return an error response if the department already exists
                    return Json(new { success = false, message = "Mapping already exists." });
                }

                // Insert the new position mapping and get the inserted ID
                var query = "INSERT INTO PositionMapping (OrganizationId, DepartmentId, PositionId) " +
                            "VALUES (@OrganizationId, @DepartmentId, @PositionId); " +
                            "SELECT CAST(SCOPE_IDENTITY() as int)";

                // Execute the insert and retrieve the new ID
                var newId = db.ExecuteScalar<int>(query, new
                {
                    OrganizationId = map.OrganizationId,
                    DepartmentId = map.DepartmentId,
                    PositionId = map.PositionId
                });

                // Optionally retrieve the complete mapping record if needed
                var newMapping = new
                {
                    PostMapId = newId,
                    OrganizationId = map.OrganizationId,
                    DepartmentId = map.DepartmentId,
                    PositionId = map.PositionId
                    // You can add more properties if needed
                };


                return Json(new { success = true, mapping = newMapping });
            }
        }


        [HttpPost]
        public JsonResult UpdateMapping(int postMapId, int positionId, int organizationId, int departmentId)
        {
            try
            {

                var connectionString = configuration.GetConnectionString("dbcs");

                using (IDbConnection db = new SqlConnection(connectionString))
                {


                    string updateQuery = @"
                UPDATE PositionMapping 
                SET PositionId = @positionId, OrganizationId = @organizationId, DepartmentId = @departmentId
                WHERE PostMapId = @PostMapId";

                    db.Execute(updateQuery, new { PostMapId = postMapId, PositionId = positionId, OrganizationId = organizationId, DepartmentId = departmentId });

                    return Json(new { success = true });
                }
            }
            catch (SqlException x)
            {
                if(x.ErrorCode ==1452){
                    return Json(new { success = false, message="Position Mapping already in use" });

                }
                else
                {
                    return Json(new { success = false, message=x.Message});

                }
                throw;
            }
        }


   
        public async Task<IActionResult> Filter()
        {

            using (IDbConnection db = new SqlConnection(configuration.GetConnectionString("dbcs")))
            {


                var organizations = await db.QueryAsync<Organizations>("SELECT * FROM Organizations");
                var departments = await db.QueryAsync<Departments>("SELECT * FROM Departments");
                var positions = await db.QueryAsync<Positions>("SELECT * FROM Positions");

                ViewBag.Organizations = new SelectList(organizations, "OrganizationId", "OrganizationName");
                ViewBag.Departments = new SelectList(departments, "DepartmentId", "DepartmentName");
                ViewBag.Positions = new SelectList(positions, "PositionId", "PositionTitle");
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetFilter(int? organizationId, int? departmentId, int? positionId)
        {
            var connectionString = configuration.GetConnectionString("dbcs");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Base SQL query to get average salaries by position in each department of every organization
                var sql = @"
            WITH AvgSalaries AS (
                SELECT 
                    o.OrganizationId,
                    o.OrganizationName,
                    d.DepartmentId,
                    d.DepartmentName,
                    p.PositionId,
                    p.PositionTitle,
                    
                    AVG(e.Salary) AS AverageSalary,
                    ROW_NUMBER() OVER (PARTITION BY o.OrganizationId ORDER BY AVG(e.Salary) DESC) AS SalaryRank
                FROM 
                    Employees e
                JOIN 
                    Positions p ON e.PositionId = p.PositionId
                JOIN 
                    Departments d ON p.DepartmentId = d.DepartmentId
                JOIN 
                    Organizations o ON e.OrganizationId = o.OrganizationId
                GROUP BY 
                    
                    o.OrganizationId, o.OrganizationName,
                    d.DepartmentId, d.DepartmentName,
                    p.PositionId, p.PositionTitle
            )
            SELECT 
                OrganizationId,
                OrganizationName,
                DepartmentId,
                DepartmentName,
                PositionId,
                PositionTitle,
                AverageSalary
            FROM 
                AvgSalaries
            WHERE 
                SalaryRank <= 2";
               
                var parameters = new DynamicParameters();

                // Apply filters based on optional parameters
                if (organizationId.HasValue)
                {
                    sql += " AND OrganizationId = @OrganizationId";
                    parameters.Add("@OrganizationId", organizationId.Value);
                }
                if (departmentId.HasValue)
                {
                    sql += " AND DepartmentId = @DepartmentId";
                    parameters.Add("@DepartmentId", departmentId.Value);
                }
                if (positionId.HasValue)
                {
                    sql += " AND PositionId = @PositionId";
                    parameters.Add("@PositionId", positionId.Value);
                }

                
                var result = await db.QueryAsync<EmployeeViewModel>(sql, parameters);

               
            

                return Json(result);
            }
        }








        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}