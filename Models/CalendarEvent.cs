using System;

namespace SyllabusZip.Models
{
    public class CalendarEvent
    {
        // The shape of this class is modeled after
        // https://fullcalendar.io/docs/event-object
        // so we can use it directly with the fullcalendar
        // plugin on each Syllabus's calendar page

        public Guid Id { get; set; }

        public bool AllDay { get; set; }

        public DateTime Start { get; set; }

        public string Title { get; set; }
    }
}