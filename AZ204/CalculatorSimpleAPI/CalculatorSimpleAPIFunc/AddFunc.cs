using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace CalculatorSimpleAPIFunc
{
    public static class AddFunc
    {
        [FunctionName("AddFunc")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequestMessage req, ILogger log)
        {
            log.LogInformation("Add operation");

            // parse query parameter
            var val1 = req.GetQueryNameValuePairs().FirstOrDefault(q => String.Compare(q.Key, "val1", StringComparison.OrdinalIgnoreCase) == 0).Value;
            var val2 = req.GetQueryNameValuePairs().FirstOrDefault(q => string.Compare(q.Key, "val2", StringComparison.OrdinalIgnoreCase) == 0).Value;

            try
            {
                var res = int.Parse(val1) + int.Parse(val2);
                return req.CreateResponse(HttpStatusCode.OK, res);
            }
            catch (Exception)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "provide val1 and val2 integers");
            }
        }
    }
}