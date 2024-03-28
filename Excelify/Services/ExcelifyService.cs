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
        public override void SetSheetName(int sheetName)
        {
            if (sheetName < 0)
                throw new ArgumentNullException(nameof(sheetName), "Invalid sheet name");

            _sheetName = sheetName;
            _excelifyMapper = new ExcelifyMapper();
        }

        public override bool CanImportSheet(string extensionType)
        {
            if (string.IsNullOrEmpty(extensionType))
                throw new ArgumentNullException(nameof(extensionType), "Extension type can not be empty");

            return extensionType.Equals(ExtensionType.xls.GetDescription<DescriptionAttribute>()) ||
                extensionType.Equals(ExtensionType.xlsx.GetDescription<DescriptionAttribute>());
        }


        public override DataTable ImportSheet(IImportSheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            return sheet.ExtractValues(_sheetName);
        }

        public override IList<T> ImportToEntity<T>(IImportSheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            var extractedValues = sheet.ExtractValues(_sheetName);
            var entities = _excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;
        }

        public override IList<T> ImportToEntity<T>(IImportSheet sheet, IExcelMapper excelifyMapper)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            if (excelifyMapper == null)
                throw new ArgumentNullException(nameof(excelifyMapper), "Excel mapper can not be null");

            var extractedValues = sheet.ExtractValues(_sheetName);
            var entities = excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;
        }

        public override byte[] Export<T>(IEntityExport<T> dataExport)
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

       
        private int _sheetName; 
        private IExcelMapper _excelifyMapper;
    }
}
