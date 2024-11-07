namespace DapperTask.Models
{
    public class Employees
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int PositionId { get; set; }
        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }
        public string Address { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public decimal Salary { get; set; }

        public Positions Positions { get; set; } // Navigation property
        public Departments Departments { get; set; } // Navigation property
        public Organizations Organizations { get; set; } // Navigation property
    }
}
