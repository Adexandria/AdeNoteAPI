using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models.DTOs;
using Microsoft.Azure.Cosmos;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Infrastructure.Repositories
{
    internal class ThreadRepository : IThreadRepository
    {
        public ThreadRepository(Database _database, ThreadQuery _subThreadQuery)
        {
            container = _database.CreateContainerIfNotExistsAsync("threads", "/id").Result;
            subThreadQuery = _subThreadQuery;
        }

        public async Task<ThreadDtos> Add(Thread thread)
        {
            try
            {
                _ = await container.CreateItemAsync(thread);
                var threadDto = new ThreadDtos()
                {
                    Id = thread.Id,
                    Message = thread.Message,
                    UserIds = thread.UserIds,
                    Date = thread.Created,
                    LastModified = thread.Modified
                };
                return threadDto;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<bool> Delete(string threadId)
        {
            try
            {
                _ = await container.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(threadId));

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<ThreadDtos> GetThread(string threadId)
        {
            try
            {
                var response = await container.ReadItemAsync<Thread>(threadId, new PartitionKey(threadId));
                var thread = response.Resource;

                var threadDto = new ThreadDtos()
                {
                    Id = thread.Id,
                    Message = thread.Message,
                    UserIds = thread.UserIds,
                    Date = thread.Created,
                    LastModified = thread.Modified,
                    SubThreads = await subThreadQuery.GetSubThread(threadId,
                    "SELECT * FROM c ORDER BY c.created")
                };

                return threadDto;
            }
            catch (Exception)
            {
                return default;
            }
            
        }

        public async Task<ThreadDtos> Update(Thread thread)
        {
            try
            {
                _ = await container.ReplaceItemAsync(thread, thread.Id, new PartitionKey(thread.Id));

                var threadDto = new ThreadDtos()
                {
                    Id = thread.Id,
                    Message = thread.Message,
                    UserIds = thread.UserIds,
                    Date = thread.Created,
                    LastModified = thread.Modified
                };

                return threadDto;
            }
            catch (Exception)
            {
                return default;
            }
           
           
        }

        public async Task<List<ThreadDto>> GetThreads()
        {
            try
            {
                var iterator = container.GetItemQueryIterator<Thread>("SELECT * FROM c ORDER BY c.created");

                var threadDtos = new List<ThreadDto>();
                while (iterator.HasMoreResults)
                {
                    var threads = await iterator.ReadNextAsync();
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

                if (threadDtos.Count < 0)
                {
                    return default;
                }

                return threadDtos;
            }
            catch (Exception)
            {
                return default;
            }
        }

        public async Task<Thread> GetSingleThread(string threadId)
        {
            try
            {
                var response = await container.ReadItemAsync<Thread>(threadId, new PartitionKey(threadId));

                return response.Resource;
            }
            catch (Exception)
            {
                return default;
            }
        }

        private readonly ThreadQuery subThreadQuery;
        private readonly Container container;
    }
}
