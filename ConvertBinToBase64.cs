using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace Mondi.Functions.DataOperations
{
    public static class ConvertBinToBase64
    {
        [FunctionName("ConvertBinToBase64")]
        public static async Task<ActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("ConvertBinToBase64 HTTP trigger function is processing a request...");

            string filePath = Environment.GetEnvironmentVariable("temp");
            var provider = new MultipartFormDataStreamProvider(filePath);
            var multipartProvider = await req.Content.ReadAsMultipartAsync(provider);
            var fileData = multipartProvider.FileData.FirstOrDefault();

            if (fileData == null)
            {
                return new BadRequestObjectResult("ContentType must be multipart/form-data and one file must be submitted");
            }

            var b64String = Convert.ToBase64String(File.ReadAllBytes(fileData.LocalFileName));
            var dataUrl = String.Format("data:{0};base64,{1}", "application/octet-stream", b64String);

            log.LogInformation("File was successfully as Base64 Data Url encoded.");

            return new OkObjectResult(new {dataUrl=dataUrl});
        }
    }
}
