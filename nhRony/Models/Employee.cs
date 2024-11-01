namespace ClearingAndForwarding.Models
{
    public class Employee
    {
        public int Id { get; set; } // Primary key for the Employee table
        public string Name { get; set; } = string.Empty; // Employee's name
        public string Designation { get; set; } = string.Empty; // Employee's designation
        public string Department { get; set; } = string.Empty; // Employee's department
        public string Email { get; set; } = string.Empty; // Employee's email
        public DateOnly JoinDate { get; set; } // Employee's join date
        public string EmploymentStatus { get; set; } = string.Empty; // Employment status

        // Navigation property for related expenses
        public ICollection<Expenses> Expenses { get; set; } = new List<Expenses>();
    }
}
