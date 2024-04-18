using Excelify.Models;
using Excelify.Services.Extensions;
using Excelify.Services.Utility;
using Excelify.Services.Utility.Attributes;
using System.ComponentModel;
using System.Data;

namespace Excelify.Services
{
    public class CsvService : ExcelService
    {
        public CsvService()
        {
            _excelifyMapper = new ExcelifyMapper();
        }


        public override bool CanImportSheet(string extensionType)
        {
            if (string.IsNullOrEmpty(extensionType))
                throw new ArgumentNullException(nameof(extensionType), "Extension type can not be empty");

           return ExtensionType.csv.ToString() == extensionType
                || extensionType.Equals(ExtensionType.csv.GetDescription());
        }

        public override byte[] ExportToBytes<T>(ISheetExport<T> dataExport)
        {
            var extractedAttributes = ExcelifyRecord.GetAttributeProperty<ExcelifyAttribute, T>();
            return dataExport.CreateCsvBytes(extractedAttributes);
        }

        public override Stream ExportToStream<T>(ISheetExport<T> dataExport)
        {
            var extractedAttributes = ExcelifyRecord.GetAttributeProperty<ExcelifyAttribute, T>();
            return dataExport.CreateCsvSheet(extractedAttributes);
        }

        public override DataTable ImportToTable(ISheetImport sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            return sheet.ExtractCsvValues();
        }

        public override IList<T> ImportToEntity<T>(ISheetImport sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            var extractedValues = sheet.ExtractCsvValues();
            var entities = _excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;

        }

        public override IList<T> ImportToEntity<T>(ISheetImport sheet, IExcelMapper excelifyMapper)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            if(excelifyMapper == null)
                throw new ArgumentNullException(nameof(excelifyMapper), "Excel mapper can not be null");

            var extractedValues = sheet.ExtractCsvValues();
            var entities = excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;
        }

        private readonly ExcelifyMapper _excelifyMapper;
    }
}
