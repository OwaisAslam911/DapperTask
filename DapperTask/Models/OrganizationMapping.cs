namespace DapperTask.Models
{
    public class OrganizationMapping
    {
        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }

        public Organizations Organizations { get; set; } // Navigation property
        public Departments Departments { get; set; } // Navigation property
    }
}
