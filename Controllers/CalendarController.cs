using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net.DataTypes;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SyllabusZip.Services;

namespace SyllabusZip.Controllers
{
    [Authorize]
    [Route("[Controller]")]
    public class CalendarController : Controller
    {
        private CalendarService CalendarService { get; }

        public CalendarController(CalendarService calendarService)
        {
            CalendarService = calendarService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult CalendarFeed(string id)
        {
            var calendar = new Calendar();

            calendar.Events.AddRange(CalendarService.GetEvents(e => e.Syllabus.UserId == id).Select(e => new CalendarEvent
            {
                Start = new CalDateTime(e.Start),
                End = new CalDateTime(e.Start),
                IsAllDay = e.AllDay,
                Summary = e.Title,
                Description = e.Title,
            }));

            var serializer = new CalendarSerializer();
            var serializedCalendar = serializer.SerializeToString(calendar);

            return Content(serializedCalendar, "text/calendar");
        }
    }
}