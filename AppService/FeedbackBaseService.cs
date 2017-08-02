﻿using System;
using System.Collections.Generic;
using System.Text;
using Domain;
using System.Data.SqlClient;
using QualityRepository;

namespace AppService
{
    public class FeedbackBaseService
    {
        private FeedbackBaseRepository _repository { get; set; }

        public FeedbackBaseService(SqlConnection sqlconnection)
        {
            _repository = new FeedbackBaseRepository(sqlconnection);
        }
        public bool AddFeedbackBase(FeedbackBase baseInfo, SqlTransaction tran)
        {
            return _repository.AddFeedbackBase(baseInfo, tran);
        }

        public List<tp_carCraft> GetInfo(string chrBatchID)
        {
            return _repository.GetInfo(chrBatchID);
        }

        public string GetOrderNo()
        {
            return _repository.GetOrderNo();
        }

        public Machine GetMachine(string chrMachineID)
        {
            return _repository.GetMachine(chrMachineID);
        }

        public List<FeedbackBase> GetOrderInfoByWhere(string strWhere)
        {
            return _repository.GetOrderInfoByWhere(strWhere);
        }

        public List<FeedbackBase> GetOrderInfoByWhere(string strWhere, SqlTransaction tran)
        {
            return _repository.GetOrderInfoByWhere(strWhere,tran);
        }

        public bool UpdateBase(FeedbackBase BaseInfo, SqlTransaction tran)
        {
            return _repository.UpdateBase(BaseInfo,tran);
        }

        public bool Delete(string OrderNo, SqlTransaction tran)
        {
            return _repository.Delete(OrderNo, tran);
        }
    }
}
