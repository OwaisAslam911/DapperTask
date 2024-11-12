namespace DapperTask.Models
{
    public class Organizations
    {

        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; }
        public DateTime FoundedDate { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
       
        public string Phone { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }

        public List<Departments> Departments { get; set; }
    }
}
