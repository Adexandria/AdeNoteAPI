using ChattyPie.Models.DTOs;
using ChattyPie.Models;
using Microsoft.Azure.Cosmos;


namespace ChattyPie.Infrastructure.Repositories
{
    internal class SubThreadQuery
    {
        public SubThreadQuery(Database _database)
        {
            subThreadContainer = _database.CreateContainerIfNotExistsAsync("subthreads", "/threadId").Result;
        }

        public async Task<List<SubThreadDtos>> GetSubThread(string threadId, string query)
        {
            var iterator = subThreadContainer.GetItemQueryIterator<SubThread>(query, requestOptions: new QueryRequestOptions()
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

        private readonly Container subThreadContainer;
    }
}
