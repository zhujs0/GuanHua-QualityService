using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pomelo.AspNetCore.TimedJob;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Data.SqlClient;
using AppService;
using System.Data;

namespace QualityWebApi.Filter
{
    public class TimeJob: Job
    {

        //[Invoke(Begin = "2017-12-18 15:36:00", Interval = 1000*60*3, SkipWhileExecuting = true)]
        [Invoke(Begin = "2017-11-27 23:59:59", Interval = 1000*60*60*24, SkipWhileExecuting = true)]
        public void Run()
        {
            Console.WriteLine("-----------定点保存当天库存开始-----------");
            string ConString = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("host.json", optional: true).Build().GetSection("ghSource").Value;
            SqlConnection con = new SqlConnection(ConString);
            if (con.State == ConnectionState.Closed)
            {
                con.Open();
            }
            SqlTransaction tran = con.BeginTransaction();
            try
            {
                BllRateTotal Bll = new BllRateTotal(con);
                string CreateTime = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd");//时间减一个小时，防止程序执行时间耽误正确时间：23:59:59
                Bll.InsertToCirculationStock(CreateTime, tran);
                Bll.InsertToFeeding(CreateTime, tran);
                Bll.InsertToOutInOfStorage(CreateTime, tran);
                Bll.InsertToStorage(CreateTime, tran);
                tran.Commit();
                Console.WriteLine("-----------定点保存当天库存结束-----------");
            }
            catch (Exception ex)
            {
                tran.Rollback();
                string fileName = "TimeJobError.txt";
                string textToAdd = DateTime.Now.ToString()+":"+ex.Message;
                FileStream fs = null;
                fs = new FileStream(fileName, FileMode.Append);
                using (StreamWriter sw = new StreamWriter(fs,System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(textToAdd);
                }
                fs.Dispose();
                Console.WriteLine("-----------定点保存当天库存异常-----------");
                throw ex;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            
                
                

        //Job要执行的逻辑代码

        //LogHelper.Info("Start crawling");
        //AddToLatestMovieList(100);
        //AddToHotMovieList();
        //LogHelper.Info("Finish crawling");
    }
    }
}
