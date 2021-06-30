using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using System.IO;
using SyllabusZip.Common.Data;
using Microsoft.EntityFrameworkCore;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSSyllabusAnalyzeSQS
{
    

    public class Function
    {
        private ApplicationDbContext dbContext;

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
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

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an SQS event object and can be used 
        /// to respond to SQS messages.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task FunctionHandler(SQSEvent evnt, ILambdaContext context)
        {

            foreach(var message in evnt.Records)
            {
                await ProcessMessageAsync(message, context);
            }

            //Grab SQS Messaage and extracting the Body of the message
            var messageString = evnt.Records.First().Body;
            context.Logger.LogLine(messageString);
            JObject SQSmessage = JObject.Parse(messageString);

            //Grab the Body and extracting the Message out of it
            string SNSMessageString = SQSmessage["Message"].ToString();
            JObject SNSMessage = JObject.Parse(SNSMessageString);
            context.Logger.LogLine(SNSMessageString);

            //Extract syllabus ID out of SNSMessageString
            string syllabusFileName = SNSMessage["DocumentLocation"]["S3ObjectName"].ToString();
            context.Logger.LogLine(syllabusFileName);
            string syllabusId = Path.GetFileNameWithoutExtension(syllabusFileName);
            Syllabus syllabus = dbContext.Syllabi.FirstOrDefault(s => s.Id == Guid.Parse(syllabusId));

            //Extracting the Job ID out of the message from the Body
            string jobId = SNSMessage["JobId"].ToString();
            context.Logger.LogLine(jobId);
            AmazonTextractClient client = new AmazonTextractClient();

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
                    return;
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

            //call ExtractDocument code here
            var extractedsyllabusdata = GetDemoJson.Program.ExtractSyllabusData(analysis, jobId, syllabus);
            extractedsyllabusdata.SyllabusId = syllabusId;

            string syllabusAnalysisLog = JsonConvert.SerializeObject(extractedsyllabusdata);
            context.Logger.LogLine(syllabusAnalysisLog);

            //connecting to DynamoDB client
            AmazonDynamoDBClient db_client = new AmazonDynamoDBClient();
            Table syllabusMLFiles = Table.LoadTable(db_client, "SyllabusMLFiles");
            var ThisSyllabusMLFile = Amazon.DynamoDBv2.DocumentModel.Document.FromJson(syllabusAnalysisLog);
            ThisSyllabusMLFile["id"] = Guid.NewGuid();
            syllabusMLFiles.PutItemAsync(ThisSyllabusMLFile).Wait();

        }

        private async Task ProcessMessageAsync(SQSEvent.SQSMessage message, ILambdaContext context)
        {
            context.Logger.LogLine($"Processed message {message.Body}");

            // TODO: Do interesting work based on the new message
            await Task.CompletedTask;
        }
    }
}
