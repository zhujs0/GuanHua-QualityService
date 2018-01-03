using Microsoft.VisualStudio.TestTools.UnitTesting;
using QualityWebApi.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace QualityTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            string path = Directory.GetCurrentDirectory() + "/ProblemPicture/";
            path = "E:\\GhSystem-冠华系统\\NewQuality-质量管理系统服务端\\QualityWebApi\\ProblemPicture\\";
            using (FileStream fs = new FileStream(path+"ex.xlsx", FileMode.Create, FileAccess.Write))
            {
                //EntityToExcelByNpoi Bll = new EntityToExcelByNpoi("");
                //Testclass testclass = new Testclass();
                //testclass.T1 = "1";
                //testclass.T2 = "2";
                //List<Testclass> list = new List<Testclass>();
                //list.Add(testclass);
                //MemoryStream ms= Bll.SetValueToEntity<Testclass>(list);
                //byte[] data = ms.ToArray();
                //fs.Write(data, 0, data.Length);
                //fs.Flush();
            }




            //Testclass Class1 = new Testclass();
            //Class1.T1 = "value1";
            //Class1.T2 = "value2";
            //List<Testclass> ListClass = new List<Testclass>();
            //ListClass.Add(Class1);
            //EntityToExcelByNpoi<List<Testclass>> Bll = new EntityToExcelByNpoi<List<Testclass>>();
            //Bll.SetValueToEntity<Testclass>(ListClass);
            //return;



            //List<Testclass> test = new List<Testclass> ();
            //Type t = test.GetType();
            //PropertyInfo[] pis = t.GetProperties();
            //foreach (PropertyInfo pi in pis)
            //{
            //    string name = pi.Name;
            //    Console.WriteLine(pi.Name);
            //}
            //List<Testclass> test1 = new List<Testclass>();
            //Testclass test2 = new Testclass();
            //Object qq = test2;
            //test2.T1 = "1";
            //Type t2 = test2.GetType();
            //FieldInfo[] fis1 = t2.GetFields();
            //foreach (FieldInfo fi in fis1)
            //{
            //    string name = fi.Name;
            //    Console.WriteLine(fi.Name);
            //}
            //test1.Add(test2);
            //FieldInfo fieldInfo= t2.GetField("T1");
            //MemberInfo[] member = t2.GetMembers();
            //PropertyInfo[] propertyInfo = t2.GetProperties();
            //object vv = propertyInfo.GetValue(0);
            //object va = propertyInfo[0].GetValue(qq);
            //Type t1 = test1.GetType();
            //FieldInfo[] fis = t1.GetFields();
            //foreach (FieldInfo fi in fis)
            //{
            //    string name = fi.Name;
            //    Console.WriteLine(fi.Name);
            //}



            //EntityToExcelByNpoi<Testclass> ToExcel = new EntityToExcelByNpoi<Testclass>();
            // Testclass test = new Testclass();
            //    test.T1 = "1";
            //    test.T2 = "2";
            //    List<string> NameList = new List<string>();
            //    NameList.Add("T1");
            //    NameList.Add("T2");
            //    ToExcel.SetValueToEntity(test, "T1", "1");
        }
        public class Testclass
        {
            public string T1 { get; set; }
            public string T2 { get; set; }
        }
    }
}
