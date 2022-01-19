using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using e_auction.net.Interface;
using e_auction.net.Models;
using System.Configuration;
//using Microsoft.Azure.Documents.Client;
using System;
//using Microsoft.Azure.Documents;
//using Microsoft.Azure.Documents.Linq;

using Microsoft.Azure.Cosmos;


namespace e_auction.net.Scripts
{
    public static class CosmosDbService<T> where T : class
    {
        private static readonly string DatabaseId = ConfigurationManager.AppSettings["database"];
        private static readonly string CollectionId = ConfigurationManager.AppSettings["collection"];
        private static CosmosClient cosmosClient;
        private static Database database;
        private static Container container;

        public static async void Initialize()
        {
            cosmosClient = new CosmosClient(ConfigurationManager.AppSettings["endpoint"]);
            database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseId);
            container = await database.CreateContainerIfNotExistsAsync("Seller", "/id", 400);
        }



        public static async Task<IEnumerable<T>> GetItemsAsync()
        {
            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c");
            FeedResponse<T> currentResultSet;

            FeedIterator<T> queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

            List<T> classEvents = new List<T>();

            while (queryResultSetIterator.HasMoreResults)
            {
                currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (T classEvent in currentResultSet)
                {
                    classEvents.Add(classEvent);
                }
            }

            return classEvents;
        }


        public static async Task<IEnumerable<T>> GetItemsAsync(string id)
        {
            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c where c.id='" + id + "'");
            FeedResponse<T> currentResultSet;

            FeedIterator<T> queryResultSetIterator = container.GetItemQueryIterator<T>(queryDefinition);

            List<T> classEvents = new List<T>();

            while (queryResultSetIterator.HasMoreResults)
            {
                currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (T classEvent in currentResultSet)
                {
                    classEvents.Add(classEvent);
                }
            }

            return classEvents;
        }


        public static async Task<ItemResponse<T>> GetCollectionItemAsync(string ID)
        {
            return await container.ReadItemAsync<T>(ID, new PartitionKey(ID));

        }

        public static async Task<ItemResponse<T>> ReplaceItemAsync(string ID, T modObject)
        {
            return await container.ReplaceItemAsync<T>(modObject, ID, new PartitionKey(ID));

        }


        public static async Task<ItemResponse<T>> CreateItemAsync(T item)
        {
            return await container.UpsertItemAsync(item);
        }

        public static async Task<ItemResponse<T>> DeleteItemAsync(string id)
        {
            return await container.DeleteItemAsync<T>(id, new PartitionKey(id));
        }
    }

}
