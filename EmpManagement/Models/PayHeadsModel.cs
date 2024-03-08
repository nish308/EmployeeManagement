using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EmpManagement.Models
{
    public class PayHeadsModel
    {
        public int Payment_Head_ID { get; set; }

        public string Payment_Head_Name { get; set; }
    }
}