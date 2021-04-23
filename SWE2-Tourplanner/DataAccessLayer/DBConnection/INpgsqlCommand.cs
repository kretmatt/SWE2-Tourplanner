using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    public interface INpgsqlCommand
    {
        int ExecuteNonQuery();
        INpgsqlDataReader ExecuteReader();
        Npgsql.NpgsqlParameterCollection Parameters { get; }
        Npgsql.NpgsqlConnection Connection { get; set; }
    }
}
