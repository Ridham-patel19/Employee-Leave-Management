using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class Application
    {
        public int? ApplicationId { get; set; }
        public int? EmpId { get; set; }

        public int LeaveId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }

        public string Reason { get; set; }
    }
}