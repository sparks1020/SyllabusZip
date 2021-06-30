using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SyllabusAnalyzer
{
    public class AnalysisUtils
    {
        private const string ShortMonths = "Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec";
        private const string MedMonths = "Sept";
        private const string LongMonths = "January|February|March|April|June|July|August|September|October|November|December";
        private const string Week = "Week";
        private static readonly string TimeDescriptors = $"{Week}";
        private const string Punctuation = "[.,]";
        private static readonly string LongDate = $@"({ShortMonths}|{MedMonths}|{LongMonths}|{TimeDescriptors}){Punctuation}*\s+\d+";
        public static readonly Regex DateExpression = new Regex($"{LongDate}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public static readonly Regex TimeExpression = new Regex(@"([0-2]?[0-9]|2[0-4]):[0-5][0-9]", RegexOptions.Compiled);
        public static readonly Regex RoomExpression = new Regex(@"([a-zA-Z][a-zA-Z0-9()-]*\s+)[0-9]+", RegexOptions.Compiled);
        public static readonly Regex WeekExpression = new Regex($@"({Week}){Punctuation}*\s+(\d+)", RegexOptions.Compiled);

        private static int NWeeks(int n) => n * 7;

        public static string NormalizeMonth(string month) =>
            month
                .Replace("Sept.", "September")
                .Replace("Oct.", "October")
                .Replace("Nov.", "November")
                .Replace("Dec.", "December")
                .Replace("Jan.", "January")
                .Replace("Feb.", "February")
                .Replace("Mar.", "March")
                .Replace("Apr.", "April")
                //Skipping May, June, and July for abbreviation checks
                .Replace("Aug.", "August");

        public static string NormalizeWeek(string date, DateTime startDate)
        {
            var weekMatch = WeekExpression.Match(date);
            if (weekMatch.Success)
            {
                var weeks = int.Parse(weekMatch.Groups[2].ToString());
                return startDate.AddDays(NWeeks(weeks - 1)).ToString();
            }
            else
                return date;
        }

        public static string IncludeYearIfNecessary(string date, DateTime startDate)
        {
            var couldParse = DateTime.TryParse(date, out DateTime maybeDt);
            var dt = couldParse ? maybeDt : DateTime.Parse(date + " " + startDate.Year.ToString());
            return (dt < startDate ? dt.AddYears(1) : dt).ToString();
        }
    }
}
