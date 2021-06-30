using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SyllabusZip.Common.Data;
using SyllabusZip.Models;

namespace SyllabusZip.Services
{
    public class CalendarService
    {
        private ApplicationDbContext Database { get; }

        public CalendarService(ApplicationDbContext db)
        {
            Database = db;
        }

        private IQueryable<T> Filtered<T>(DbSet<T> collection, Expression<Func<ISyllabusMember, bool>> filter) where T : class, ISyllabusMember
        {
            if (filter is null)
                return collection;

            return collection.Include(c => c.Syllabus).Where(filter).Cast<T>();
        }

        public IList<CalendarEvent> GetEvents(Expression<Func<ISyllabusMember, bool>> filter = null, bool addToday = false)
        {
            if (filter is null)
                filter = x => true;

            List<CalendarEvent> events = new List<CalendarEvent>();

            if (addToday)
            {
                events.Add(
                    new CalendarEvent
                    {
                        Id = Guid.NewGuid(),
                        AllDay = true,
                        Start = DateTime.Today,
                        Title = "Today"
                    });
            }

            DateTimeConvert dtConvert = new DateTimeConvert(Database);

            foreach (var item in Filtered(Database.Assignments, filter))
            {
                DateTime date = dtConvert.stringtodate(item.Date);

                if (!string.IsNullOrEmpty(item.Chapter))
                {

                    events.Add(
                    new CalendarEvent
                    {
                        Id = Guid.NewGuid(),
                        AllDay = true,
                        Start = date,
                        Title = item.Chapter
                    });


                }

                if (!string.IsNullOrEmpty(item.Homework))
                {
                    events.Add(
                        new CalendarEvent
                        {
                            Id = Guid.NewGuid(),
                            AllDay = true,
                            Start = date,
                            Title = item.Homework
                        });
                }

                if (!string.IsNullOrEmpty(item.Project))
                {
                    events.Add(
                        new CalendarEvent
                        {
                            Id = Guid.NewGuid(),
                            AllDay = true,
                            Start = date,
                            Title = item.Project
                        });
                }
            }

            foreach (var item in Filtered(Database.Exams, filter))
            {

                if (!string.IsNullOrEmpty(item.ExamType))
                {
                    events.Add(
                        new CalendarEvent
                        {
                            Id = Guid.NewGuid(),
                            AllDay = true,
                            Start = dtConvert.stringtodate(item.Date),
                            Title = item.ExamType
                        });
                }

            }

            return events;
        }
    }
}