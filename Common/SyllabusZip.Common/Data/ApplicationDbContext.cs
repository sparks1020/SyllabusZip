using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SyllabusZip.Common.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public DbSet<ContactInfo> Contact { get; set; }
        public DbSet<Assignment> Assignments { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<Materials> Materials { get; set; }
        public DbSet<Syllabus> Syllabi { get; set; }
        public DbSet<SyllabusSource> SyllabusSources { get; set; }

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
        }


    }

    public class ContactInfo
    {
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Office { get; set; }
        public string OfficeHours { get; set; }
        public string Mailbox { get; set; }
        public string ClassTitle { get; set; }
        public string Teacher { get; set; }
        public string ClassTime { get; set; }
        public string Classroom { get; set; }

        public Guid SyllabusId { get; set; }


        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }

    public interface ISyllabusMember
    {
        Guid SyllabusId { get; set; }
        Syllabus Syllabus { get; set; }
    }

    public class Assignment : ISyllabusMember
    {
        public string Date { get; set; }
        public string Topic { get; set; }
        public string Chapter { get; set; }
        public string Homework { get; set; }
        public string Project { get; set; }

        public Guid SyllabusId { get; set; }
        public Syllabus Syllabus { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }

    public class Exam : ISyllabusMember
    {
        public string Date { get; set; }
        public string ExamType { get; set; }

        public Guid SyllabusId { get; set; }
        public Syllabus Syllabus { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }

    public class Materials
    {
        public string Material_Type { get; set; }

        public string Material_Value { get; set; }

        public Guid SyllabusId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
    }

    public class Syllabus
    {
        public string UserId { get; set; }
        public Guid Id { get; set; }
        public ContactInfo ContactInfo { get; set; }
        public bool CourseStatus { get; set; }

        /// <summary>The first day of the course, in the local time zone</summary>
        public DateTime CourseFirstDay { get; set; }

        public Guid? SourceId { get; set; }
        public SyllabusSource Source { get; set; }
    }

    public class SyllabusSource
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Service { get; set; }
        public string BaseUrl { get; set; }
        public string AuthToken { get; set; }
        public string AuthTokenSecret { get; set; }
        public DateTime AuthTokenExpires { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpires { get; set; }
        public string SourceUserId { get; set; }
        public string DisplayName { get; set; }

        public UserModel User { get; set; }
        public IList<Syllabus> Syllabi { get; set; }
    }
}
