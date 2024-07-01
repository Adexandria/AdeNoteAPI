using AdeNote.Infrastructure.Services.BookSetting;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.ExcelSettings;
using AdeNote.Models.DTOs;
using Excelify.Services;


namespace AdeNote.Infrastructure.Services.Excel
{
    public class ExcelService : IExcel
    {
        public ExcelService(ExcelifyFactory excelifyFactory, IBookService bookService)
        {
            _excelifyFactory = excelifyFactory;
            _bookService = bookService;
        }
        public Stream ExportEntities<T>(string extensionType, string name, IEnumerable<T> entities) where T : class
        {
            var excelService = _excelifyFactory.CreateService(extensionType);
            Stream file;
            if (excelService is ExcelifyService && string.IsNullOrEmpty(name))
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

        public async Task<ActionResult> ImportEntities(ImportBookDto importBookDto)
        {
            var excelService = _excelifyFactory.CreateService(importBookDto.ContentType);
            var newBooks = excelService.ImportToEntity<BookCreateDTO>(new ImportSheet(importBookDto.File, importBookDto.SheetName));

            if (newBooks.Count < 0)
            {
                return ActionResult.Failed("Cannot import empty sheet", StatusCodes.Status400BadRequest);
            }

            var response = await _bookService.Add(importBookDto.UserId, newBooks);


            if (response.IsSuccessful)
            {
                return ActionResult.SuccessfulOperation();
            }

            return ActionResult.Failed("Failed to add books", StatusCodes.Status400BadRequest);
        }

        private readonly ExcelifyFactory _excelifyFactory;

        private readonly IBookService _bookService;
    }
}
