using ChattyPie.Models.DTOs;
using ChattyPie.Models;
using Microsoft.Azure.Cosmos;
using Thread = ChattyPie.Models.Thread;
using Microsoft.Azure.Cosmos.Linq;
using Container = Microsoft.Azure.Cosmos.Container;
using System.Linq.Expressions;


namespace ChattyPie.Infrastructure.Repositories
{
    internal class ThreadQuery
    {
        public ThreadQuery(Database _database)
        {
            threadContainer = _database.CreateContainerIfNotExistsAsync("threads", "/id").Result;
            subThreadContainer = _database.CreateContainerIfNotExistsAsync("subthreads", "/threadId").Result;
        }

        public async Task<List<SubThreadDtos>> GetSubThread(string threadId, string query)
        {
            try
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
                            LastModified = subThread.Modified,
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
            catch (Exception)
            {
                return default;
            }
           
        }


        public async Task<List<ThreadDto>> SearchThreadsByMessage(string message)
        {
            try
            {
                var query = threadContainer.GetItemLinqQueryable<Thread>(allowSynchronousQueryExecution:true)
                    .Where(x => x.Message.Contains(message,StringComparison.CurrentCultureIgnoreCase));

                var threads = await FetchThreads(query);

                return threads;
            }
            catch (Exception)
            {
                return default;
            }
          
        }


        public async Task<List<ThreadDto>> SearchThreadsByUserId(string userId)
        {
            try
            {
                var query = threadContainer.GetItemLinqQueryable<Thread>(allowSynchronousQueryExecution:true)
                    .Where(s=>s.UserIds.Contains(userId));

                var threads = await FetchThreads(query);

                return threads;
            }
            catch (Exception)
            {
                return default;
            }
            
        }


        private async Task<List<ThreadDto>> FetchThreads<TModel>(IQueryable<TModel> query)
            where TModel : Thread
        {
            var threadDtos = new List<ThreadDto>();

            var threadQuery = query.ToFeedIterator();
            
            while (threadQuery.HasMoreResults)
            {
                var threads = await threadQuery.ReadNextAsync();

                foreach (var thread in threads)
                {
                    var threadDto = new ThreadDto()
                    {
                        Date = thread.Created,
                        Id = thread.Id,
                        Message = thread.Message,
                        UserIds = thread.UserIds,
                        LastModified = thread.Modified
                    };

                    threadDtos.Add(threadDto);
                }
            }


            return threadDtos.Any()? threadDtos : default;
        }


        private readonly Container threadContainer;
        private readonly Container subThreadContainer;
    }
}
