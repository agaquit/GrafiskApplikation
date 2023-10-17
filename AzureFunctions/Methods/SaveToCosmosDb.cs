using System;
using System.Text;
using Azure.Messaging.EventHubs;
using AzureFunctions.Models;
using Microsoft.Azure.Amqp;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctions.Methods
{
    public class SaveToCosmosDb
    {
        private readonly ILogger<SaveToCosmosDb> _logger;
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;


        public SaveToCosmosDb(ILogger<SaveToCosmosDb> logger)
        {
            _logger = logger;

            _cosmosClient = new CosmosClient("AccountEndpoint=https://kyhstockholm-cosmosdb.documents.azure.com:443/;AccountKey=UT6RLWM0MhoesC2QSpLg44FDXZQxl84OTttZVmYGgzmXHjfW51H8TYBJXY2g0dObYnHqCY7Ze3LNACDbMzd0Hg==;");
            var database = _cosmosClient.GetDatabase("kyh");
            _container = database.GetContainer("messages");
        }

        [Function(nameof(SaveToCosmosDb))]
        public async Task Run([EventHubTrigger("iothub-ehub-kyhstockho-25282124-32e99f73c6", Connection = "IotHubEndpoint")] EventData[] events)
        {
            foreach (EventData @event in events)
            {
                try
                {
                    var json = Encoding.UTF8.GetString(@event.Body.ToArray());
                    var data = JsonConvert.DeserializeObject<TemperatureData>(json);
                    await _container.CreateItemAsync(data, new PartitionKey(data.id));
                    //await _container.CreateItemAsync(json);
                   

                    _logger.LogInformation($"sparade meddelandet: {data}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not save: {ex.Message}");
                }
            }
        }
    }
}
