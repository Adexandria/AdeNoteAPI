using DocBuilder.Models;


namespace DocBuilder.Services
{
    public interface IDocService
    {
        Stream ExportToWord<T>(IEntityDoc<T> doc, Stream template) where T: class;

    }
}
