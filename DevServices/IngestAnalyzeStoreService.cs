using Amazon;
using Amazon.Textract;
using Amazon.Textract.Model;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SyllabusZip.Common.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SyllabusZip.DevServices
{
    /// <summary>
    /// This is esentially a mock for the lambdas that run in production to
    /// run the full syllabus analysis pipeline on development machines.
    /// With few exceptions, the code here is identical (in fact, linked to
    /// the exact same files) to the code that runs in production.
    /// </summary>
    public class IngestAnalyzeStoreService
    {
        AmazonTextractClient client = new AmazonTextractClient(RegionEndpoint.USEast2);
        private IServiceScopeFactory ScopeFactory { get; }

        public IngestAnalyzeStoreService(IServiceScopeFactory scopeFactory)
        {
            ScopeFactory = scopeFactory;
        }

        public async Task Run(Syllabus syllabus, string bucketName, string keyName, string origFilename)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                // In production, this is triggered by S3 and fires Textract with
                // SNS -> SQS configured as the notification mechanism
                // NOTE: This does not call the same code as in production due
                // to significant differences in the call to Textract
                string jobId = await Ingest(bucketName, keyName, origFilename);

                GetDemoJson.Program.SyllabusResult syllabusResult = await Analyze(jobId, syllabus);
                syllabusResult.SyllabusId = syllabus.Id.ToString();

                Store(syllabusResult, scope.ServiceProvider.GetRequiredService<ApplicationDbContext>());
            }
        }

        private async Task<string> Ingest(string bucketName, string keyName, string origFilename)
        {
            using var md5 = MD5.Create();
            string hash = string.Join("", md5.ComputeHash(Encoding.UTF8.GetBytes(origFilename)).Select(x => x.ToString("x2")));

            var response = await client.StartDocumentAnalysisAsync(new StartDocumentAnalysisRequest
            {
                DocumentLocation = new DocumentLocation
                {
                    S3Object = new S3Object
                    {
                        Bucket = bucketName,
                        Name = keyName,
                    },
                },
                
                FeatureTypes = new List<string> { "TABLES", "FORMS" },
                //ClientRequestToken = hash, // This gives a "Request has invalid clientToken" message

                // Don't notify SQS in Dev
                /*NotificationChannel = new NotificationChannel
                {
                    SNSTopicArn = "arn:aws:sns:us-east-2:186604993685:initialsyllabusuploadsuccess",
                    RoleArn = "arn:aws:iam::186604993685:role/TextractRole",
                },*/

            });

            return response.JobId;
        }

        private async Task<GetDemoJson.Program.SyllabusResult> Analyze(string jobId, Syllabus syllabus)
        {
            var analysis = await GetDemoJson.Program.WaitForAnalysisToCompleteAsync(client, jobId);
            return GetDemoJson.Program.ExtractSyllabusData(analysis, jobId, syllabus);
        }

        private void Store(GetDemoJson.Program.SyllabusResult syllabus, ApplicationDbContext db)
        {
            // First, convert SyllabusResult to a DynamoDB object
            var document = Amazon.DynamoDBv2.DocumentModel.Document.FromJson(JsonConvert.SerializeObject(syllabus));

            // Then call our DynamoDB document handler
            new SyllabusZipSort.Function(db).SyllabusIntoDataBase(document.ToAttributeMap());
        }
    }
}
