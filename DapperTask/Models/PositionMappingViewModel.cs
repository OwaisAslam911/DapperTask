namespace DapperTask.Models
{
    public class PositionMappingViewModel
    {
        public int PostMapId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } // New property for organization name
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } // Existing property for department name
        public int PositionId { get; set; }
        public string PositionTitle { get; set; } // Existing property for position title
    }
}
