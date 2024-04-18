

namespace Excelify.Services.Extensions
{
    /// <summary>
    /// Includes extension method to perform actions
    /// </summary>
    public static class ExcelifyExtension
    {
        /// <summary>
        /// Convert byte array to file 
        /// </summary>
        /// <param name="workSheet">Sheet to convert</param>
        /// <param name="path">Path to store file</param>
        public static void ToFile(this byte[] workSheet, string path)
        {
            workSheet.WriteToFile(path);
        }
    }
}
