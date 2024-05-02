using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Excelify.Services;


namespace AdeNote.Infrastructure.Services
{
    public class ExcelService : IExcel
    {
        public ExcelService(ExcelifyFactory excelifyFactory, IBookService bookService)
        {
             _excelifyFactory = excelifyFactory;
            _bookService = bookService;
        }
        public Stream ExportEntities<T>(string extensionType,string name,IEnumerable<T> entities) where T : class
        {
            var excelService = _excelifyFactory.CreateService(extensionType);
            Stream file;
            try
            {
                if(excelService is ExcelifyService && string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Sheet name is invalid");
                }

                var exportEntity = new ExportEntity<T>()
                {
                  Entities = entities.ToList(),
                  SheetName = name
                };

                file = excelService.ExportToStream(exportEntity);

                return file;

            }
            catch (Exception ex )
            {
                throw new Exception(ex.Message,ex);
            }
        }

        public async Task<ActionResult> ImportEntities(ImportBookDto importBookDto)
        {
            var excelService = _excelifyFactory.CreateService(importBookDto.ContentType);

            try
            {
                var newBooks = excelService.ImportToEntity<BookCreateDTO>(new ImportSheet(importBookDto.File, importBookDto.SheetName));

                if (newBooks.Count < 0)
                {
                    return ActionResult.Failed("Cannot import empty sheet", StatusCodes.Status400BadRequest);
                }

                var response = await _bookService.Add(importBookDto.UserId, newBooks);


                if (response.IsSuccessful)
                {
                    return ActionResult.Successful();
                }

                return ActionResult.Failed("Failed to add books", StatusCodes.Status400BadRequest);
            }
            catch (Exception)
            {
                return ActionResult.Failed("Failed to import");
            }
        }

        private readonly ExcelifyFactory _excelifyFactory;

        private readonly IBookService _bookService;
    }
}
