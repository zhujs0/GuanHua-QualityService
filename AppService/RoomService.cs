using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using QualityRepository;
using Domain;

namespace AppService
{
    public class RoomService
    {
        private RoomRepository _repository { get; set; }
        public RoomService(SqlConnection sqlconnection)
        {
            _repository = new RoomRepository(sqlconnection);
        }

        public List<Room> GetRoom()
        {
            return _repository.GetRoom();
        }
    }
}
