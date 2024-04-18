using Excelify.Models;
using Excelify.Services.Utility;
using Microsoft.VisualBasic.FileIO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;
using System.Reflection;

namespace Excelify.Services.Extensions
{
    /// <summary>
    /// Extensions method only available to excelify
    /// </summary>
    internal static class SheetExtension
    {
        /// <summary>
        /// Extracts values from xls or xlsx sheet
        /// </summary>
        /// <param name="excelSheet">A model used to import sheet</param>
        /// <returns>Extracted values in table format</returns>
        public static DataTable ExtractSheetValues(this ISheetImport excelSheet,string extensionType)
        {
            ISheet sheet;

            DataTable table = new();

            List<string> rowList = new();
            IWorkbook workBook;

            if(ExtensionType.xls.ToString() == extensionType || ExtensionType.xls.GetDescription() == extensionType)
            {
                workBook = new HSSFWorkbook(excelSheet.File)
                {
                    MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK
                };
            }
            else
            {
                workBook = new XSSFWorkbook(excelSheet.File)
                {
                    MissingCellPolicy = MissingCellPolicy.RETURN_NULL_AND_BLANK
                };
            }
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

        /// <summary>
        /// Extracts values from csv sheet
        /// </summary>
        /// <param name="excelSheet">Excel sheet to extract values from</param>
        /// <returns>Extracted values in table format</returns>
        /// <exception cref="Exception"></exception>
        public static DataTable ExtractCsvValues(this ISheetImport excelSheet)
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

        /// <summary>
        /// Create xlsx or xls sheet and insert values
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="dataExport">Entities to export</param>
        /// <param name="extractedAttributes">Properties to extract</param>
        /// <returns>Workbook</returns>
        public static IWorkbook CreateSheet<T>(this ISheetExport<T> dataExport, List<ExcelifyProperty> extractedAttributes,string extensionType)
        {
            IWorkbook workBook;

            if (ExtensionType.xls.ToString() == extensionType || ExtensionType.xls.GetDescription() == extensionType)
            {
                workBook = new HSSFWorkbook();
            }
            else
            {
                workBook = new XSSFWorkbook();
            }
            var workSheet = workBook.CreateSheet(dataExport.SheetName);
            var headerRow = workSheet.CreateRow(0);
            for (int i = 0; i < extractedAttributes.Count; i++)
            {
                if (extractedAttributes[i].AttributeName is int value)
                {
                    headerRow.CreateCell(value, CellType.String)
                       .SetCellValue(extractedAttributes[i].PropertyName);
                    InsertValues(workSheet, dataExport.Entities,
                        extractedAttributes[i].PropertyName, value);
                }
                else
                {
                    headerRow.CreateCell(i, CellType.String)
                         .SetCellValue((string)extractedAttributes[i].AttributeName);
                    InsertValues(workSheet, dataExport.Entities,
                       extractedAttributes[i].PropertyName, i);
                }

                workSheet.AutoSizeColumn(i);
            }

            return workBook;
        }

        /// <summary>
        /// Creates Csv sheet and insert values
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="dataExport">Entities to export</param>
        /// <param name="extractedAttributes">Properties to extract</param>
        /// <returns>Stream of inserted data</returns>
        public static Stream CreateCsvSheet<T>(this ISheetExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
        {
            return WriteToCsv(dataExport, extractedAttributes);
        }

        /// <summary>
        /// Creates Csv sheet and insert values
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="dataExport">Entities to export</param>
        /// <param name="extractedAttributes">Properties to extract</param>
        /// <returns>bytes array of data</returns>
        public static byte[] CreateCsvBytes<T>(this ISheetExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
        {
            var ms = WriteToCsv(dataExport, extractedAttributes);
            return ms.ToArray();
        }

        /// <summary>
        /// Writes byte array of data to file
        /// </summary>
        /// <param name="workbook">Values to write</param>
        /// <param name="path">Path to staore file</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteToFile(this byte[] workbook, string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path), "File name can not be empty");
            }

            using var fileStream = new FileStream($"{path}.xlsx", FileMode.Create, FileAccess.Write);

            fileStream.Write(workbook, 0, workbook.Length);
        }

        /// <summary>
        /// Insert values into csv format
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="dataExport">Entities to export</param>
        /// <param name="extractedAttributes">Properties to extract</param>
        /// <returns>Strean of data</returns>
        private static MemoryStream WriteToCsv<T>(ISheetExport<T> dataExport, List<ExcelifyProperty> extractedAttributes)
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

        /// <summary>
        /// Insert values into csv columns
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="writer">Writes data into stream</param>
        /// <param name="entity">Enity to extract values from</param>
        /// <param name="propertyInfos">Property of entity</param>
        /// <param name="left">number of interation</param>
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


        /// <summary>
        /// Insert values into xlsx or xls sheet
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="sheet">Sheet to insert into</param>
        /// <param name="entities">Entities to extract values from</param>
        /// <param name="propertyName">Name of the propery to extract</param>
        /// <param name="columnNumber">Current column number</param>
        /// <param name="rowNumber">Current row number</param>
        private static void InsertValues<T>(ISheet sheet, IList<T> entities, string propertyName, int columnNumber, int rowNumber = 1)
        {
            if (rowNumber <= entities.Count)
            {
                var row = sheet.GetRow(rowNumber) ?? sheet.CreateRow(rowNumber);
                var entity = entities[rowNumber - 1];
                var property = entity?.GetType().GetProperty(propertyName);
                if (property == null)
                {
                    return;
                }
                var column = row.CreateCell(columnNumber);

                switch (property.PropertyType )
                {
                    case Type when property.PropertyType == typeof(int) :

                        column.SetCellType(CellType.Numeric);
                        column.SetCellValue((int)property.GetValue(entity));
                    break;

                    case Type when property.PropertyType == typeof(double):
                        column.SetCellType(CellType.Numeric);
                         column.SetCellValue((double)property.GetValue(entity));
                    break;

                    case Type when property.PropertyType == typeof(DateTime):
                        var value = (DateTime)property.GetValue(entity);

                        column.SetCellType(CellType.String);
                            column.SetCellValue(value.ToString());
                        break;

                    case Type when property.PropertyType == typeof(bool):
                        column.SetCellType(CellType.Boolean);
                             column.SetCellValue((bool)property.GetValue(entity));
                        break;

                    case Type when property.PropertyType == typeof(Guid):
                         var guid = (Guid)property.GetValue(entity);

                        column.SetCellType(CellType.String);
                        column.SetCellValue(guid.ToString("N"));
                    break;

                    default:
                        column.SetCellType(CellType.String);
                        column.SetCellValue((string)property.GetValue(entity));
                        break;
                }
                sheet.AutoSizeColumn(columnNumber);
               
                rowNumber++;

                InsertValues(sheet, entities, propertyName, columnNumber, rowNumber);
            }
        }

        /// <summary>
        /// Extract column values from row
        /// </summary>
        /// <param name="row">Current row to extract</param>
        /// <param name="rowNumber">Current row number</param>
        /// <param name="numberOfCells">Number of cells</param>
        /// <param name="rowList">List of extracted rows</param>
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
