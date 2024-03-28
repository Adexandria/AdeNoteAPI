using Excelify.Services.Utility.Attributes;

namespace AdeNote.Models.DTOs
{
    public class ImportBookDto
    {
        public ImportBookDto(int sheetName, Stream file, string contentType, Guid userId)
        {
            SheetName = sheetName ;
            File = file ?? throw new NullReferenceException(nameof(sheetName), new Exception("file can not be empty"));
            ContentType = contentType ?? throw new NullReferenceException(nameof(sheetName), new Exception("Content type can not be empty"));
            UserId = userId;
        }

        public Stream File { get; set; }
        public int SheetName {  get; set; }
        public string ContentType { get; set; }
        public Guid UserId { get; set; }
    }
}
