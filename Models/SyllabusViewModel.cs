using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Models
{
    public class SyllabusViewModel
    {
        public IList<Syllabus> Syllabus { get; set; }
        public IList<ContactInfo> Contact { get; set; }
    }
}
