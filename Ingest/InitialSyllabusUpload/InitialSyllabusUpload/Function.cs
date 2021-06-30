using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.Textract;
using Amazon.Textract.Model;
using Amazon.Lambda.Core;
using Amazon.Lambda.S3Events;
using Amazon;
using Amazon.S3;
using Amazon.S3.Util;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace InitialSyllabusUpload
{
    public class Function
    {
        IAmazonS3 S3Client { get; set; }

        /// <summary>
        /// Default constructor. This constructor is used by Lambda to construct the instance. When invoked in a Lambda environment
        /// the AWS credentials will come from the IAM role associated with the function and the AWS region will be set to the
        /// region the Lambda function is executed in.
        /// </summary>
        public Function()
        {
            S3Client = new AmazonS3Client();
        }

        /// <summary>
        /// Constructs an instance with a preconfigured S3 client. This can be used for testing the outside of the Lambda environment.
        /// </summary>
        /// <param name="s3Client"></param>
        public Function(IAmazonS3 s3Client)
        {
            this.S3Client = s3Client;

        }

        /// <summary>
        /// This method is called for every Lambda invocation. This method takes in an S3 event object and can be used 
        /// to respond to S3 notifications.
        /// </summary>
        /// <param name="evnt"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(S3Event evnt, ILambdaContext context)
        {
            var s3Event = evnt.Records?[0].S3;
            AmazonTextractClient client = new AmazonTextractClient(RegionEndpoint.USEast2);
            if (s3Event == null)
            {
                return null;
            }

            try
            {
                context.Logger.LogLine(s3Event.Bucket.Name);
                context.Logger.LogLine(s3Event.Object.Key);
                //var response = await this.S3Client.GetObjectMetadataAsync(s3Event.Bucket.Name, s3Event.Object.Key);
                var response = await client.StartDocumentAnalysisAsync(new StartDocumentAnalysisRequest
                {
                    DocumentLocation = new DocumentLocation
                    {
                        S3Object = new S3Object
                        {
                            Bucket = s3Event.Bucket.Name,
                            Name = s3Event.Object.Key
                        }
                    },
                    FeatureTypes = new List<string> { "TABLES", "FORMS" },
                    //ClientRequestToken = "example-syllabus", //replace with actual variable


                    NotificationChannel = new NotificationChannel
                    {
                        SNSTopicArn = "REDACTED",
                        RoleArn = "REDACTED",
                    },

                });

                return response.JobId;

                // TODO: Save JobID with syllabus record in DB. When we get notified on
                // the SNS/SQS topic, we'll get sent the jobID, which we then use to get the
                // results (I think). At that point, we can look up the internal syllabus
                // ID from the database
                // For now, we'll include the JobID in the output JSON

                //return response.Headers.ContentType;
            }
            catch (Exception e)
            {
                context.Logger.LogLine($"SOMETHING DIED");
                context.Logger.LogLine(e.Message);
                context.Logger.LogLine(e.StackTrace);
                throw;
            }
        }
    }
}
