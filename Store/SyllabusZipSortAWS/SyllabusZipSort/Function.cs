using System;
using System.IO;
using System.Text;

using Amazon.Lambda.Core;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.DynamoDBv2.Model;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SyllabusZip.Common.Data;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SyllabusZipSort
{
    public class Function
    {
        private ApplicationDbContext dbContext;

        public Function()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>();
            options.UseSqlServer(Environment.GetEnvironmentVariable("SqlConnectionString"));
            dbContext = new ApplicationDbContext(options.Options);
        }

        public Function(ApplicationDbContext db)
        {
            dbContext = db;
        }

        public void FunctionHandler(DynamoDBEvent dynamoEvent, ILambdaContext context)
        {
            context.Logger.LogLine($"Beginning to process {dynamoEvent.Records.Count} records...");

            foreach (var record in dynamoEvent.Records)
            {
                //context.Logger.LogLine($"Event ID: {record.Dynamodb}");
                context.Logger.LogLine($"Event Name: {record.EventName}");
				
				// TODO: Add business logic processing the record.Dynamodb object.
            }
            context.Logger.LogLine("Stream processing complete.");

            //Sort the data into the database
            SyllabusIntoDataBase(dynamoEvent.Records.First().Dynamodb.NewImage);
        }

        public void SyllabusIntoDataBase(IDictionary<string, AttributeValue> document)
        {
            //context.Logger.LogLine(document["id"].S);
            Guid syllabusId;
            try
            {
                 syllabusId = Guid.Parse(document["SyllabusId"].S);
            }

            catch
            {
                return;
            }

            //Getting values of Who
            string who_value = document["Who"].S;
            

            //Getting values of Where
            string where_value = document.ContainsKey("Where") ? document["Where"].S : string.Empty;

            //Getting values of When
            string when_value = document.ContainsKey("When") ? document["When"].S : string.Empty;

            string class_value = document["Class"].S;

            //Getting values from Contact Section
            Dictionary<string, AttributeValue> contact_value = document["Contact"].M;

            var email = contact_value.ContainsKey("Email") ? contact_value["Email"].S : string.Empty;
            var phone = contact_value.ContainsKey("Phone") ? contact_value["Phone"].S : string.Empty;
            var office = contact_value.ContainsKey("Office") ? contact_value["Office"].S : string.Empty;
            var officehours = contact_value.ContainsKey("OfficeHours") ? contact_value["OfficeHours"].S : string.Empty;
            var mailbox = contact_value.ContainsKey("Mailbox") ? contact_value["Mailbox"].S : string.Empty;
            ContactInfo contact = new ContactInfo();
            contact.Email = email;
            contact.Phone = phone;
            contact.Office = office;
            contact.OfficeHours = officehours;
            contact.Mailbox = mailbox;
            contact.Classroom = where_value;
            contact.ClassTime = when_value;
            contact.Teacher = who_value;
            contact.ClassTitle = class_value;
            contact.SyllabusId = syllabusId;
            dbContext.Contact.Add(contact);
            dbContext.SaveChanges();


            //Getting values from Assignments Section
            Dictionary<string, AttributeValue> assignment_value = document["Assignments"].M;
            Dictionary<string, AttributeValue> schedule_value = document["Schedule"].M;
            foreach (var item in assignment_value)
            {
                Assignment assignment = new Assignment();
                var date = item.Key;
                assignment.Date = date;
                if (item.Value.L.Count >= 1)
                {
                    var chapter = item.Value.L[0].S;
                    assignment.Chapter = chapter;
                }

                if (item.Value.L.Count >= 2)
                {
                    var homework = item.Value.L[1].S;
                    assignment.Homework = homework;
                }

                if (item.Value.L.Count >= 3)
                {
                    var project = item.Value.L[2].S;
                    assignment.Project = project;
                }

                assignment.Topic = schedule_value[date]?.M["Topic"]?.S;
                assignment.SyllabusId = syllabusId;
                dbContext.Assignments.Add(assignment);
                dbContext.SaveChanges();

            }

            //Getting values from Exams Section
            Dictionary<string, AttributeValue> exam_value = document["Exams"].M;
            foreach (var item in exam_value)
            {
                var examdate = item.Key;
                var examtype = item.Value.S;
                Exam exam = new Exam();
                exam.Date = examdate;
                exam.ExamType = examtype;
                exam.SyllabusId = syllabusId;
                dbContext.Exams.Add(exam);
                dbContext.SaveChanges();
            }


            //Getting values from Materials Section
            List<AttributeValue> materials_value = document["Materials"].L;
            foreach (var item in materials_value)
            {
                //var material_type = null;
                var material_values = item.S;
                Materials materials = new Materials();
                materials.Material_Value = material_values;
                materials.SyllabusId = syllabusId;
                dbContext.Materials.Add(materials);
                dbContext.SaveChanges();

            }

        }
    }
}