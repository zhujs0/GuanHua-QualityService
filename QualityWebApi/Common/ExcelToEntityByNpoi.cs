using Npoi.Core.HSSF.UserModel;
using Npoi.Core.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace QualityWebApi.Common
{
    public class ExcelToEntityByNpoi<TEntity> where TEntity : class
    {

        private HSSFWorkbook _hssfworkbook;
        private IList<string> _nameList;
        public void InitializeWorkbook(string path)
        {

            using (FileStream file = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                _hssfworkbook = new HSSFWorkbook(file);
            }
        }




        public IList<TEntity> ImportToEntity()
        {
            //一个单元格表示一个实体
            List<TEntity> valueList = new List<TEntity>();
            ISheet sheet = _hssfworkbook.GetSheetAt(0);//获取第一张Sheet表格
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();//获取表格行数

            while (rows.MoveNext())
            {
                IRow row = (HSSFRow)rows.Current;//当前行
                TEntity entity = System.Activator.CreateInstance<TEntity>();//创建实体
                for (int i = 0; i < _nameList.Count; i++)
                {
                    ICell cell = row.GetCell(i);//当前行第i个单元格
                    string cellName = _nameList[i];
                    string cellValue;
                    if (cell == null)
                    {
                        cellValue = "";
                    }
                    else
                    {
                        cellValue = cell.ToString();
                    }
                    this.SetValueToEntity(entity, cellName, cellValue);
                }
                valueList.Add(entity);

            }


            return valueList;
        }
        public void SetValueToEntity(TEntity entity, string name, string value)
        {
            //获取当前实例的 Type
            Type type = (entity as object).GetType();
            //获取当前 Type 的特定属性 
            PropertyInfo propertyInfo = type.GetProperty(name);
            if (propertyInfo == null)
            {
                throw new Exception(string.Format("{0}属性查找失败!", name));
            }
            //指示当前类型是否是泛型类型
            //if (!propertyInfo.PropertyType.IsGenericType)
            //{
            //    //用索引属性(Property) 的可选索引值设置该属性(Property) 的值，不存在索引时参数设置为null
            //    propertyInfo.SetValue(entity, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, propertyInfo.PropertyType), null);
            //}
            //else
            //{
            //    //返回一个表示可用于构造当前泛型类型的泛型类型定义的 Type 对象
            //    Type genericTypeDefinition = propertyInfo.PropertyType.GetGenericTypeDefinition();
            //    if (genericTypeDefinition == typeof(Nullable<>))
            //    {
            //        propertyInfo.SetValue(entity, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value,
            //            Nullable.GetUnderlyingType(propertyInfo.PropertyType)), null);
            //    }
            //}



        }

        public IList<TEntity> ImportToEntity(List<string> nameList)
        {
            _nameList = nameList;
            return ImportToEntity();
        }

    }
}
