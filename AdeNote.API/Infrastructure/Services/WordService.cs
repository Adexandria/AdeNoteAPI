using AdeNote.Infrastructure.Utilities;
using DocBuilder.Services;


namespace AdeNote.Infrastructure.Services
{
    public class WordService : IWordService
    {
        public WordService(IDocService docService)
        {
            _docService = docService; 
        }
        public Stream ExportToWord<T>(string name, IEnumerable<T> entities, Stream template)
            where T : class
        {
            try
            {
               var entityDoc = new EntityDoc<T>()
               { 
                 Entities = entities.ToList(),
                 Name = name
               };

               var file = _docService.ExportToWord(entityDoc, template);
               return file;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private readonly IDocService _docService;
    }
}
