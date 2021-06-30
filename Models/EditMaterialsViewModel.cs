using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Models
{
    public class EditMaterialsViewModel
    {
        [DataType(DataType.MultilineText)]
        public string Material_Value { get; set; }

        public string SyllabusId { get; set; }
    }
}
