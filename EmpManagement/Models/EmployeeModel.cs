using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmpManagement.Models
{
    public class EmployeeModel
    {
        public int Employee_ID { get; set; }

        public int Departement_ID { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Salary is required.")]
        public string Salary { get; set; }

        public string SelectedPaymentHeadsXml { get; set; }
    }
}