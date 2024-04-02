using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Excelify.Services;
using Microsoft.AspNetCore.Mvc.Versioning;
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
        public async Task<ActionResult<string>> ExportEntities(Guid userId, string extensionType,string sheetName)
        {
            var excelService = _excelifyFactory.CreateService(extensionType);
            
            try
            {
                if(excelService is ExcelifyService && string.IsNullOrEmpty(sheetName))
                {
                    throw new ArgumentException("Sheet name is invalid");
                }

                if (_bookService.GetAll(userId).Result.Data is IEnumerable<BookDTO> currentBooks)
                {
                    var exportEntity = new ExportEntity()
                    {
                        Entities = currentBooks.ToList(),
                        SheetName = sheetName
                    };

                    var mime = GetMimeType(extensionType);

                    var file = excelService.ExportToStream(exportEntity);

                    var url = await _blobService.UploadImage("AdeNote", file, mime);

                    return ActionResult<string>.SuccessfulOperation(url);
                }
                else
                {
                    return ActionResult<string>.Failed("Unable to export", StatusCodes.Status400BadRequest);
                }
            }
            catch (Exception ex)
            {
                var x = ex;
                return ActionResult<string>.Failed("Failed to export");
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

                return ActionResult.Failed("Failed to add books",StatusCodes.Status400BadRequest);
            }
            catch (Exception )
            {
                return ActionResult.Failed("Failed to import");
            }
            
        }

        private MimeType GetMimeType(string extensionType)
        {
            if (Enum.TryParse(extensionType, true, out MimeType mimeType)) 
            {
                return mimeType;
            }
            else
            {
                switch(extensionType)
                {
                    case string when extensionType.Equals(MimeType.xls.GetDescription()):
                        mimeType = MimeType.xls;
                        break;
                    case string when extensionType.Equals(MimeType.xlsx.GetDescription()):
                        mimeType = MimeType.xlsx;
                        break;
                    case string when extensionType.Equals(MimeType.csv.GetDescription()):
                        mimeType = MimeType.csv;
                        break;
                };

                return mimeType;

            }
        }

        private readonly ExcelifyFactory _excelifyFactory;

        private readonly IBookService _bookService;

        private readonly IBlobService _blobService;
    }
}
