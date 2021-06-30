using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Documents;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace SyllabusZip_Analyze
{
    public static class Parse
    {
        private static string body { get; set; }
        private static int height { get; set; }
        private static Bucket bucket = new Bucket();
        private static IList<Bucket> bucketList = new List<Bucket>() { bucket };

        private static IReadOnlyList<Document> input { get; set; }
        private static string title_bucket;
        private static string content_bucket;
        private static string titles;
        private static string contents;


        [FunctionName("Parse")]
        public static void Run(
            [CosmosDBTrigger(
            databaseName: "syllabusDB",
            collectionName: "syllabuscontainer",
            ConnectionStringSetting = "PythonScriptStorage",
            LeaseCollectionName = "leases")]IReadOnlyList<Document> input, ILogger log)
        {
            Parse.input =  input;

            if (input != null && input.Count > 0)
            {
                log.LogInformation("Documents modified " + input.Count);
                log.LogInformation("First document Id " + input[0].Id);
            }

            SortSyllabusData(log);

            log.LogInformation("Contents of Title bucket" + title_bucket);
            log.LogInformation("Contents of Content bucket\\\\" + content_bucket);
         
        }

        class Bucket
        {
            public string Title { get; set; }
            public string Content { get; set; }
        }

        private static void SortSyllabusData(ILogger log)
        {
            var array = input.First().GetPropertyValue<JArray>("bounding_boxes");
            //logging each piece of the function to make sure stuff is processing
            log.LogInformation(array.ToString());
            foreach (var bounded_text in array)
            {
                //while JObject array is (condition, not less than a value, but not null) then get the value of the bounding box
                //else, break
                var data = ((JObject)bounded_text).GetValue("bounding_box").ToString();
                var line = ((JObject)bounded_text).GetValue("line");
                //also need to increment the array position to get each part
                if (data != null)
                {

                    //if data >= 50, then it's a title, if it's <= 49 it's body content
                    int word_height = int.Parse(data.Split(",")[3]);
                    if (word_height >= 50)
                    {
                        //we need the line content after sorting by the bounding box content
                        //title_bucket = line.ToString();
                        //titles += title_bucket + " ";
                        bucket = new Bucket();
                        bucketList.Add(bucket);
                        bucket.Title = line.ToString();
                    }

                    else
                    {
                        //we need the line content after sorting by the bounding box content
                        bucket.Content = line.ToString();
                    }


                }

            }

        }

        private static void OrganizeSyllabusData()
        {
            
        }


    }
}
