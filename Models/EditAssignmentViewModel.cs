using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Models
{
    public class EditAssignmentViewModel
    {
        public string ClassTitle { get; set; }
        public string Date { get; set; }
        public string Topic { get; set; }
        public string Chapter { get; set; }
        public string Homework { get; set; }
        public string Project { get; set; }

        public string AssignmentId { get; set; }
    }
}
