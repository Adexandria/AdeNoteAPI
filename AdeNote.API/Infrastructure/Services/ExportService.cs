using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Utilities;
using Excelify.Services;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class ExportService : IExportService
    {
       
        public ExportService(IBlobService blobService, IWordService wordService, IExcel excelService)
        {
            _blobService = blobService;
            _wordService = wordService;
            _excelService = excelService;
        }

       

        public async Task<ActionResult<string>> ExportEntities<T>(string extensionType, string name, IEnumerable<T> entities) 
            where T : class
        {
            try
            {
                Stream file;

                if (extensionType == MimeType.docx.ToString())
                {
                    var template = await _blobService.DownloadStream("AdenoteLetterHead", MimeType.docx);
                    file = _wordService.ExportToWord(name, entities,template);
                }
                else
                {
                    file = _excelService.ExportEntities(extensionType, name, entities);
                }

                var mime = GetMimeType(extensionType);

                var url = await _blobService.UploadImage(name, file, mime);

                return ActionResult<string>.SuccessfulOperation(url);
            }
            catch (Exception)
            {
                return ActionResult<string>.Failed("Failed to export");
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
                switch (extensionType)
                {
                    case string when extensionType.Equals(MimeType.xls.GetDescription()):
                        mimeType = MimeType.xls;
                        break;
                    case string when extensionType.Equals(MimeType.xlsx.GetDescription()):
                        mimeType = MimeType.xlsx;
                        break;
                    case string when extensionType.Equals(MimeType.csv.GetDescription()):
                        break;
                    default:
                        mimeType = MimeType.csv;
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
