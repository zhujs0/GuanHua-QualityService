using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npoi.Core;
using System.IO;
using Npoi.Core.HSSF.UserModel;
using System.Reflection;
using Npoi.Core.SS.UserModel;

namespace QualityWebApi.Common
{
    public class EntityToExcelByNpoi
    {
        public EntityToExcelByNpoi()
        {

        }
        public MemoryStream EntityToStream<T>(List<T> entity)
        {
            MemoryStream ms = new MemoryStream();
            if (entity!=null)
            {
                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet("Sheet1");
                //添加标题行
                IRow TitleRow = sheet.CreateRow(0);
                for (int i=0;i<entity.Count;i++)
                {
                    Type type = (entity[i] as object).GetType();
                    PropertyInfo[] propertyInfo = type.GetProperties();
                    FieldInfo[] field = type.GetFields();
                    IRow row = sheet.CreateRow(i+1);//在工作表中添加一行
                    int j = 0;
                    foreach (PropertyInfo pi in propertyInfo)
                    {
                        object val = pi.GetValue(entity[i]);
                        if (i == 0)
                        {
                            string CellTitle = pi.Name;
                            switch(CellTitle)
                            {
                                case "lastMonthCheck":
                                    CellTitle = "上月半成品";
                                    break;
                                case "feedingAmount":
                                    CellTitle = "本月投料数";
                                    break;
                                case "stockAmount":
                                    CellTitle = "正品入库数";
                                    break;
                                case "noHitAmount":
                                    CellTitle = "非命中数";
                                    break;
                                case "allStockAmount":
                                    CellTitle = "入库数（含非命中）";
                                    break;
                                case "monthCheck":
                                    CellTitle = "本月半成品";
                                    break;
                                case "allRate":
                                    CellTitle = "命中率%（含非命中）";
                                    break;
                                case "stockRate":
                                    CellTitle = "合格率%";
                                    break;
                                case "date":
                                    CellTitle = "日期";
                                    break;
                                default:
                                    break;
                            }
                            TitleRow.CreateCell(j).SetCellValue(CellTitle);
                        }
                        row.CreateCell(j).SetCellValue(val.ToString());
                        j++;
                    }
                }
                workbook.Write(ms);
                ms.Flush();
                ms.Position = 0;
            }
            return ms;
        }
    }
}
