using System;
using System.Collections.Generic;
using Microsoft.Azure.Batch;
using Microsoft.Azure.Batch.Auth;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureBatchRenderingCompletionSupervisor
{
    public class CheckRenderingTasksCompletion
    {

        // Batch account credentials
        string _batchAccountName = "";
        string _batchAccountKey = "";
        string _batchAccountUrl = "";


        // the app env config
        private readonly IConfiguration _configuration;

        public CheckRenderingTasksCompletion(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName("CheckRenderingTasksCompletion")]
        public void Run([TimerTrigger("/10 * * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Apply Design rendering completion supervisor function executed at: {DateTime.Now}");

            // load enviorment vars 
            loadConfig(_configuration, log);

            // Get a Batch client using account creds
            BatchSharedKeyCredentials cred = new BatchSharedKeyCredentials(_batchAccountUrl, _batchAccountName, _batchAccountKey);

            // get current env
            string currentEnvironment = Environment.GetEnvironmentVariable("CurrentEnvironment") ?? "Local";

            // get all current rendering jobs in batch
            using (BatchClient batchClient = BatchClient.Open(cred))
            {
                foreach (var job in batchClient.JobOperations.ListJobs())
                {
                    var (callbackUrl, renderJobId, renderURL) = extractMetadataFromJob(job);

                    if (currentEnvironment)
                    {

                    }
                }


            }
        }


        private static (string, string, string, string) extractMetadataFromJob(CloudJob cloudJob)
        {
            string callbackUrl = "", renderJobId = "", renderURL = "", env = "";
            using (IEnumerator<MetadataItem> enumrator = cloudJob.Metadata.GetEnumerator())
            {
                while (enumrator.MoveNext())
                {
                    MetadataItem item = enumrator.Current;
                    switch (item.Name)
                    {
                        case "CallbackUrl":
                            callbackUrl = item.Value;
                            break;

                        case "RenderJobId":
                            renderJobId = item.Value;
                            break;

                        case "ContainerUrl":
                            renderURL = item.Value + $"/{renderJobId}/0001.png";
                            break;

                        case "Environment":
                            env = item.Value + $"/{renderJobId}/0001.png";
                            break;
                    }

                }
            }
            return (callbackUrl, renderJobId, renderURL, env);
        }

        private void loadConfig(IConfiguration i_configuration, ILogger log)
        {
            log.LogInformation($" KEY_BatchAccountName : {i_configuration["KEY_BatchAccountName"]}");
            _batchAccountName = i_configuration["KEY_BatchAccountName"];

            log.LogInformation($" KEY_BatchAccountKey : {i_configuration["KEY_BatchAccountKey"]}");
            _batchAccountKey = i_configuration["KEY_BatchAccountKey"];

            log.LogInformation($" KEY_BatchAccountUrl : {i_configuration["KEY_BatchAccountUrl"]}");
            _batchAccountUrl = i_configuration["KEY_BatchAccountUrl"];
        }
    }
}
