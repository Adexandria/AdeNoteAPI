using Excelify.Models;
using Excelify.Services.Utility;
using Microsoft.VisualBasic.FileIO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Reflection;

namespace Excelify.Services.Extensions
{
    internal static class SheetExtension
    {
        public static DataTable ExtractSheetValues(this IImportSheet excelSheet)
        {
            ISheet sheet;

            DataTable table = new();

            List<string> rowList = new();

            XSSFWorkbook workBook = new(excelSheet.File)
            {
                MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK
            };

            sheet = workBook.GetSheetAt(excelSheet.SheetName);

            IRow headerRow = sheet.GetRow(0);

            int cellCount = headerRow.LastCellNum;

            for (int i = 0; i < cellCount; i++)
            {
                ICell cell = headerRow.GetCell(i);
                if (cell == null || string.IsNullOrEmpty(cell.ToString()))
                    continue;

                table.Columns.Add(cell.ToString());
            }

            for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null || row.Cells.TrueForAll(d => d.CellType == CellType.Blank))
                    continue;

                ExtractColumnValues(row, row.FirstCellNum, cellCount, rowList);

                if (rowList.Count > 0)
                    table.Rows.Add(rowList.ToArray());

                rowList.Clear();
            }

            return table;
        }

        public static DataTable ExtractCsvValues(this IImportSheet excelSheet)
        {
            DataTable table = new();

            using var parser = new TextFieldParser(excelSheet.File);

            parser.SetDelimiters(",");

            var headers = parser.ReadFields() ?? throw new Exception("Sheet can not be empty");

            foreach (string header in  headers)
            {
                table.Columns.Add(header);
            }

            while (!parser.EndOfData)
            {
                string[] rows = parser.ReadFields();

                if(rows.Length == 0)
                {
                    continue;
                }

                table.Rows.Add(rows);
            }

            return table;
        }

        public static XSSFWorkbook CreateSheet<T>(this IEntityExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
        {
            var workBook = new XSSFWorkbook();
            var workSheet = workBook.CreateSheet(dataExport.SheetName);
            var headerRow = workSheet.CreateRow(0);
            for (int i = 0; i < extractedAttributes.Count; i++)
            {
                if (extractedAttributes[i].AttributeName is int value)
                {
                    headerRow.CreateCell(value, CellType.String)
                       .SetCellValue(extractedAttributes[i].PropertyName);
                    InsertValues(workSheet, dataExport.Entities,
                        extractedAttributes[i].PropertyName, i);
                }
                else
                {
                    headerRow.CreateCell(i, CellType.String)
                         .SetCellValue((string)extractedAttributes[i].AttributeName);
                    InsertValues(workSheet, dataExport.Entities,
                       extractedAttributes[i].PropertyName, i);
                }
            }

            return workBook;
        }

        public static Stream CreateCsvSheet<T>(this IEntityExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
        {
            return WriteToCsv(dataExport, extractedAttributes);
        } 

        public static byte[] CreateCsvBytes<T>(this IEntityExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
        {
            var ms = WriteToCsv(dataExport, extractedAttributes);
            return ms.ToArray();
        }

        public static void WriteToFile(this byte[] workbook, string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "File name can not be empty");
            }

            using var fileStream = new FileStream($"{path}.xlsx", FileMode.Create, FileAccess.Write);

            fileStream.Write(workbook, 0, workbook.Length);
        }

        private static MemoryStream WriteToCsv<T>(IEntityExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
        {
            var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, null, -1, true);
            for (int i = 0; i < extractedAttributes.Count; i++)
            {
                if (extractedAttributes[i].AttributeName is string value)
                {
                    if (i > 0)
                    {
                        writer.Write(',');
                    }
                    writer.Write(value);
                }
            }

            writer.WriteLine();
            for (int j = 0; j < dataExport.Entities.Count; j++)
            {
                var properties = dataExport.Entities[j].GetType().GetProperties();
                var extractedProperties = properties.Where(s => extractedAttributes.Any(p => p.PropertyName == s.Name)).ToList();
                if (properties == null)
                {
                    continue;
                }
                WriteToCsvColumns(writer, dataExport.Entities[j], extractedProperties);
                writer.WriteLine();
            }
            writer.Close();
            ms.Position = 0;
            return ms;
        }

        private static void WriteToCsvColumns<T>(StreamWriter writer, T entity, List<PropertyInfo> propertyInfos, int left = 0)
        {
            if(left < propertyInfos.Count)
            {
                var value = propertyInfos[left].GetValue(entity);

                if (left > 0)
                {
                    writer.Write(',');
                }
                writer.Write(value);

                left++;

                WriteToCsvColumns(writer, entity, propertyInfos, left);
            }
        }

        private static void InsertValues<T>(ISheet sheet, IList<T> entities, string propertyName, int cellNumber, int rowNumber = 1)
        {
            if (rowNumber <= entities.Count)
            {
                var column = sheet.GetRow(rowNumber) ?? sheet.CreateRow(rowNumber);
                var entity = entities[rowNumber - 1];
                var property = entity?.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    return;
                }
                var row = column.CreateCell(cellNumber);

                switch (property.PropertyType )
                {
                    case Type when property.PropertyType == typeof(int) :

                        row.SetCellType(CellType.Numeric);
                            row.SetCellValue((int)property.GetValue(entity));
                    break;

                    case Type when property.PropertyType == typeof(double):
                        row.SetCellType(CellType.Numeric);
                         row.SetCellValue((double)property.GetValue(entity));
                    break;

                    case Type when property.PropertyType == typeof(DateTime):
                        var value = (DateTime)property.GetValue(entity);

                        row.SetCellType(CellType.String);
                            row.SetCellValue(value.ToString());
                        break;

                    case Type when property.PropertyType == typeof(bool):
                        row.SetCellType(CellType.Boolean);
                             row.SetCellValue((bool)property.GetValue(entity));
                        break;

                    case Type when property.PropertyType == typeof(Guid):
                         var guid = (Guid)property.GetValue(entity);

                        row.SetCellType(CellType.String);
                        row.SetCellValue(guid.ToString("N"));
                    break;

                    default:
                        row.SetCellType(CellType.String);
                        row.SetCellValue((string)property.GetValue(entity));
                        break;
                }

                rowNumber++;

                InsertValues(sheet, entities, propertyName, cellNumber, rowNumber);
            }
        }

        private static void ExtractColumnValues(IRow row, int rowNumber, int numberOfCells, List<string> rowList)
        {
            if (rowNumber < numberOfCells)
            {
                var cell = row.GetCell(rowNumber);

                if (cell == null)
                {
                    rowList.Add(null);
                }
                else
                {
                    rowList.Add(cell.ToString());
                }

                rowNumber++;
                ExtractColumnValues(row, rowNumber, numberOfCells, rowList);
            }
        }
    }
}
