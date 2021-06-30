using Amazon;
using Amazon.Runtime;
using Amazon.Textract;
using Amazon.Textract.Model;
using Newtonsoft.Json;
using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static SyllabusAnalyzer.AnalysisUtils;

namespace GetDemoJson
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            // I think the goal is that the creds should be provided via environment variables,
            // but I don't remember exactly what that looks like in code here. These objects I'm
            // using are obviously deprecated, but I wanted to be explicit about where my access
            // tokens were coming from.
            AWSCredentials creds = StoredProfileAWSCredentials.CanCreateFrom("syllabuszip", "")
                ? new StoredProfileAWSCredentials("syllabuszip")
                : new StoredProfileAWSCredentials();
            AmazonTextractClient client = new AmazonTextractClient(creds, RegionEndpoint.USWest2);


            // This function goes in the Azure Function that is triggered from blob storage.
            // If you dive into the function, you'll see it references an S3 bucket and object,
            // but there IS a variant of the request (I believe) that allows you stream a byte
            // array to the API endpoint instead. If you continue to use S3 in an Azure function
            // triggered from blob storage, you would first have to copy from blob storage to S3,
            // then fire off the StartDocumentAnalysis request. A better way (if you decide not
            // to stream the bytes) would be to change the web site to upload to S3, then have an
            // S3-triggered Lambda function in AWS that just references the object that is
            // already stored in S3.
            var jobId = await StartDocumentAnalysis(client);

            // Eventually, we want to listen for notifications from SNS by way of SQS....
            // Otherwise, you're just paying for waiting time in either cloud function environment
            var analysis = await WaitForAnalysisToCompleteAsync(client, jobId);

            // This should go in something that is triggered by the SNS/SQS notification, so
            // probably an AWS Lambda function (not sure if you could even do that from Azure)
            // TBH, I am not sure what you get from SQS, so you might have to use the jobId you're
            // given from SQS (I assume you get at least *that*) to download the analysis results
            // similar to what I'm doing in WaitForAnalysisToCompleteAsync, just without the
            // busy wait (while ... sleep(1)). From here, you can either serialize the object
            // I created into CosmosDB or another data store, or you can just launch right into
            // your SyllabusZipSort functionality to write the necessary records to the SQL DB.
            var result = ExtractSyllabusData(analysis, jobId, new Syllabus { CourseFirstDay = DateTime.Now });

            Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        private static async Task<string> StartDocumentAnalysis(AmazonTextractClient client)
        {
            var response = await client.StartDocumentAnalysisAsync(new StartDocumentAnalysisRequest
            {
                DocumentLocation = new DocumentLocation
                {
                    S3Object = new S3Object
                    {
                        Bucket = "ss-sz-test",
                        Name = "MKTG6700SyllabusFA14.pdf"
                    }
                },
                FeatureTypes = new List<string> { "TABLES", "FORMS" },
                ClientRequestToken = "MKTG6700SyllabusFA14",

                /*NotificationChannel = new NotificationChannel
                {
                    SNSTopicArn = "arn:aws:sns:us-west-2:555632256646:ss-test"
                },
                JobTag = "example-syllabus"*/
            });

            return response.JobId;

            // TODO: Save JobID with syllabus record in DB. When we get notified on
            // the SNS/SQS topic, we'll get sent the jobID, which we then use to get the
            // results (I think). At that point, we can look up the internal syllabus
            // ID from the database
            // For now, we'll include the JobID in the output JSON
        }

        public static async Task<GetDocumentAnalysisResponse> WaitForAnalysisToCompleteAsync(AmazonTextractClient client, string jobId)
        {
            GetDocumentAnalysisResponse analysis = null;
            while (true)
            {
                analysis = await client.GetDocumentAnalysisAsync(new GetDocumentAnalysisRequest
                {
                    JobId = jobId
                });
                if (analysis.JobStatus == JobStatus.FAILED)
                {
                    Console.Error.WriteLine("FAILED");
                    return null;
                }
                if (analysis.JobStatus == JobStatus.SUCCEEDED)
                {
                    GetDocumentAnalysisResponse interim = analysis;
                    while (interim.NextToken != null)
                    {
                        interim = await client.GetDocumentAnalysisAsync(new GetDocumentAnalysisRequest
                        {
                            JobId = jobId,
                            NextToken = interim.NextToken
                        });
                        analysis.Blocks.AddRange(interim.Blocks);
                    }
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            return analysis;
        }

        public static SyllabusResult ExtractSyllabusData(GetDocumentAnalysisResponse analysis, string jobId, Syllabus syllabus)
        {
            // NOTE: This entire function has been redacted, sicne it is not part of the portfolio piece (and written by someone else)

            var result = new SyllabusResult();
            result.JobId = jobId;

            return result;
        }

        public class ScheduleItem
        {
            public string Date { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string SourceBlock { get; set; }
        }

        public class SyllabusResult
        {
            public string SyllabusId { get; set; }
            public string JobId { get; set; }
            public string Class { get; set; }
            public string Who { get; set; }
            public string When { get; set; }
            public string Where { get; set; }
            public ContactInfo Contact { get; } = new ContactInfo();
            public Dictionary<string, Dictionary<string, string>> Schedule { get; set; }
            public Dictionary<string, List<string>> Assignments { get; set; }
            public Dictionary<string, string> Exams { get; set; }
            public List<string> Materials { get; } = new List<string>();
        }

        public class ContactInfo
        {
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Office { get; set; }
            public string OfficeHours { get; set; }
            public string Mailbox { get; set; }
        }
    }

    public static class StringExtensions
    {
        public static string RemoveFront(this string orig, string toRemove)
        {
            if (!orig.StartsWith(toRemove, StringComparison.OrdinalIgnoreCase))
                return orig;
            return orig.Substring(toRemove.Length);
        }

        public static string RemoveLeadingPunctuation(this string orig)
        {
            for (int i = 0; i < orig.Length; i++)
            {
                if (char.IsLetterOrDigit(orig[i]))
                    return orig.Substring(i);
            }
            return string.Empty;
        }
    }
}
