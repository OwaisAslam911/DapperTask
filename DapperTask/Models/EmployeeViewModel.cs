namespace DapperTask.Models
{
    public class EmployeeViewModel
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Salary { get; set; }
        public int PositionId { get; set; }  // Make sure this exists
        public int DepartmentId { get; set; }
        public int OrganizationId { get; set; }
        public string PositionTitle { get; set; }  // Additional info for displaying
        public string DepartmentName { get; set; }
        public string OrganizationName { get; set; }
        public int AverageSalary { get; set; }

    }
}
