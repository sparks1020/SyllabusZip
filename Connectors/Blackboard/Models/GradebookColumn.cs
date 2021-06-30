using System;

namespace SyllabusZip.Connectors.Blackboard.Models
{
    public class GradebookColumn
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string ExternalToolId { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool ExternalGrade { get; set; }
        public DateTime Created { get; set; }
        public string ContentId { get; set; }
        public Score Score { get; set; }
        public Availability Availability { get; set; }
        public GradingOptions Grading { get; set; }
        public string GradebookCategoryId { get; set; }
        public GradebookFormula Formula { get; set; }
        public bool IncludeinCalculations { get; set; }
        public bool ShowStatisticsToStudents { get; set; }
        public string ScoreProviderHandle { get; set; }
    }

    public class Score
    {
        public double Possible { get; set; }
    }

    public class GradingOptions
    {
        public string Type { get; set; }
        public DateTime Due { get; set; }
        public int AttemptsAllowed { get; set; }
        public string ScoringModel { get; set; }
        public string SchemaId { get; set; }
        public AnonymousGrading AnonymousGrading { get; set; }
    }

    public class AnonymousGrading
    {
        public string Type { get; set; }
        public DateTime ReleaseAfter { get; set; }
    }

    public class GradebookFormula
    {
        public string Formula { get; set; }
    }
}