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
EquipmentNo,FeedbackMan,FeedbackTime,Status,ProblemLevel,ProductClass) values(@OrderNo,@WorkProcedure,@BatchNo,@Model,@Qty,@EquipmentName,
@EquipmentNo,@FeedbackMan,@FeedbackTime,@Status,@ProblemLevel,@ProductClass)";
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

        public bool Insert(FeedbackBase baseInfo, SqlTransaction tran)
        {
            string strsql = @"
insert into ZL_FeedbackBase(ID,OrderNo,WorkProcedure,BatchNo,Model,Qty,EquipmentName,
EquipmentNo,FeedbackMan,FeedbackTime,Status,ProblemLevel,ProductClass,OrderType,Measure,Report,TechnologistMembers,EmployeeID) 
values(@ID,@OrderNo,@WorkProcedure,@BatchNo,@Model,@Qty,@EquipmentName,
@EquipmentNo,@FeedbackMan,@FeedbackTime,@Status,@ProblemLevel,@ProductClass,@OrderType,@Measure,@Report,@TechnologistMembers,@EmployeeID)";
            int iResult = _sqlconnnect.Execute(strsql, baseInfo, tran);
            
            if (iResult > 0)
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
            parm.Add("@chrTableName", "ZL_FeedbackBase");
            parm.Add("@chrColumnName", "OrderNo");
            parm.Add("@intSeqLen",4);
            parm.Add("@chrHeadMark", "");
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
delete from ZL_FeedbackExProblem where OrderNo=@OrderNo;
delete from ZL_FeedbackExReason where OrderNo=@OrderNo;
delete from ZL_ApprovalStream where OrderNo=@OrderNo;
delete from ZL_Card where FKOrderNo=@OrderNo;
delete from ZL_OrderSuggestion where OrderNo=@OrderNo;
delete from GHOA.dbo.WorkFlowTask where Title like '%" + OrderNo.Trim() + "%'";
            if(_sqlconnnect.Execute(strsql,new { OrderNo=OrderNo},tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool DeleteWorkFlowTask(string ID,SqlTransaction tran)
        {
            string strSql = @"delete from GHOA.dbo.WorkFlowTask where InstanceID=@ID";
            if(_sqlconnnect.Execute(strSql,new { ID = ID },tran)>0)
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

        public List<FeedbackBase> GetQualityOrder(long PageIndex, long PageSize,string strWhere,out long RowCount)
        {
            RowCount = 0;
            string strSql = @"select * from (select * , ROW_NUMBER() OVER (ORDER BY FeedbackTime desc) AS RowNumber from 
ZL_FeedbackBase " + strWhere + ") as t where RowNumber > @PageSize*(@PageIndex-1) and RowNumber<=@PageSize*@PageIndex";
            string RowSql = @"select * from ZL_FeedbackBase " + strWhere;
            RowCount = _sqlconnnect.Query<FeedbackBase>(RowSql).AsList().Count;
            return _sqlconnnect.Query<FeedbackBase>(strSql, new { PageIndex= PageIndex, PageSize= PageSize }).AsList();
        }
        //join GHOA.dbo.WorkFlowTask  on GHOA.dbo.WorkFlowTask.InstanceID=ID
        public List<FeedbackBase> GetOrderByWorkFlowTaskOnPage(long PageIndex, long PageSize, string strWhere, out long RowCount)
        {
            RowCount = 0;
            //            string strSql = @"select * from (select a.* , b.Status as ProvalStatus, ROW_NUMBER() OVER (ORDER BY FeedbackTime desc) AS RowNumber from 
            //ZL_FeedbackBase a join GHOA.dbo.WorkFlowTask b on b.InstanceID=a.ID where 
            // b.Sort=(select MAX(Sort) from GHOA.dbo.WorkFlowTask where FlowID =FlowID AND GroupID = GroupID) 
            //" + strWhere + ") as t where RowNumber > @PageSize*(@PageIndex-1) and RowNumber<=@PageSize*@PageIndex";
            string strSql = @"select * from ( SELECT a.*,b.PrevStatus,b.Sort,b.Status as ProvalStatus,b.StepName,b.StepID,ROW_NUMBER() OVER (ORDER BY FeedbackTime desc) AS RowNumber FROM dbo.ZL_FeedbackBase a 
OUTER APPLY GHOA.dbo.Fun_GetFlowStatusName(ID) b where " + strWhere + " ) as t  where RowNumber > @PageSize*(@PageIndex-1) and RowNumber<=@PageSize*@PageIndex ";
            string RowSql = @" SELECT OrderNo FROM dbo.ZL_FeedbackBase a 
OUTER APPLY GHOA.dbo.Fun_GetFlowStatusName(ID) b where " + strWhere;
            //            string RowSql = @"select OrderNo from ZL_FeedbackBase a join GHOA.dbo.WorkFlowTask b 
            //on b.InstanceID=a.ID where b.Sort=(select MAX(Sort) from GHOA.dbo.WorkFlowTask where FlowID =FlowID AND GroupID = GroupID)" + strWhere;
            RowCount = _sqlconnnect.Query<FeedbackBase>(RowSql).AsList().Count;
            return _sqlconnnect.Query<FeedbackBase>(strSql, new { PageIndex = PageIndex, PageSize = PageSize }).AsList();
        }

        public List<FeedbackBase> GetWaitConfirm(string strWhere)
        {
            string strSql = @"SELECT a.*,b.*, b.Status as ProvalStatus  FROM dbo.ZL_FeedbackBase a 
OUTER APPLY GHOA.dbo.Fun_GetFlowStatusName(ID) b "+ strWhere;
            return _sqlconnnect.Query<FeedbackBase>(strSql).AsList();

        }




        public Guid GetNewID()
        {
            string strSql = @"declare @ID uniqueidentifier;set @ID=NewID(); select @ID as ID";
            NewID result = _sqlconnnect.Query<NewID>(strSql).AsList().FirstOrDefault();
            Guid ID = result.ID;
            return ID;
        }

        public bool InsertOrderSuggestion(SqlTransaction tran,string Suggestion,string OrderNo,string CodeString)
        {
            string strSql = @"insert into ZL_OrderSuggestion(Suggestion,OrderNo,CodeString) values (@Suggestion,@OrderNo,@CodeString)";
            int iResult= _sqlconnnect.Execute(strSql, new { Suggestion = Suggestion, OrderNo = OrderNo, CodeString = CodeString }, tran);
            if(iResult>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public RateTotal.tm_dTempStoreIO GetTempStoreAmount(string BatchNo,string WorkProcedure)
        {
            string strSql = @" select  isnull(SUM(decSumQty),0) as decSumQty  from tm_dTempStoreIO  join tm_dTempStoreIODtl on tm_dTempStoreIO.ChrIssRptID =tm_dTempStoreIODtl.ChrIssRptID
 where  ChrCurProcessId like '%"+ WorkProcedure + "%' and  RTRIM(chrLot) ='"+ BatchNo + "'";
            return _sqlconnnect.Query<RateTotal.tm_dTempStoreIO>(strSql).AsList().FirstOrDefault();
        }

        public bool UpdateStatus(string ID,string StepID,int Status)
        {
            string strSql = "update GHOA.dbo.WorkFlowTask set Status=" + Status + " where UPPER(InstanceID)='" + ID.ToUpper()
                + "' and StepID='" + StepID + "'";
            if(_sqlconnnect.Execute(strSql)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /*
           public int Sort { get; set; }
            public string InstanceID { get; set; }
            public string StepID { get; set; }
            public string StepName { get; set; }
            public int Status { get; set; }*/
        //public CommomDomain.WorkFlowTask GetInfo(string InstanceID,int Sort)
        //{
        //    string strSql = @"select * from GHOA.dbo.WorkFlowTask where InstanceID='" + InstanceID
        //        + "' and Sort=" + Sort + " and (Status!=0 and Status!=1 and Status!=4)";
        //    return _sqlconnnect.Query<CommomDomain.WorkFlowTask>(strSql)
        //}

        
    }
}
