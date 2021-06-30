using NUnit.Framework;
using System.Collections.Generic;
using static GetDemoJson.Program;

namespace AnalyzerTests
{
    public class ScheduleBuilderTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DateWithTwoItemsOfDifferentType()
        {
            var initial = new List<ScheduleItem>
            {
                new ScheduleItem
                {
                    Date = "July 4",
                    Type = "Assignment",
                    Description = "Test Assignment"
                },
                new ScheduleItem
                {
                    Date = "July 4",
                    Type = "Exam",
                    Description = "Test Exam"
                },
            };

            var expected = new Dictionary<string, Dictionary<string, string>>
            {
                ["July 4"] = new Dictionary<string, string>
                {
                    ["Assignment"] = "Test Assignment",
                    ["Exam"] = "Test Exam"
                }
            };

            var actual = PopulateSchedule(initial);

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void DateWithTwoItemsOfSameType()
        {
            var initial = new List<ScheduleItem>
            {
                new ScheduleItem
                {
                    Date = "July 4",
                    Type = "Assignment",
                    Description = "Test Assignment"
                },
                new ScheduleItem
                {
                    Date = "July 4",
                    Type = "Assignment",
                    Description = "Test Assignment 2"
                },
            };

            var expected = new Dictionary<string, Dictionary<string, string>>
            {
                ["July 4"] = new Dictionary<string, string>
                {
                    ["Assignment"] = "Test Assignment, Test Assignment 2"
                }
            };

            var actual = PopulateSchedule(initial);

            Assert.AreEqual(expected, actual);
        }
    }
}