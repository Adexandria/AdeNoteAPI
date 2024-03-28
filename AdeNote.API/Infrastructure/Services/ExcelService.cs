using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Excelify.Services;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class ExcelService : IExcel
    {
        public ExcelService(ExcelifyFactory excelifyFactory, IBookService bookService, IBlobService blobService)
        {
             _excelifyFactory = excelifyFactory;
            _bookService = bookService;
            _blobService = blobService;
        }
        public async Task<ActionResult<string>> ExportEntities(string sheetName,Guid userId, string extensionType)
        {
            var excelService = _excelifyFactory.CreateService(extensionType);
            
            try
            {
                if (_bookService.GetAll(userId).Result.Data is IEnumerable<BookDTO> currentBooks)
                {
                    var exportEntity = new ExportEntity()
                    {
                        Entities = currentBooks.ToList(),
                        SheetName = sheetName
                    };

                    var file = excelService.ExportToStream(exportEntity);

                    var url = await _blobService.UploadImage("AdeNote", file, MimeType.xlsx);

                    return ActionResult<string>.SuccessfulOperation(url);
                }
                else
                {
                    return ActionResult<string>.Failed("Unable to export", StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception)
            {
                return ActionResult<string>.Failed("Failed to export");
            }
        }

        public async Task<ActionResult> ImportEntities(ImportBookDto importBookDto)
        {
            var excelService = _excelifyFactory.CreateService(importBookDto.ContentType);

            try
            {
                excelService.SetSheetName(importBookDto.SheetName);

                var newBooks = excelService.ImportToEntity<BookCreateDTO>(new ImportSheet(importBookDto.File));

                if (newBooks.Count < 0)
                {
                    return ActionResult.Failed("Cannot import empty sheet", StatusCodes.Status400BadRequest);
                }

                var response = await _bookService.Add(importBookDto.UserId, newBooks);


                if (response.IsSuccessful)
                {
                    return ActionResult.Successful();
                }

                return ActionResult.Failed("Failed to add books",StatusCodes.Status400BadRequest);
            }
            catch (Exception )
            {
                return ActionResult.Failed("Failed to import");
            }
            
        }

        private readonly ExcelifyFactory _excelifyFactory;

        private readonly IBookService _bookService;

        private readonly IBlobService _blobService;
    }
}
