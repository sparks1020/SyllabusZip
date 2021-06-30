using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
using Azure.AI.FormRecognizer.Training;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace formrecognizer_quickstart
{
    class Program
    {
        static void Main(string[] args)
        {
            //var analyzeForm = RecognizeContent();
            //Task.WaitAll(analyzeForm);

            //var trainCustomModel = TrainCustomModelWithLabels();
            //Task.WaitAll(trainCustomModel);

            var trainCustomModel = TrainCustomModelNoLabels();
            Task.WaitAll(trainCustomModel);

            //var recognizeContentCustomModel = RecognizeContentCustomModel();
            //Task.WaitAll(recognizeContentCustomModel);
        }

        //static private FormRecognizerClient AuthenticateClient()
        //{
        //    string endpoint = "https://syllabustextrecognizer.cognitiveservices.azure.com/";
        //    string apiKey = "REDACTED";
        //    var credential = new AzureKeyCredential(apiKey);
        //    var client = new FormRecognizerClient(new Uri(endpoint), credential);
        //    return client;
        //}

        static private FormTrainingClient AuthenticateTrainingClient()
        {
            string endpoint = "https://syllabustextrecognizer.cognitiveservices.azure.com/";
            string apiKey = "REDACTED";
            var credential = new AzureKeyCredential(apiKey);
            var trainingClient = new FormTrainingClient(new Uri(endpoint), credential);
            return trainingClient;
        }

        //static async Task TrainCustomModelWithLabels()
        //{
        //    var trainingDataUrl = "REDACTED";
        //    var trainingClient = AuthenticateTrainingClient();

        //    CustomFormModel model = await trainingClient
        //        .StartTrainingAsync(new Uri(trainingDataUrl), useTrainingLabels: true)
        //        .WaitForCompletionAsync();
        //    Console.WriteLine($"Custom Model Info:");
        //    Console.WriteLine($"    Model Id: {model.ModelId}");
        //    Console.WriteLine($"    Model Status: {model.Status}");
        //    Console.WriteLine($"    Training model started on: {model.TrainingStartedOn}");
        //    Console.WriteLine($"    Training model completed on: {model.TrainingCompletedOn}");

        //    foreach (CustomFormSubmodel submodel in model.Submodels)
        //    {
        //        Console.WriteLine($"Submodel Form Type: {submodel.FormType}");
        //        foreach (CustomFormModelField field in submodel.Fields.Values)
        //        {
        //            Console.Write($"    FieldName: {field.Name}");
        //            if (field.Label != null)
        //            {
        //                Console.Write($", FieldLabel: {field.Label}");
        //            }
        //            Console.WriteLine("");
        //        }
        //    }
        //}

        //static async Task RecognizeContent()
        //{
        //    var invoiceUri = "https://raw.githubusercontent.com/Azure/azure-sdk-for-python/master/sdk/formrecognizer/azure-ai-formrecognizer/tests/sample_forms/forms/Invoice_1.pdf";
        //    var recognizeClient = AuthenticateClient();
        //    FormPageCollection formPages = await recognizeClient
        //        .StartRecognizeContentFromUri(new Uri(invoiceUri))
        //        .WaitForCompletionAsync();
        //    foreach (FormPage page in formPages)
        //    {
        //        Console.WriteLine($"Form Page {page.PageNumber} has {page.Lines.Count} lines.");

        //        for (int i = 0; i < page.Lines.Count; i++)
        //        {
        //            FormLine line = page.Lines[i];
        //            Console.WriteLine($"    Line {i} has {line.Words.Count} word{(line.Words.Count > 1 ? "s" : "")}, and text: '{line.Text}'.");
        //        }

        //        for (int i = 0; i < page.Tables.Count; i++)
        //        {
        //            FormTable table = page.Tables[i];
        //            Console.WriteLine($"Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");
        //            foreach (FormTableCell cell in table.Cells)
        //            {
        //                Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) contains text: '{cell.Text}'.");
        //            }
        //        }
        //    }
        //}

        static async Task TrainCustomModelNoLabels()
        {
            //var trainingDataUrl = "REDACTED";
            var trainingDataUrl = "REDACTED";
            var trainingClient = AuthenticateTrainingClient();
            CustomFormModel model = await trainingClient
                .StartTrainingAsync(new Uri(trainingDataUrl), useTrainingLabels: false)
                .WaitForCompletionAsync();
            Console.WriteLine($"Custom Model Info:");
            Console.WriteLine($"    Model Id: {model.ModelId}");
            Console.WriteLine($"    Model Status: {model.Status}");
            Console.WriteLine($"    Training model started on: {model.TrainingStartedOn}");
            Console.WriteLine($"    Training model completed on: {model.TrainingCompletedOn}");

            foreach (CustomFormSubmodel submodel in model.Submodels)
            {
                Console.WriteLine($"Submodel Form Type: {submodel.FormType}");
                foreach (CustomFormModelField field in submodel.Fields.Values)
                {
                    Console.Write($"    FieldName: {field.Name}");
                    if (field.Label != null)
                    {
                        Console.Write($", FieldLabel: {field.Label}");
                    }
                    Console.WriteLine("");
                }
            }
        }

        //static async Task RecognizeContentCustomModel()
        //{
        //    // Use the custom model ID returned in the previous example.
        //    string modelId = "decf9f1f-e895-4f42-b0c2-4cba797d375a";
        //    var invoiceUri = "https://raw.githubusercontent.com/Azure/azure-sdk-for-python/master/sdk/formrecognizer/azure-ai-formrecognizer/tests/sample_forms/forms/Invoice_1.pdf";
        //    var recognizeClient = AuthenticateClient();

        //    RecognizedFormCollection forms = await recognizeClient
        //    .StartRecognizeCustomFormsFromUri(modelId, new Uri(invoiceUri))
        //    .WaitForCompletionAsync();
        //    foreach (RecognizedForm form in forms)
        //    {
        //        Console.WriteLine($"Form of type: {form.FormType}");
        //        foreach (FormField field in form.Fields.Values)
        //        {
        //            Console.WriteLine($"Field '{field.Name}: ");

        //            if (field.LabelData != null)
        //            {
        //                Console.WriteLine($"    Label: '{field.LabelData.Text}");
        //            }

        //            Console.WriteLine($"    Value: '{field.ValueData.Text}");
        //            Console.WriteLine($"    Confidence: '{field.Confidence}");
        //        }
        //        Console.WriteLine("Table data:");
        //        foreach (FormPage page in form.Pages)
        //        {
        //            for (int i = 0; i < page.Tables.Count; i++)
        //            {
        //                FormTable table = page.Tables[i];
        //                Console.WriteLine($"Table {i} has {table.RowCount} rows and {table.ColumnCount} columns.");
        //                foreach (FormTableCell cell in table.Cells)
        //                {
        //                    Console.WriteLine($"    Cell ({cell.RowIndex}, {cell.ColumnIndex}) contains {(cell.IsHeader ? "header" : "text")}: '{cell.Text}'");
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
