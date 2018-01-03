using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Code
    {
        public long CodeID { get; set; }//编码流水号
        public string Suggestion { get; set; }//处理意见

        public string QualityClass { get; set; } //判类

        private string _qualityCodeString;
        public string CodeString {
            get {
                return _qualityCodeString ?? TopClassCode+ RoomCode + TypeCode + ProCode+ PreCode;
            }
            set {
                _qualityCodeString = value;
            }
        }//编码
        public string RoomName { get; set; }//车间名
        public string RoomCode { get; set; }//车间编码代号
        public string TypeName { get; set; }//质量分类
        public string TypeCode { get; set; }//分类编码代号
        public string Problem { get; set; }//质量问题
        public string ProCode { get; set; }//质量问题编码代号
        public string Present { get; set; }//比例
        public string PreCode { get; set; }//比例编码代号

        public string TopClass { get; set; }
        public string TopClassCode { get; set; }
        public string ProblemLevel { get; set; }

        public string EmployeeID { get; set; }
        public string Employee { get; set; }
        public string CreateTime
        {
            get;set;
            //get
            //{
            //    return DateTime.Now.ToString("YYYY-MM-DD HH: mm");
            //}
            //set
            //{
            //    if(value!=null&&value!="")
            //    {
            //        CreateTime = Convert.ToDateTime(value).ToString("YYYY-MM-DD HH:mm");
            //    }
               
            //}
        }
    }
}
