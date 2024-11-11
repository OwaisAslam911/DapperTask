using System.Collections.Generic;
using DapperTask.Models;

namespace DapperTask.Models
{
    public class FilterViewModel
    {

        public int? SelectedOrganization { get; set; }
        public int? SelectedDepartment { get; set; }
        public int? SelectedPosition { get; set; }
        public IEnumerable<Organizations> Organizations { get; set; }
        public IEnumerable<Departments> Departments { get; set; }
        public IEnumerable<Positions> Positions { get; set; }
        public IEnumerable<EmployeeViewModel> Employees { get; set; }

    }
}