using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmpManagement.Models
{
    public class EditEmployeeViewModel
    {
        public EmployeeModel Employee { get; set; }

        public SelectList DepartmentList { get; set; }

        public List<PayHeadsModel> SelectedpayHeads { get; set; }

        public List<PayHeadsModel> AllpayHeads { get; set; }

        [AllowHtml]
        public string SelectedPaymentHeadsXml { get; set; }
    }
}