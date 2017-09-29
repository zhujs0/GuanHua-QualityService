using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Dapper;
using Domain;
using System.Linq;

namespace QualityRepository
{
    public class DllProductCard
    {
        private SqlConnection _con { get; set; }
        public DllProductCard (SqlConnection con)
        {
            _con = con;
        }

        public ProductCard.BraidStockCard GetBraidStockCard(string strWhere)
        {
            string strSql = @"select * from tb_GetBraidStockupdel " + strWhere;
            return _con.Query<ProductCard.BraidStockCard>(strSql).AsList().FirstOrDefault();
        }
        public ProductCard.InvoiceCard InvoiceCard(string strWhere)
        {
            string strSql = @"select * from sq_invoiceCard " + strWhere;
            return _con.Query<ProductCard.InvoiceCard>(strSql).AsList().FirstOrDefault();
        }
        public ProductCard.PacketBraidCard PacketBraidCard(string strWhere)
        {
            string strSql = @"select * from tb_PacketBraidStockupDetail " + strWhere;
            return _con.Query<ProductCard.PacketBraidCard>(strSql).AsList().FirstOrDefault();
        }
        public bool InsertCard(ProductCard.CardInfo newCard,SqlTransaction tran)
        {
            string strSql = @"insert into ZL_Card(FKOrderNo,CardNo,OrderNo,Customer,TempClass,TempModel,ProductModel,ProductClass,
Amount,TempAmount,BatchNo) values(@FKOrderNo,@cardNo,@orderNo,@customer,@tempClass,@tempModel,@productModel,@productClass,
@amount,@tempAmount,@batchNo)";
            if(_con.Execute(strSql,newCard,tran)>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ProductCard.CardInfo> GetCard(string strWhere)
        {
            string strSql = @"select FKOrderNo,CardNo as cardNo,OrderNo as orderNo,Customer as customer,
TempClass as tempClass,TempModel as tempModel,ProductModel as productModel,ProductClass as productClass,
Amount as amount,TempAmount as tempAmount,BatchNo as batchNo from ZL_Card  " + strWhere;
            return _con.Query<ProductCard.CardInfo>(strSql).AsList();
        }
    }
}
