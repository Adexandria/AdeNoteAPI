using Excelify.Models;
using Excelify.Services.Extensions;
using Excelify.Services.Utility;
using Excelify.Services.Utility.Attributes;
using System.ComponentModel;
using System.Data;

namespace Excelify.Services
{
    public class ExcelifyService : ExcelService
    {
        public ExcelifyService()
        {
            _excelifyMapper = new ExcelifyMapper();
        }

        public override bool CanImportSheet(string extensionType)
        {
            if (string.IsNullOrEmpty(extensionType))
                throw new ArgumentNullException(nameof(extensionType), "Extension type can not be empty");

            return extensionType.Equals(ExtensionType.xls.GetDescription<DescriptionAttribute>()) ||
                extensionType.Equals(ExtensionType.xlsx.GetDescription<DescriptionAttribute>()) 
                || extensionType.Equals(ExtensionType.xls.ToString()) 
                || extensionType.Equals(ExtensionType.xlsx.ToString());
        }

        public override DataTable ImportToTable(IImportSheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            return sheet.ExtractSheetValues();
        }

        public override IList<T> ImportToEntity<T>(IImportSheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            var extractedValues = sheet.ExtractSheetValues();
            var entities = _excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;
        }

        public override IList<T> ImportToEntity<T>(IImportSheet sheet, IExcelMapper excelifyMapper)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            if (excelifyMapper == null)
                throw new ArgumentNullException(nameof(excelifyMapper), "Excel mapper can not be null");

            var extractedValues = sheet.ExtractSheetValues();
            var entities = excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;
        }

        public override byte[] ExportToBytes<T>(IEntityExport<T> dataExport)
        {
            var extractedAttributes = ExcelifyRecord.GetAttributeProperty<ExcelifyAttribute, T>();

            var excelSheet = dataExport.CreateSheet(extractedAttributes);

            using var memoryStream = new MemoryStream();

            excelSheet.Write(memoryStream);

            return memoryStream.ToArray();
        }

        public override Stream ExportToStream<T>(IEntityExport<T> dataExport)
        {
            var extractedAttributes = ExcelifyRecord.GetAttributeProperty<ExcelifyAttribute, T>();

            var excelSheet = dataExport.CreateSheet(extractedAttributes);

            var memoryStream = new MemoryStream();

            excelSheet.Write(memoryStream,true);

            memoryStream.Position = 0;

            return memoryStream;
        }

        private readonly IExcelMapper _excelifyMapper;
    }
}
