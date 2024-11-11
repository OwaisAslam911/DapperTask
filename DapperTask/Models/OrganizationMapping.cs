namespace DapperTask.Models
{
    public class OrganizationMapping
    {
        public int mapId { get; set; }
        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }

        public Organizations Organizations { get; set; } // Navigation property
        public Departments Departments { get; set; } // Navigation property
    }
}
