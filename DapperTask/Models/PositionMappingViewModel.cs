using DapperTask.Models;

namespace DapperTask.Models
{
    public class PositionMappingViewModel
    {
        public int PostMapId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int PositionId { get; set; }
        public string PositionTitle { get; set; }

        public List<Organizations> Organizations { get; set; }
        public List<Departments> Departments { get; set; }
        public List<Positions> Positions { get; set; }
        public List<PositionMappingViewModel> PositionMappings { get; set; }
    }
}
