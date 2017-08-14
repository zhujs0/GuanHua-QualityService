using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Domain;
using Dapper;
using System.Linq;

namespace QualityRepository
{
    public class FeedbackBaseRepository
    {
        private SqlConnection _sqlconnnect { get; set; }
        public FeedbackBaseRepository(SqlConnection sqlconnect)
        {
            _sqlconnnect = sqlconnect;
        }

        public bool AddFeedbackBase(FeedbackBase baseInfo,SqlTransaction tran)
        {
            string strsql = @"insert into ZL_FeedbackBase(OrderNo,WorkProcedure,BatchNo,Model,Qty,EquipmentName,
EquipmentNo,FeedbackMan,FeedbackTime,Status) values(@OrderNo,@WorkProcedure,@BatchNo,@Model,@Qty,@EquipmentName,
@EquipmentNo,@FeedbackMan,@FeedbackTime,@Status)";
            int iResult = _sqlconnnect.Execute(strsql, baseInfo, tran);
            if(iResult>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<tp_carCraft> GetInfo(string chrBatchID)
        {
            string strsql = "select chrBatchID,chrType,intChipAmount from dbo.tp_carCraft where chrBatchID=@chrBatchID";
            return _sqlconnnect.Query<tp_carCraft>(strsql, new { chrBatchID = chrBatchID }).AsList();
        }

        public string GetOrderNo()
        {
            var parm = new DynamicParameters();
            parm.Add("@Message", dbType:System.Data.DbType.String,direction:System.Data.ParameterDirection.Output,size:14);
            parm.Add("@chrTableName", "FeedbackBase");
            parm.Add("@chrColumnName", "OrderNo");
            parm.Add("@intSeqLen",4);
            parm.Add("@chrHeadMark", "FK");
            parm.Add("@chrTailMark", "");
            parm.Add("@chrInDate", "");
            _sqlconnnect.Execute("pBas_GetNextTableID", parm, commandType: System.Data.CommandType.StoredProcedure);
            string Message = parm.Get<string>("@Message");
            return Message;
        }

        public Machine GetMachine(string chrMachineID)
        {
            string strsql = "select chrMachineID,chrMachine from tp_basMachineNo where chrMachineID=@chrMachineID";
            return  _sqlconnnect.Query<Machine>(strsql,new { chrMachineID = chrMachineID }).SingleOrDefault();
        }

        public List<FeedbackBase> GetOrderInfoByWhere(string strWhere)
        {
            string strsql = @"select * from ZL_FeedbackBase " + strWhere;
            return _sqlconnnect.Query<FeedbackBase>(strsql).AsList();
        }

        public List<FeedbackBase> GetOrderInfoByWhere(string strWhere,SqlTransaction tran)
        {
            string strsql = @"select * from ZL_FeedbackBase " + strWhere;
            return _sqlconnnect.Query<FeedbackBase>(strsql,tran).AsList();
        }


        public bool UpdateBase(FeedbackBase BaseInfo,SqlTransaction tran)
        {
            string strsql = @"update ZL_FeedbackBase set WorkProcedure=@WorkProcedure,
BatchNo=BatchNo,Model=@Model,Qty=@Qty,EquipmentName=@EquipmentName,EquipmentNo=@EquipmentNo,FeedbackMan=@FeedbackMan
where OrderNo=@OrderNo";
            if(_sqlconnnect.Execute(strsql,BaseInfo,tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Delete(string OrderNo,SqlTransaction tran)
        {
            string strsql = @"
delete from ZL_FeedbackBase where OrderNo=@OrderNo;
delete from ZL_FeedbackExHandle where OrderNo=@OrderNo;
delete from ZL_FeedbackExProblem where OrderNo=@OrderNo
delete from ZL_FeedbackExReason where OrderNo=@OrderNo";
            if(_sqlconnnect.Execute(strsql,new { OrderNo=OrderNo},tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool UpdatePrint(string OrderNo)
        {
            string strsql = "update ZL_FeedbackBase set HPrint=1 where OrderNo='" + OrderNo + "'";
            if(_sqlconnnect.Execute(strsql)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
    }
}
