using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SyllabusZip.Common.Data;

namespace SyllabusZip.Models
{
    public class DateTimeConvert
    {
        private readonly ApplicationDbContext _context;

        public DateTimeConvert(ApplicationDbContext applicationDbContext)
        {
            _context = applicationDbContext;
        }

        public IList<Assignment> CalculateAssignmentDate(string userId)
        {
            var dayNumber = DateTime.Today.AddDays(21);
            IList<Assignment> assignment = _context.Assignments.Include(a => a.Syllabus).Where(a => a.Syllabus.UserId ==
                 userId).ToList()
                .Where(a => stringtodate(a.Date) < dayNumber).ToList();

            return assignment;
        }

        public IList<Exam> CalculateExamDate(string userId)
        {
            var dayNumber = DateTime.Today.AddDays(21);
            IList<Exam> exam = _context.Exams.Include(a => a.Syllabus).Where(a => a.Syllabus.UserId ==
                 userId).ToList()
                .Where(a => stringtodate(a.Date) < dayNumber).ToList();

            return exam;
        }

        public DateTime stringtodate(string date)
        {
            if (DateTime.TryParse(date, out DateTime result))
                return result;

            string newMonth = date;
            newMonth = newMonth.Replace("Sept.", "September");
            newMonth = newMonth.Replace("Oct.", "October");
            newMonth = newMonth.Replace("Nov.", "November");
            newMonth = newMonth.Replace("Dec.", "December");
            newMonth = newMonth.Replace("Jan.", "January");
            newMonth = newMonth.Replace("Feb.", "February");
            newMonth = newMonth.Replace("Mar.", "March");
            newMonth = newMonth.Replace("Apr.", "April");
            //Skipping May, June, and July for abbreviation checks
            newMonth = newMonth.Replace("Aug.", "August");

            DateTime datetime = DateTime.Parse(newMonth + " " + DateTime.Now.Year);

            return datetime;
        }
    }

}
