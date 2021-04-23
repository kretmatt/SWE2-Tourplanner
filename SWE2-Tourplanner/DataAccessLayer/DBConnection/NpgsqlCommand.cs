using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DataAccessLayer.DBConnection
{
    public class NpgsqlCommand : INpgsqlCommand
    {
        private Npgsql.NpgsqlCommand npgsqlCommand;
        public int ExecuteNonQuery() => Connection != null ? npgsqlCommand.ExecuteNonQuery() : 0;
        public INpgsqlDataReader ExecuteReader() => Connection != null ? new NpgsqlDataReader(npgsqlCommand.ExecuteReader()) : null;
        public NpgsqlParameterCollection Parameters => npgsqlCommand.Parameters;
        public NpgsqlConnection Connection
        {
            get
            {
                return npgsqlCommand.Connection;
            }
            set
            {
                npgsqlCommand.Connection = value;
            }
        }

        public NpgsqlCommand(string cmdText)
        {
            npgsqlCommand = new Npgsql.NpgsqlCommand(cmdText);
        }
    }
}
