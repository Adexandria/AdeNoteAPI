using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using Microsoft.Azure.Cosmos;
using System.Net;


namespace ChattyPie.Infrastructure.Repositories
{
    internal class SubThreadRepository : ISubThreadRepository
    {
        public SubThreadRepository(Database database, SubThreadQuery _subThreadQuery)
        {
            container = database.CreateContainerIfNotExistsAsync("subthreads", "/threadId").Result;
            subThreadQuery = _subThreadQuery;
        }
        public async Task<SubThreadDtos> Add(SubThread thread)
        {
            try
            {
                var response = await container.CreateItemAsync(thread, new PartitionKey(thread.ThreadId));

                var subThread = response.Resource;

                var subThreadDto = new SubThreadDtos()
                {
                    Id = subThread.Id,
                    Message = subThread.Message,
                    UserIds = subThread.UserIds,
                    SubUserIds = subThread.SubUserIds,
                    ThreadId = subThread.ThreadId,
                    Date = subThread.Created
                };

                return subThreadDto;
            }
            catch (Exception)
            {
                return default;
            }
            
        }

        public async Task<bool> Delete(string threadId, string parentThreadId)
        {
            try
            {
                _  = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(threadId));

                var r = await container.DeleteItemAsync<SubThread>(threadId, new PartitionKey(parentThreadId));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<SubThreadDtos> Update(SubThread subThread)
        {
            try
            {
                 var response = await container.ReplaceItemAsync(subThread, subThread.Id, new PartitionKey(subThread.ThreadId));

                var subThreadDto = new SubThreadDtos()
                {
                    Id = subThread.Id,
                    Message = subThread.Message,
                    UserIds = subThread.UserIds,
                    SubUserIds = subThread.SubUserIds,
                    ThreadId = subThread.ThreadId,
                    Date = subThread.Created
                };

                return subThreadDto;
            }
            catch (Exception)
            {
                return default;
            }
            
        }

        public async Task<SubThreadDtos> GetSubThreadAsync(string subThreadId, string parentThreadId)
        {
            try
            {
                var response = await container.ReadItemAsync<SubThread>(subThreadId, new PartitionKey(parentThreadId));

                var subThread = response.Resource;

                var subThreadDto = new SubThreadDtos()
                {
                    Id = subThread.Id,
                    Message = subThread.Message,
                    UserIds = subThread.UserIds,
                    SubUserIds = subThread.SubUserIds,
                    ThreadId = subThread.ThreadId,
                    Date = subThread.Created,
                    SubThreads = await subThreadQuery.GetSubThread(subThread.Id, "SELECT * FROM c ORDER BY c.created")
                };

                return subThreadDto;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<SubThread> GetSingleSubThread(string subThreadId, string parentThreadId)
        {
            try
            {
                var response = await container.ReadItemAsync<SubThread>(subThreadId, new PartitionKey(parentThreadId));

                return response.Resource;
            }
            catch (Exception)
            {
                return default;
            }
           
        }

        private readonly SubThreadQuery subThreadQuery;
        private readonly Container container;
    }
}
