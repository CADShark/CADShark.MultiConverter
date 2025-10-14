using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace CADShark.Common.MultiConverter.Extractors
{
    public class ExcelExporter
    {
        public static void SaveToExcel(List<CutListItem> cutLists, string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException ex)
                {
                    throw new IOException($"Не удалось удалить существующий файл {filePath}. Убедитесь, что он не открыт в другом приложении.", ex);
                }
            }


            var lic = ExcelPackage.License;
            lic.SetNonCommercialPersonal("Dede");

            using var package = new ExcelPackage(new FileInfo(filePath));
            var worksheet = package.Workbook.Worksheets.Add("CutList");


            worksheet.Cells[1, 1].Value = "File Path";
            worksheet.Cells[1, 2].Value = "Configuration Name";
            worksheet.Cells[1, 3].Value = "Workpiece X";
            worksheet.Cells[1, 4].Value = "Workpiece Y";
            worksheet.Cells[1, 5].Value = "Bend";
            worksheet.Cells[1, 6].Value = "Thickness";
            worksheet.Cells[1, 7].Value = "Surface Area";


            for (int i = 0; i < cutLists.Count; i++)
            {
                worksheet.Cells[i + 2, 1].Value = cutLists[i].FilePath;
                worksheet.Cells[i + 2, 2].Value = cutLists[i].ConfigurationName;
                worksheet.Cells[i + 2, 3].Value = (double)cutLists[i].WorkpieceX;
                worksheet.Cells[i + 2, 4].Value = (double)cutLists[i].WorkpieceY;
                worksheet.Cells[i + 2, 5].Value = cutLists[i].Bend;
                worksheet.Cells[i + 2, 6].Value = (double)cutLists[i].Thickness;
                worksheet.Cells[i + 2, 7].Value = (double)cutLists[i].SurfaceArea;
            }

            worksheet.Cells[2, 3, cutLists.Count + 1, 4].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[2, 6, cutLists.Count + 1, 6].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[2, 7, cutLists.Count + 1, 7].Style.Numberformat.Format = "#,##0.00";
                
            worksheet.Cells.AutoFitColumns();

            package.Save();
        }
    }
}