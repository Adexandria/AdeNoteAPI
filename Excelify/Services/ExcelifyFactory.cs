using System.Reflection;

namespace Excelify.Services
{
    public class ExcelifyFactory
    {
        public ExcelifyFactory(Assembly assembly = null)
        {
            var entryTypes = assembly?.GetTypes().Where(s => !s.IsAbstract
           && s.BaseType == typeof(ExcelService));
            if (entryTypes != null)
            {
                excelTypes.AddRange(entryTypes);
            }
            else
            {
                excelTypes.AddRange(Assembly.GetExecutingAssembly().GetTypes().Where(s => !s.IsAbstract
               && s.BaseType == typeof(ExcelService)));
            }
        }

        public IExcelService CreateService(string extensionType)
        {
            IExcelService excelService;
            try
            {
               var excelType = excelTypes.Where(s =>
               {
                   var newExcelType = Activator.CreateInstance(s) as IExcelService;
                   return newExcelType.CanImportSheet(extensionType);
               }).FirstOrDefault() ?? throw new Exception("Excel service does not exist");

               excelService = Activator.CreateInstance(excelType) as IExcelService;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create service", ex);
            }

            return excelService;
        }

        private readonly List<Type> excelTypes = new();
    }
}
