using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;

using AutoAnalytics.Upload.Model.DataTypes.XlsxV1;

namespace AutoAnalytics.Upload.ExcelReader
{
    public static class ExcelReader
    {
        public static List<BreakdownInfo> Read(string path)
        {
            if (File.Exists(path) == false)
                throw new FileNotFoundException("Файл по пути " + path + " не найден. Открытие невозможно.");

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            ExcelPackage package = new ExcelPackage(fs);

            //Всегда берем первый лист
            var workSheet = package.Workbook.Worksheets[0];

            ////Объявляем List, в который потом запишем считанные записи. Надеюсь, их не несколько миллионов.
            List<BreakdownInfo> excelRecords = new List<BreakdownInfo>(workSheet.Dimension.End.Row - 1);

            //Проходимся по всем строкам
            for (int rowNumber = 1; rowNumber <= workSheet.Dimension.End.Row; rowNumber++)
            {
                BreakdownInfo newRecord = new BreakdownInfo();

                newRecord.GroupName = workSheet.Cells[rowNumber, 1].Value?.ToString();
                newRecord.SubgroupName = workSheet.Cells[rowNumber, 2].Value?.ToString();
                newRecord.Country = workSheet.Cells[rowNumber, 3].Value?.ToString();
                newRecord.Dealer = workSheet.Cells[rowNumber, 4].Value?.ToString();
                newRecord.ComplaintId = workSheet.Cells[rowNumber, 5].Value?.ToString();
                //try parse date
                newRecord.CarModel = workSheet.Cells[rowNumber, 7].Value?.ToString();
                newRecord.EngineNum = workSheet.Cells[rowNumber, 8].Value?.ToString();
                newRecord.DetailName = workSheet.Cells[rowNumber, 9].Value?.ToString();
                newRecord.Culprit = workSheet.Cells[rowNumber, 10].Value?.ToString();
                newRecord.Description = workSheet.Cells[rowNumber, 11].Value?.ToString();

                excelRecords.Add(newRecord);
            }

            return excelRecords;
        }
    }
}
