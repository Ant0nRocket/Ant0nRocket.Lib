using Ant0nRocket.Lib.Std20.Logging;

using ExcelDataReader;

using System;
using System.Data;
using System.IO;

namespace Ant0nRocket.Lib.Std20.IO
{
    /// <summary>
    /// Helper-class for reading Excel files.<br />
    /// Requires NuGet packages "ExcelReader" and "ExcelReader.DataSet".
    /// </summary>
    public static class ExcelReader
    {
        private static readonly Logger logger = Logger.Create(nameof(ExcelReader));

        /// <summary>
        /// Reads Excel file into DataSet.<br />
        /// If error occure - returnes empty DataSet with logging into
        /// standard logging system of this library.<br />
        /// If <paramref name="useHeaderRow"/> equals true then first row will handled as header.<br /><br />
        /// <code>
        /// // If you use .Net Core+ then add in startup file:
        /// Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);</code>
        /// </summary>
        public static DataSet ReadExcelFileAsDataSet(string fileName, bool useHeaderRow = true)
        {
            if (!File.Exists(fileName))
            {
                logger.LogError($"File '{fileName}' not found. Empty DataSet returned");
                return new();
            }

            var excelDataSetConfiguration = new ExcelDataSetConfiguration
            {
                ConfigureDataTable = (obj) =>
                new ExcelDataTableConfiguration() { UseHeaderRow = useHeaderRow }
            };

            try
            {

                using var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                using var reader = ExcelReaderFactory.CreateReader(stream);
                return reader.AsDataSet(excelDataSetConfiguration);
            }
            catch (Exception e)
            {
                logger.LogException(e);
                if (e.Message.Contains("1252"))
                    logger.LogInformation($"Add this in startup file: Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);");
                return new();
            }
        }
    }
}
