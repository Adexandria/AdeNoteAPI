using Excelify.Models;
using Excelify.Services.Extensions;
using Excelify.Services.Utility;
using Excelify.Services.Utility.Attributes;
using System.Data;

namespace Excelify.Services
{
    public class CsvService : ExcelService
    {
        public override bool CanImportSheet(string extensionType)
        {
            if (string.IsNullOrEmpty(extensionType))
                throw new ArgumentNullException(nameof(extensionType), "Extension type can not be empty");

            return ExtensionType.csv.Equals(extensionType);
        }

        public override byte[] Export<T>(IEntityExport<T> dataExport)
        {
            var extractedAttributes = ExcelifyRecord.GetAttributeProperty<ExcelifyAttribute, T>();
        }

        public override Stream ExportToStream<T>(IEntityExport<T> dataExport)
        {
            throw new NotImplementedException();
        }

        public override DataTable ImportSheet(IImportSheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            return sheet.ExtractCsv();
        }

        public override IList<T> ImportToEntity<T>(IImportSheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            var extractedValues = sheet.ExtractCsv();
            var entities = _excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;

        }

        public override IList<T> ImportToEntity<T>(IImportSheet sheet, IExcelMapper excelifyMapper)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "sheet can not be null");

            if(excelifyMapper == null)
                throw new ArgumentNullException(nameof(excelifyMapper), "Excel mapper can not be null");

            var extractedValues = sheet.ExtractCsv();
            var entities = excelifyMapper.Map<T>(extractedValues.Rows.OfType<DataRow>()).Result;
            return entities;
        }

        public override void SetSheetName(int sheetName)
        {
            if (sheetName < 0)
                throw new ArgumentNullException(nameof(sheetName), "Invalid sheet name");

            _sheetName = sheetName;
        }

        private ExcelifyMapper _excelifyMapper;
        private int _sheetName;
    }
}
