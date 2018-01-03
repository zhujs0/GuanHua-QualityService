using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Domain;
using System.Linq;
using System.Data;



namespace QualityRepository
{
    public class DllRateTotal
    {
        private SqlConnection _con { get; set; }
        public DllRateTotal(SqlConnection con)
        {
            _con = con;
        }

        public List<RateTotal.tm_item> GetTm_item()
        {
            string strSql = @"select chritemid,chrspec from tm_item where left(chritemid,3)='312' order by chritemid";
            return _con.Query<RateTotal.tm_item>(strSql).AsList();
        }
        /// <summary>
        /// 获取入库数
        /// </summary>
        /// <returns></returns>
        public RateTotal.PutCount GetPut(string strWhere)
        {
            string strSql = @"select sum(isnull(decQuantity,0)) as decQuantity
 From V_tm_dProIOStore_Rfid t1
 Inner Join V_tm_dProIOStoreDtl_RFID t2 On t2.chrIssRptID = t1.chrIssRptID
 Left Join tm_sCustomer t3 On t3.chrCustomerID = t1.chrCustomerID
 Left Join M_Customer t8 On t8.CustomerID = t1.chrCustomerID 
 Left Join vper_Employee t4 On t4.chrEmployeeID = t1.chrSuppmarket
 Left Join vper_Employee t5 On t5.chrEmployeeID = t1.chrOccuerId
-- join tp_carCraft on chrLot=tp_carCraft.chrBatchID
 Where (Left(t1.chrPassed,1) ='F') 
 And (dbo.GetPortTypeByBatchID(t2.chrLot) = 'G' Or dbo.GetPortTypeByBatchID(t2.chrLot) = 'N' 
 Or dbo.GetPortTypeByBatchID(t2.chrLot) = 'Z') " + strWhere;
            RateTotal.PutCount Obj = new RateTotal.PutCount();
            Obj.decQuantity = 0;
            SqlCommand command = new SqlCommand(strSql, _con);
            command.CommandTimeout = 120;
            if (_con.State != ConnectionState.Open)
            {
                _con.Open();
            }
            else
            {
                _con.Close();
                _con.Open();
            }
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                if(reader["decQuantity"] != null&&reader["decQuantity"].ToString()!="")
                {
                    Obj.decQuantity = Convert.ToDecimal(reader["decQuantity"].ToString());
                }
                //Obj.decQuantity = reader["decQuantity"] != null ? Convert.ToDecimal(reader["decQuantity"].ToString()) : 0;
            }
            reader.Dispose();
            command.Dispose();
            return Obj;
            //return _con.Query<RateTotal.PutCount>(strSql).AsList().FirstOrDefault();
        }

        /// <summary>
        /// 投料数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public RateTotal.FeedCount GetFeedCount(string strWhere)
        {
            string strSql = @"select sum(decquantity) as decquantity from tm_dTempStoreIO t1 
               left join tm_dTempStoreIODtl t2 on t1.chrissrptid = t2.chrissrptid  
               left join  tp_carCraft t3 on t2.chrlot=t3.chrBatchID 
               where ( RTRIM(LTRIM(ChrPreProcessId)) like '丝印'  or (( RTRIM(LTRIM(ChrPreProcessId)) like '%新工艺%' ) and ( RTRIM(LTRIM(ChrCurProcessId)) like '层压' ) ))
            and chrinouttype = 'sia' and (t1.chrstate = 'F' or t1.chrstate = 'P') " + strWhere;
         //   string strSql = @"select  ChrItemID,decquantity ,datInOutDate,chrLot from tm_dTempStoreIO t1 
	  	     //   left join tm_dTempStoreIODtl t2 on t1.chrissrptid = t2.chrissrptid  
	  	     //   left join  tp_carCraft t3 on t2.chrlot=t3.chrBatchID 
	  	     //   where ( chrcurprocessid = '层压' )  
	  		    //and chrinouttype = 'sia' and t1.chrstate = 'F' " + strWhere;
            return _con.Query<RateTotal.FeedCount>(strSql).AsList().FirstOrDefault();
        }

        /// <summary>
        /// 获取半成品数
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        public RateTotal.MonthCheckProd GetMonthCheckProd(string strWhere,string strWhere2)
        {
            string strSql = @" 
declare @LiuTongAmount decimal(18, 0)
declare @ZhengpinAmount decimal(18, 0)
declare @MonthCheckAmount decimal(18,0)
select @MonthCheckAmount=sum(isnull(decCheckNum,0)) from tp_proMonthCheckProd join tp_proMonthCheckProds 
	   on tp_proMonthCheckProds.intNo=tp_proMonthCheckProd.intNo  where  intProductKind='0'
  and intState!=3 and intState!=8 and intState!=4 and intState!=9 and chrProcedure<>'配料流延' " + strWhere 
            +@"
        SELECT  @LiuTongAmount=sum(isnull(decQtySeparPass,0)) FROM ZL_PreparedProduct t1 
	    --LEFT JOIN tm_dProcedure t14 ON t1.chrProcessID=t14.chrProcedure  
	    --left join tp_carcraft t2 on t1.chrLot = t2.chrBatchID   
	    WHERE chrParentProcId  like '%中转仓%' And (chrProcessID like '%Ni电极烧端%'   
	    or chrProcessID like'%起筛巴%') " + strWhere2
         + @"SELECT   @ZhengpinAmount=sum(isnull(DecQtyNal,0))
	    FROM ZL_PreparedProduct t1 
	    --LEFT JOIN tm_dProcedure t14 ON t1.chrProcessID=t14.chrProcedure  
	    --left join tp_carcraft t2 on t1.chrLot = t2.chrBatchID   
	    WHERE chrParentProcId  like '%中转仓%' 
	    And ( chrProcessID like'%测试%'  or chrProcessID like'%编带%'or chrProcessID like'%沉积%') " + strWhere2
         + " select @LiuTongAmount as LiuTongAmount,@ZhengpinAmount as ZhengpinAmount,@MonthCheckAmount as  MonthCheckAmount";

            //SqlCommand command = new SqlCommand(strSql, _con);
            //command.CommandTimeout = 120;
            //if(_con.State!=ConnectionState.Open)
            //{
            //    _con.Open();
            //}else
            //{
            //    _con.Close();
            //    _con.Open();
            //}
            //SqlDataReader reader = command.ExecuteReader();
            //reader.Dispose();
            //command.Dispose();
            //return null;
            
            return _con.Query<RateTotal.MonthCheckProd>(strSql).AsList().FirstOrDefault();
            //string strSql = @" select  chrBatchID,chrType,decCheckNum,datDate from tp_proMonthCheckProd join tp_proMonthCheckProds 
            //on tp_proMonthCheckProds.intNo=tp_proMonthCheckProd.intNo " + strWhere + " order by datDate desc";
            //return _con.Query<RateTotal.MonthCheckProd>(strSql).AsList();
        }

        public List<RateTotal.dTempStoreBalance> GetdTempStoreBalance(string strWhere)
        {
            string strSql = @"select sum(DecQtyNal) as DecQtyNal  from Tm_dTempStoreBalance 
 where (chrProcessID not in ('待丝印','丝印','薄膜膜片仓','新工艺一楼印刷','新工艺二楼叠层','新工艺一楼叠层',
'新工艺二楼印刷','新工艺','新工艺印刷','新工艺叠层','新工艺待印刷')) " + strWhere;
            //string strSql = @"select *  from Tm_dTempStoreBalance " + strWhere;
            return _con.Query<RateTotal.dTempStoreBalance>(strSql).AsList();
        }

        public RateTotal.ZhuanZhengpin GetZhuanZhengpin(string strWhere)
        {
            string strSql = @"SELECT sum(t2.decChangeInQnty) as decChangeInQnty FROM vm_dPackOut t1 LEFT JOIN vm_dPackIn t2 ON t1.chrissrptid =t2.chrOutNo And t1.chrItemid=t2.chrItemid 
 And t1.chrLot = t2.chrlot And t1.chrQualityType = t2.chrQualityType left join tm_sSaleOrder t3
  on t2.chrordernos=t3.chrOrderid left join tm_sCustomer t4 on t3.chrcustomerid=t4.chrcustomerid
    Where 1 = 1 
	 And (t1.chrState='P' or t1.chrState='F')
 And (Isnull(t2.chrChangeItemid,'') Like '11%' And Isnull(t2.decChangeInQnty,0) <> 0) " + strWhere;
            return _con.Query<RateTotal.ZhuanZhengpin>(strSql).AsList().FirstOrDefault();
        }


        public class DecAmount
        {
            public decimal DecQty { get; set; }
        }

        public decimal Get_WarehouseStorage(string strWhere)
        {
            string strSql = @"select sum(DecQty) as DecQty from ZL_WarehouseStorage " + strWhere;
            DecAmount value = _con.Query<DecAmount>(strSql).AsList().FirstOrDefault();
            return Convert.ToDecimal(value.DecQty);
        }
        public decimal Get_Feeding(string strWhere)
        {
            string strSql = @"select sum(DecQty) as DecQty from ZL_Feeding " + strWhere;
            DecAmount value = _con.Query<DecAmount>(strSql).AsList().FirstOrDefault();
            return Convert.ToDecimal(value.DecQty);
        }

        public decimal Get_CirculationStock(string strWhere)
        {
            string strSql = @"select (sum(DecQtyNal)+sum(DecQtySeparPass)) as DecQty from ZL_CirculationStock " + strWhere;
            DecAmount value = _con.Query<DecAmount>(strSql).AsList().FirstOrDefault();
            return Convert.ToDecimal(value.DecQty);
        }

        public decimal Get_OutInOfStorage(string strWhere)
        {
            string strSql = @"select sum(DecQty) as DecQty from ZL_OutInOfStorage " + strWhere;
            DecAmount value = _con.Query<DecAmount>(strSql).AsList().FirstOrDefault();
            return Convert.ToDecimal(value.DecQty);
        }

        public bool InsertToStorage(string CreateTime,SqlTransaction tran)
        {
            string strSql = @"insert into ZL_WarehouseStorage(ChrItemID,ChrSize,ChrPorcelain,ChrClass,DecQty,ChrType,Date) 
 (select   SUBSTRING(rtrim(ltrim(chrItemId)),3,16),SUBSTRING(chrSpec,1,4),chrPowder,dbo.Fun_GetByItemID(chrSpec), decQuantity,
t1.chrType,datIssRptDate from V_tm_dProIOStore_Rfid t1 Inner Join V_tm_dProIOStoreDtl_RFID t2 On t2.chrIssRptID = t1.chrIssRptID
 Left Join tm_sCustomer t3 On t3.chrCustomerID = t1.chrCustomerID
 Left Join M_Customer t8 On t8.CustomerID = t1.chrCustomerID 
 Left Join vper_Employee t4 On t4.chrEmployeeID = t1.chrSuppmarket
 Left Join vper_Employee t5 On t5.chrEmployeeID = t1.chrOccuerId
 join tp_carCraft t10 on chrLot=t10.chrBatchID where (Left(t1.chrPassed,1) ='F') 
 And (dbo.GetPortTypeByBatchID(t2.chrLot) = 'G' Or dbo.GetPortTypeByBatchID(t2.chrLot) = 'N' 
 Or dbo.GetPortTypeByBatchID(t2.chrLot) = 'Z') and
 (t1.chrType='FIE' or t1.chrType='FIA') and  datIssRptDate Between '" + CreateTime + "' and '" + CreateTime + " 23:59:59' )";
            if (_con.Execute(strSql,new { }, tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InsertToOutInOfStorage(string CreateTime, SqlTransaction tran)
        {
            string strSql = @"INSERT INTO ZL_OutInOfStorage(ChrItemID,ChrSize,DecQty,ChrClass,ChrChangeItemID,ChrPorcelain,Date,ChrType,
ChrChangeSize,ChrChangeClass,DecInQnty)
 (SELECT SUBSTRING(LTRIM(rtrim(t2.chrItemID)),3,16), 
 SUBSTRING(LTRIM(rtrim(t2.chrItemID)),3,4) as ChrSize,
 t2.decChangeInQnty as DecQty ,
 dbo.Fun_GetByItemID(SUBSTRING(LTRIM(rtrim(t2.chrItemID)),3,15))  as ChrClass,
 substring(LTRIM(rtrim(t2.chrChangeItemID)),3,16) as ChrChangeItemID,
 chrPowder,t2.datInOutStock,
 (case 
 when t1.chritemid like '13%' then '13' 
 when t1.chritemid like '12%' then '12'
 end) as ChrType,
 SUBSTRING(LTRIM(rtrim(t2.chrChangeItemID)),3,4) as ChrChangeItemSize,
 dbo.Fun_GetByItemID(substring(LTRIM(rtrim(t2.chrChangeItemID)),3,15)),
 t2.decinqnty
  FROM vm_dPackOut t1 LEFT JOIN vm_dPackIn t2 ON t1.chrissrptid =t2.chrOutNo And t1.chrItemid=t2.chrItemid 
 And t1.chrLot = t2.chrlot And t1.chrQualityType = t2.chrQualityType left join tm_sSaleOrder t3
on t2.chrordernos=t3.chrOrderid left join tm_sCustomer t4 on t3.chrcustomerid=t4.chrcustomerid
join tp_carCraft t10 on t10.chrBatchID=t2.chrChangeLot
Where (t1.chrState='P' or t1.chrState='F') 
And (Isnull(t2.chrChangeItemid,'') Like '11%' And Isnull(t2.decChangeInQnty,0) <> 0) 
and t2.datInOutStock between '"+CreateTime+" 00:00:00' and '"+CreateTime+" 23:59:59' and (t1.chritemid like '13%' or t1.chritemid like '12%'))";
            if (_con.Execute(strSql, new { }, tran) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InsertToFeeding(string CreateTime, SqlTransaction tran)
        {
            string strSql = @"insert into ZL_Feeding(ChrItemID,ChrSize,ChrClass,ChrPorcelain,DecQty,Date)
(select ChrItemID, left(LTRIM(rtrim(ChrItemID)),4) as ChrSize,dbo.Fun_GetByItemID(left(rtrim(ltrim(ChrItemID)),15)),chrPowder,
decquantity,datInOutDate from tm_dTempStoreIO t1 
left join tm_dTempStoreIODtl t2 on t1.chrissrptid = t2.chrissrptid  
join  tp_carCraft t3 on t2.chrlot=t3.chrBatchID 
where ( RTRIM(LTRIM(ChrPreProcessId)) like '丝印'  
or (( RTRIM(LTRIM(ChrPreProcessId)) like '%新工艺%' )
 and ( RTRIM(LTRIM(ChrCurProcessId)) like '层压' ) ))
and chrinouttype = 'sia' and (t1.chrstate = 'F' or t1.chrstate = 'P')
and datInOutDate between '"+CreateTime+"' and '"+CreateTime+" 23:59:59')";
            if (_con.Execute(strSql, new { }, tran) > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InsertToCirculationStock(string CreateTime, SqlTransaction tran)
        {
            string strSql = @"insert into ZL_CirculationStock(ChrItemID,ChrSize,ChrClass,ChrPorcelain,DecQtyNal,DecQtySeparPass,ChrParentProcId,ChrProcessID,Date)
select ChrItemID,LEFT(rtrim(ltrim(ChrItemID)),4),
dbo.Fun_GetByItemID(left(rtrim(ltrim(ChrItemID)),15)),
chrPowder,DecQtyNal,decQtySeparPass,chrParentProcId,chrProcessID,'"+CreateTime
+@"' from Tm_dTempStoreBalance t1  join tp_carCraft t2 on t1.chrLot=t2.chrBatchID
 where (chrProcessID not in ('待丝印','丝印','薄膜膜片仓','新工艺一楼印刷','新工艺二楼叠层','新工艺一楼叠层',
'新工艺二楼印刷','新工艺','新工艺印刷','新工艺叠层','新工艺待印刷'))";
            if (_con.Execute(strSql, new { }, tran) > 0)
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
