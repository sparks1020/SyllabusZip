using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.API.Models
{
    public class Course
    {

		public string id { get; set; }

		public string uuid { get; set; }

		public string externalId { get; set; }

		public string dataSourceId { get; set; }

		public string courseId { get; set; }

		public string name { get; set; }

		public string description { get; set; }

		public string created { get; set; }

		public bool organization { get; set; }

		public string ultraStatus { get; set; }

		public bool allowGuests { get; set; }

		public bool readOnly { get; set; }

		public string termId { get; set; }

		//public Availability availability { get; set; }

		//public Enrollment enrollment { get; set; }

		//public Locale locale { get; set; }
	}
}
