using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Domain;
using QualityRepository;

namespace AppService
{
    public  class BllProductCard
    {
        private DllProductCard Dll { get; set; }
        public BllProductCard (SqlConnection con)
        {
            Dll = new DllProductCard(con);
        }
        public ProductCard.BraidStockCard GetBraidStockCard(string strWhere)
        {
            return Dll.GetBraidStockCard(strWhere);
        }
        public ProductCard.InvoiceCard InvoiceCard(string strWhere)
        {
            return Dll.InvoiceCard(strWhere);
        }
        public ProductCard.PacketBraidCard PacketBraidCard(string strWhere)
        {
            return Dll.PacketBraidCard(strWhere);
        }
        public bool InsertCard(ProductCard.CardInfo newCard, SqlTransaction tran)
        {
            return Dll.InsertCard(newCard, tran);
        }
        public List<ProductCard.CardInfo> GetCard(string strWhere)
        {
            return Dll.GetCard(strWhere);
        }
    }
}
