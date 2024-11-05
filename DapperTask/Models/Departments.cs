namespace DapperTask.Models
{
    public class Departments
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int OrganizationId { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public Organizations Organizations { get; set; } // Navigation property
    }
}
