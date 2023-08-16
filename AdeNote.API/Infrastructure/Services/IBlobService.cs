namespace AdeNote.Infrastructure.Services
{
    public interface IBlobService
    {
        Task<string> UploadFile(string fileName, Stream file);
    }
}
