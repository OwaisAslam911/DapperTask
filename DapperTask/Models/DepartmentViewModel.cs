namespace DapperTask.Models
{
    public class DepartmentViewModel
    {
        public Departments departments { get; set; }
        public IEnumerable<Organizations> organizations { get; set; }
    }
}
