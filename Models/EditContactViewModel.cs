using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Models
{
    public class EditContactViewModel
    {
        public string ClassTime { get; set; }
        public string Teacher { get; set; }
        public string Office { get; set; }
        public string Office_Hours { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ClassTitle { get; set; }

        public string Mailbox { get; set; }
        public string ContactId { get; set; }
    }
}
