namespace VesselProcessingSystem.Services
{
    public interface ICosmosDbService
    {
        Task<bool> AddItemAsync<T>(T document, string partitionKey);

        Task<List<T>> GetAllItemsAsync<T>();

        Task<T> GetItemByIdAsync<T>(string id);
    }
}
