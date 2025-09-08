namespace HospitalManagementSystem.Models
{
    public class Department
    {
        public int DepartmentID { get; set; }
        public string? Name { get; set; }
        public ICollection<Doctor> Doctors { get; set; }
    }
}
