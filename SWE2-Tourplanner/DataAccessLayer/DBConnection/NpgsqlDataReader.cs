using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    public class NpgsqlDataReader : INpgsqlDataReader
    {
        private Npgsql.NpgsqlDataReader npgsqlDataReader;

        public NpgsqlDataReader(Npgsql.NpgsqlDataReader npgsqlDataReader)
        {
            this.npgsqlDataReader = npgsqlDataReader;
        }

        public int FieldCount() => npgsqlDataReader.FieldCount;
        public bool Read() => npgsqlDataReader.Read();
        public object GetValue(int i) => npgsqlDataReader.GetValue(i);

        public void Dispose()
        {
            npgsqlDataReader.DisposeAsync();
        }
    }
}
