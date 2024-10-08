﻿using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Services.Excel;
using AdeNote.Infrastructure.Services.Word;
using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services.Export
{
    public class ExportService : IExportService
    {

        public ExportService(IBlobService blobService, IWordService wordService, IExcel excelService)
        {
            _blobService = blobService;
            _wordService = wordService;
            _excelService = excelService;
        }



        public async Task<ActionResult<string>> ExportEntities<T>(string extensionType, string name, IEnumerable<T> entities, CancellationToken cancellationToken)
            where T : class
        {
            var mime = GetMimeType(extensionType);

            if (mime == MimeType.none)
            {
                return ActionResult<string>.Failed("Unsupported mime type", StatusCodes.Status400BadRequest);
            }

            Stream file;

            if (mime == MimeType.docx)
            {
                var template = await _blobService.DownloadStream("AdenoteLetterHead", cancellationToken, MimeType.docx) ?? throw new NullReferenceException("AdenoteLetterHead");
                file = _wordService.ExportToWord(name, entities, template);
            }
            else
            {
                file = _excelService.ExportEntities(extensionType, name, entities);
            }

            cancellationToken.ThrowIfCancellationRequested();

            var url = await _blobService.UploadImage(name, file,cancellationToken ,mime);

            if(string.IsNullOrEmpty(url))
            {
                return ActionResult<string>.Failed($"{url}, try {MimeType.docx} format",400);
            }

            return ActionResult<string>.SuccessfulOperation(url);

        }

        private MimeType GetMimeType(string extensionType)
        {
            if (Enum.TryParse(extensionType, true, out MimeType mimeType))
            {
                return mimeType;
            }
            else
            {
                switch (extensionType)
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
                    default:
                        mimeType = MimeType.none;
                        break;
                }

                return mimeType;

            }
        }

        private readonly IBlobService _blobService;
        private readonly IWordService _wordService;
        private readonly IExcel _excelService;
    }
}
