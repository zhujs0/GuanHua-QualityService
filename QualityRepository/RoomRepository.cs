using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Domain;
using System.Data.SqlClient;

namespace QualityRepository
{
    public class RoomRepository
    {
        private SqlConnection _sqlconnection { get; set; }
        public RoomRepository(SqlConnection sqlconnection)
        {
            _sqlconnection = sqlconnection;
        }

        public List<Room> GetRoom()
        {
            string strsql = "select * from dbo.ZL_Room";
            return _sqlconnection.Query<Room>(strsql).AsList();
        }

    }
}
