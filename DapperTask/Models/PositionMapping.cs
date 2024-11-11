namespace DapperTask.Models
{
    public class PositionMapping
    {
        public int PostMapId { get; set; }
        public int OrganizationId { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }

        public Organizations organizations { get; set; } // Navigation property
        public Departments Departments { get; set; } // Navigation property
        public Positions Positions { get; set; } // Navigation property
    }
}