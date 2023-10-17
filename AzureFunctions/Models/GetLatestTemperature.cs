using System.Net;
using AzureFunctions.Methods;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctions.Models
{
    public class GetLatestTemperature
    {
        private readonly ILogger _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public GetLatestTemperature(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<GetLatestTemperature>();

            _cosmosClient = new CosmosClient("AccountEndpoint=https://kyhstockholm-cosmosdb.documents.azure.com:443/;AccountKey=UT6RLWM0MhoesC2QSpLg44FDXZQxl84OTttZVmYGgzmXHjfW51H8TYBJXY2g0dObYnHqCY7Ze3LNACDbMzd0Hg==;");
            var database = _cosmosClient.GetDatabase("kyh");
            _container = database.GetContainer("messages");
        }

        [Function("GetLatestTemperature")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {

            var result = _container.GetItemLinqQueryable<DataMessage>(true).OrderByDescending(x => x._ts).Take(1).ToList().FirstOrDefault();

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.WriteString(result.ToString());

            return response;
        }
    }
}
