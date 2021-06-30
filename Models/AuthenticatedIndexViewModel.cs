using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Models
{
    public class AuthenticatedIndexViewModel
    {
        public IList<Assignment> Assignment { get; set; }
        public IList<Exam> Exam { get; set; }
    }
}
