/*
{
    "results": [
        {
            "id":"_4_1",
            "userId":"_6_1",
            "courseId":"_3_1",
            "course":{
                "id":"_3_1",
                "uuid":"ac93966501144515b3107299dbacfbd7",
                "externalId":"5000",
                "dataSourceId":"_2_1",
                "courseId":"5000",
                "name":"MarineAnimals",
                "created":"2021-04-09T21:29:33.260Z",
                "organization":false,
                "ultraStatus":"Classic",
                "allowGuests":false,
                "readOnly":false,
                "availability":{
                    "available":"Yes",
                    "duration":{
                        "type":"DateRange",
                        "start":"2021-04-09T21:27:00.000Z",
                        "end":"2021-08-27T21:27:00.000Z"
                    }
                },
                "enrollment":{
                    "type":"SelfEnrollment",
                    "start":"2021-04-09T21:27:57.458Z"
                },
                "locale":{
                    "force":false
                },
                "externalAccessUrl":"https://blackboard.syllabuszip.com/webapps/blackboard/execute/courseMain?course_id=_3_1&sc="
            },
            "dataSourceId":"_2_1",
            "created":"2021-05-01T20:59:44.439Z",
            "modified":"2021-05-01T20:59:44.439Z",
            "availability":{
                "available":"Yes"
            },
            "courseRoleId":"Student"
        }
    ]
}
*/

using System;
using System.Collections.Generic;
using System.Linq;

namespace SyllabusZip.Connectors.Blackboard.Models
{

    public class CourseResults : ResultsList<CourseMembership>
    {
    }

    public class ResultsList<T>
    {
        public IList<T> Results { get; set; }
    }

    public class CourseMembership
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public string CourseId { get; set; }
        public Course Course { get; set; }
        public string DataSourceId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Availability Availability { get; set; }
        public string CourseRoleId { get; set; }
    }

    public class Course
    {
        public string Id { get; set; }
        public string Uuid { get; set; }
        public string ExternalId { get; set; }
        public string DataSourceId { get; set; }
        public string CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public bool Organization { get; set; }
        public string UltraStatus { get; set; }
        public bool AllowGuests { get; set; }
        public bool ReadOnly { get; set; }
        public Availability Availability { get; set; }
        public EnrollmentDetails Enrollment { get; set; }
        public LocaleDetails Locale { get; set; }
    }

    public class Availability
    {
        public string Available { get; set; }
        public Duration Duration { get; set; }
    }

    public class Duration
    {
        public string Type { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class EnrollmentDetails
    {
        public string Type { get; set; }
        public DateTime Start { get; set; }
    }

    public class LocaleDetails
    {
        public bool Force { get; set; }
    }

    public class User
    {
        // NOTE: This is intentionally an incomplete list to avoid accessing
        // any unnecessary PII

        public string Id { get; set; }
        public string Uuid { get; set; }
        public string UserName { get; set; }
        public Name Name { get; set; }
        public Contact Contact { get; set; }
    }

    public class Name
    {
        public string Given { get; set; }
        public string Family { get; set; }
        public string Middle { get; set; }
        public string Other { get; set; }
        public string Suffix { get; set; }
        public string Title { get; set; }

        public string DisplayName => string.Join(" ", new[] { Title, Given, Family, Suffix }.Where(x => !string.IsNullOrEmpty(x)));
    }

    public class Contact
    {
        public string MobilePhone { get; set; }
        public string BusinessPhone { get; set; }
        public string BusinessFax { get; set; }
        public string Email { get; set; }
        public string WebPage { get; set; }
    }
}