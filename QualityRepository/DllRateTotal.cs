using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Domain;
using System.Linq;


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
            //            string strSql = @"select t2.chrIssRptid,t1.chrboxupid,t2.chrOrderId as chrOrderNo,t1.chrDeptID,Isnull(t3.chrCustomerCName,t8.CustomerName) as chrCustomerID,chrCusType,chrSaleType
            //,Case When t1.chrType <> 'FIB' Then Isnull(t4.chrEmployee,t1.chrSuppmarket) Else Isnull(t5.chrEmployee,t1.chrOccuerId) End as chrSuppmarket,t2.chrTestId,chrItemId,chrQualityType,chrQualityTypeLast,chrSpec,isnull(chrLot,'') as chrLot
            //,isnull(chrType,'') as chrType,isnull(chrUnitId,'') as chrUnitid,isnull(decQuantity,0) as decQuantity,isnull(decPrice,0) as decPrice
            //,isnull(decPriceTax,0) as decPriceTax,isnull(decSum,0) as decSum,isnull(decTaxRate,0) as decTaxRate,isnull(decTax,0) as decTax,datIssRptDate,t2.chrMemo,Isnull(t1.chrTestID,'') as chrSampleOrderID

            // From V_tm_dProIOStore_Rfid t1
            // Inner Join V_tm_dProIOStoreDtl_RFID t2 On t2.chrIssRptID = t1.chrIssRptID
            // Left Join tm_sCustomer t3 On t3.chrCustomerID = t1.chrCustomerID
            // Left Join M_Customer t8 On t8.CustomerID = t1.chrCustomerID 
            // Left Join vper_Employee t4 On t4.chrEmployeeID = t1.chrSuppmarket
            // Left Join vper_Employee t5 On t5.chrEmployeeID = t1.chrOccuerId
            //-- join tp_carCraft on chrLot=tp_carCraft.chrBatchID
            // Where (Left(t1.chrPassed,1) ='F') 

            // And (dbo.GetPortTypeByBatchID(t2.chrLot) = 'G' Or dbo.GetPortTypeByBatchID(t2.chrLot) = 'N' 
            // Or dbo.GetPortTypeByBatchID(t2.chrLot) = 'Z') " + strWhere;
            return _con.Query<RateTotal.PutCount>(strSql).AsList().FirstOrDefault();
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
            return _con.Query<RateTotal.MonthCheckProd>(strSql).AsList().FirstOrDefault();
            //        string strSql = @" select  chrBatchID,chrType,decCheckNum,datDate from tp_proMonthCheckProd join tp_proMonthCheckProds 
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
    }
}
