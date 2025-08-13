using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;
using VesselProcessingSystem.Models;

namespace VesselProcessingSystem.Services
{
    public class CosmosDbService : ICosmosDbService
    {
        private readonly Configurations _configurations;

        public CosmosDbService(
            IOptions<Configurations> configurations)
        {
            _configurations = configurations.Value;
        }

        public async Task<bool> AddItemAsync<T>(T document, string partitionKey)
        {
            try
            {
                var connectionString = _configurations.ConnectionDbString;
                var databaseName = _configurations.DataBaseNameId;
                var containerName = _configurations.ContainerId;
                var itemCreated = false;

                using var client = new CosmosClient(connectionString);
                var container = client.GetContainer(databaseName, containerName);
                var response = await container.UpsertItemAsync(document, new PartitionKey(partitionKey));
                if (response.StatusCode == System.Net.HttpStatusCode.Created || response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    itemCreated = true;
                }
                return itemCreated;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<T>> GetAllItemsAsync<T>()
        {
            try
            {
                //TODO Improve the use of container when accessing it.
                var connectionString = _configurations.ConnectionDbString;
                var databaseName = _configurations.DataBaseNameId;
                var containerName = _configurations.ContainerId;

                using var client = new CosmosClient(connectionString);
                var container = client.GetContainer(databaseName, containerName);

                var query = "SELECT * FROM c";
                var queryDefinition = new QueryDefinition(query);
                var queryIterator = container.GetItemQueryIterator<T>(queryDefinition);

                var results = new List<T>();

                while (queryIterator.HasMoreResults)
                {
                    var response = await queryIterator.ReadNextAsync();
                    results.AddRange(response.Resource);
                }

                return results;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<T> GetItemByIdAsync<T>(string id)
        {
            try
            {
                //TODO Improve the use of container when accessing it.
                var connectionString = _configurations.ConnectionDbString;
                var databaseName = _configurations.DataBaseNameId;
                var containerName = _configurations.ContainerId;

                using var client = new CosmosClient(connectionString);
                var container = client.GetContainer(databaseName, containerName);

                var query = new QueryDefinition("SELECT * FROM c WHERE c.id = @id")
                .WithParameter("@id", id);

                using var iterator = container.GetItemQueryIterator<T>(query);

                if (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();
                    return response.FirstOrDefault();
                }

                return default;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}