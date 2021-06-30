using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SyllabusZip.Authentication;

namespace SyllabusZipSort
{
    public class SyllabusZipSort
    {
        private readonly ApplicationDbContext dbContext;

        [FunctionName("SyllabusZipSort")]
        public void Run([CosmosDBTrigger(
            databaseName: "jsoncontainer",
            collectionName: "Container1",
            ConnectionStringSetting = "pythoncriptstorage",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }

            //Sort the data into the database
            SyllabusIntoDataBase(input);
        }

        public SyllabusZipSort(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void SyllabusIntoDataBase(IReadOnlyList<Document> input)
        {
            //Getting the document as input
            var document = input.First();
            Guid syllabusId = document.GetPropertyValue<Guid>("SyllabusId");

            //Getting values from Contact Section
            Dictionary<string, string> contact_value = document.GetPropertyValue<Dictionary<string, string>>("Contact");

                var email = contact_value["Email"];
                var phone = contact_value["Phone"];
                var office = contact_value["Office"];
                var officehours = contact_value["Office Hours"];
                var mailbox = contact_value["Mailbox"];
                var classroom = contact_value["Classroom"];
                var classtime = contact_value["ClassTime"];
                var teacher = contact_value["Teacher"];
                var classtitle = contact_value["Class"];
                ContactInfo contact = new ContactInfo();
                contact.Email = email;
                contact.Phone = phone;
                contact.Office = office;
                contact.OfficeHours = officehours;
                contact.Mailbox = mailbox;
                contact.Classroom = classroom;
                contact.ClassTime = classtime;
                contact.Teacher = teacher;
                contact.ClassTitle = classtitle;
                contact.SyllabusId = syllabusId;
                dbContext.Contact.Add(contact);
                dbContext.SaveChanges();
            
            

            //Getting values from Schedule Section
            Dictionary<string, Dictionary<string, string>> schedule_value = document.GetPropertyValue<Dictionary<string, Dictionary<string, string>>>("Schedule");
            foreach (var item in schedule_value)
            {
                var date = item.Value[item.Key];
                var chapter = item.Value["Chapter"];
                var topic = item.Value["Topic"];
                var homework = item.Value["Assignments"];
                var project = item.Value["Assignments"];
                var exam = item.Value["Exams"];
            }

            //Getting values from Assignments Section
            Dictionary<string, List<string>> assignment_value = document.GetPropertyValue<Dictionary<string, List<string>>>("Assignments");
            foreach (var item in assignment_value)
            { 
                Assignment assignment = new Assignment();
                var date = item.Key;
                assignment.Date = date;
                if (item.Value.Count >= 1)
                {
                    var chapter = item.Value[0];
                    assignment.Chapter = chapter;
                }

                if (item.Value.Count >= 2)
                {
                    var homework = item.Value[1];
                    assignment.Homework = homework;
                }

                if (item.Value.Count >= 3)
                {
                    var project = item.Value[2];
                    assignment.Project = project;
                }
                assignment.SyllabusId = syllabusId;
                dbContext.Assignments.Add(assignment);
                dbContext.SaveChanges();
            }

            //Getting values from Exams Section
            Dictionary<string, string> exam_value = document.GetPropertyValue<Dictionary<string,string>>("Exams");
            foreach (var item in exam_value)
            {
                var examdate = item.Key;
                var examtype = item.Value;
                Exam exam = new Exam();
                exam.Date = examdate;
                exam.ExamType = examtype;
                exam.SyllabusId = syllabusId;
                dbContext.Exams.Add(exam);
                dbContext.SaveChanges();
            }


            //Getting values from Materials Section
            Dictionary<string, List<string>> materials_value = document.GetPropertyValue<Dictionary<string, List<string>>>("Materials");
            foreach (var item in materials_value)
            {
                var material_type = item.Key;
                var material_values = item.Value;
                foreach(var material_item in material_values)
                {
                    Materials materials = new Materials();
                    materials.Material_Type = material_type;
                    materials.Material_Value = material_item;
                    materials.SyllabusId = syllabusId;
                    dbContext.Materials.Add(materials);
                    dbContext.SaveChanges();
                }
                
            }
          
        }
    }
}
