using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using Microsoft.Azure.Cosmos;
using System.Net;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Infrastructure.Repositories
{
    internal class ThreadRepository : IThreadRepository
    {
        public ThreadRepository(Database _database)
        {
            container = _database.CreateContainerIfNotExistsAsync("threads", "/id").Result;
            subThreadContainer = _database.CreateContainerIfNotExistsAsync("subthreads", "/threadId").Result;
        }

        public async Task<bool> Add(Thread thread)
        {
            var response = await container.CreateItemAsync(thread);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> Delete(string threadId)
        {
            var response = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(threadId));
            if (response.StatusCode != HttpStatusCode.BadRequest)
            {
                return false;
            }

            var threadResponse = await container.DeleteItemAsync<Thread>(threadId, new PartitionKey(threadId));
            if (threadResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }
            return true;
        }

        public async Task<ThreadDto> GetThread(string threadId)
        {
            var response = await container.ReadItemAsync<Thread>(threadId, new PartitionKey(threadId));

            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return default;
            }

            var thread = response.Resource;
            // using mapping tool kekeke

            var threadDto = new ThreadDto()
            {
                Id = thread.Id,
                Message = thread.Message,
                UserIds = thread.UserIds,
                Date = thread.Created,
                SubThreads = await GetSubThread(threadId,
                "SELECT * FROM c ORDER BY c.created")
            };

            return threadDto;
        }

        public async Task<List<SubThreadDtos>> GetSubThread(string threadId, string query)
        {
            var iterator = subThreadContainer.GetItemQueryIterator<SubThread>(query,requestOptions: new QueryRequestOptions()
            {
                PartitionKey = new PartitionKey(threadId)
            });

            var subThreadDtos = new List<SubThreadDtos>();
            while (iterator.HasMoreResults)
            {
                var subThreads = await iterator.ReadNextAsync();
                foreach (var subThread in subThreads)
                {
                    var subThreadDto = new SubThreadDtos()
                    {
                        Id = subThread.Id,
                        Message = subThread.Message,
                        UserIds = subThread.UserIds,
                        SubUserIds = subThread.SubUserIds,
                        ThreadId = subThread.ThreadId,
                        Date = subThread.Created,
                        SubThreads = await GetSubThread(subThread.Id, query)
                    };

                    subThreadDtos.Add(subThreadDto);
                }
            }
            if (subThreadDtos.Any())
            {
                return subThreadDtos;
            }

            return default;
        }

        public async Task<bool> Update(Thread thread)
        {
          var response = await container.ReplaceItemAsync(thread, thread.Id, new PartitionKey(thread.Id));
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            return true;
        }

        private readonly Container subThreadContainer;
        private readonly Container container;
    }
}
