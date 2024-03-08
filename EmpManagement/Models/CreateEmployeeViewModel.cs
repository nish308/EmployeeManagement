using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpManagement.Models
{
    public class CreateEmployeeViewModel
    {
        public EmployeeModel Employee { get; set; }

        public SelectList DepartmentList { get; set; }

        public List<PayHeadsModel> PayHeads { get; set; }

    }
}