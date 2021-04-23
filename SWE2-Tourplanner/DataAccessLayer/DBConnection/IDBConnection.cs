using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    public interface IDBConnection
    {
        int ExecuteStatement(INpgsqlCommand npgsqlCommand);
        List<object[]> QueryDatabase(INpgsqlCommand npgsqlCommand);
        void OpenConnection();
        void CloseConnection();
    }
}
