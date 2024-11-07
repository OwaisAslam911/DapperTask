namespace DapperTask.Models
{
    public class PositionMapping
    {
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }

        public Departments Departments { get; set; } // Navigation property
        public Positions Positions { get; set; } // Navigation property
    }
}