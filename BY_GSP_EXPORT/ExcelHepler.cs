using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;


namespace Sanofi_GSP_EXPORT
{
    class ExcelHepler
    {
        public static List<DataTable> GetDataTablesFrom(string ExcelFile)
        {
            
            if (!File.Exists(ExcelFile))
                throw new FileNotFoundException("文件不存在");
            string fileType = System.IO.Path.GetExtension(ExcelFile);
            if (string.IsNullOrEmpty(fileType)) return null;
            List<DataTable> result = new List<DataTable>();
           
            Stream stream = new MemoryStream(File.ReadAllBytes(ExcelFile));
           
            IWorkbook workbook;
            if (fileType == ".xls")
                workbook = new HSSFWorkbook(stream);
            else
                workbook = new XSSFWorkbook(stream);
            
            
            for (int i = 0; i < workbook.NumberOfSheets; i++)
            {
                DataTable dt = new DataTable();
                ISheet sheet = workbook.GetSheetAt(i);
                int rowCount = sheet.LastRowNum;
                IRow headerRow = sheet.GetRow(sheet.FirstRowNum);
                //int HeaderRowCount = 0;
                //for (int rc = 0; rc < rowCount; rc++)
                //{
                //    headerRow = sheet.GetRow(rc);
                //    if (headerRow.GetCell(headerRow.FirstCellNum)!=null)
                //    {
                //        HeaderRowCount = rc;
                //        break;
                //    }
                    
                //}
                //headerRow = sheet.GetRow(HeaderRowCount);
                int cellCount = headerRow.LastCellNum;
                for (int j = headerRow.FirstCellNum; j < cellCount; j++)
                {
                    DataColumn column = new DataColumn(headerRow.GetCell(j).StringCellValue);
                    dt.Columns.Add(column);
                }


                for (int a = (sheet.FirstRowNum + 1); a <= rowCount; a++)
                {
                    IRow row = sheet.GetRow(a);
                    if (row == null) continue;

                    DataRow dr = dt.NewRow();
                    for (int b = row.FirstCellNum; b < cellCount; b++)
                    {
                        if (row.GetCell(b) == null) continue;
                        dr[b] = row.GetCell(b).ToString();
                    }

                    dt.Rows.Add(dr);
                }
                result.Add(dt);
            }
            stream.Close();

            return result;
        }

    }
        
}
