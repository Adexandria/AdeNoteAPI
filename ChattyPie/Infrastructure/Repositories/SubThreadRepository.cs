using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models;
using Microsoft.Azure.Cosmos;
using System.Net;


namespace ChattyPie.Infrastructure.Repositories
{
    internal class SubThreadRepository : ISubThreadRepository
    {
        public SubThreadRepository(Database database)
        {
            container = database.CreateContainerIfNotExistsAsync("subthreads", "/threadId").Result;
        }
        public async Task<bool> Add(SubThread thread)
        {
            var response = await container.CreateItemAsync(thread, new PartitionKey(thread.ThreadId));
            if (response.StatusCode != HttpStatusCode.Created)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Delete(string threadId, string parentThreadId)
        {
            var response = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(threadId));
            if(response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            var threadResponse = await container.DeleteItemAsync<SubThread>(threadId, new PartitionKey(parentThreadId));
            if(threadResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Update(SubThread thread)
        {
            var response = await container.ReplaceItemAsync(thread, thread.ThreadId, new PartitionKey(thread.Id));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            return true;
        }

        private readonly Container container;
    }
}
