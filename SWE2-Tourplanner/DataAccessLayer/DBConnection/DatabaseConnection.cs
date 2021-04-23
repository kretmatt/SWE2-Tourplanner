using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    public class DatabaseConnection : IDBConnection
    {
        private static DatabaseConnection dB;
        private Npgsql.NpgsqlConnection npgsqlConnection;

        private DatabaseConnection()
        {
            IConfiguration config = new ConfigurationBuilder().AddJsonFile("config.json",false,true).Build();
            npgsqlConnection = new Npgsql.NpgsqlConnection($"Host={config["dbsettings:host"]};Port={config["dbsettings:port"]};Username={config["dbsettings:username"]};Password={config["dbsettings:password"]};Database={config["dbsettings:database"]};"); 
        }

        public static IDBConnection GetDBConnection()
        {
            if (dB == null)
            {
                dB = new DatabaseConnection();
            }
            return dB;
        }

        public int ExecuteStatement(INpgsqlCommand npgsqlCommand)
        {
            npgsqlCommand.Connection = npgsqlConnection;
            int affectedRows = npgsqlCommand.ExecuteNonQuery();
            return affectedRows;
        }

        public List<object[]> QueryDatabase(INpgsqlCommand npgsqlCommand)
        {
            List<object[]> results = new List<object[]>();
            npgsqlCommand.Connection = npgsqlConnection;
            using (INpgsqlDataReader resultReader = npgsqlCommand.ExecuteReader())
            {
                while (resultReader.Read())
                {
                    object[] row = new object[resultReader.FieldCount()];
                    for (int i = 0; i < row.Length; i++)
                    {
                        row[i] = resultReader.GetValue(i);
                    }
                    results.Add(row);
                }
            }
            return results;
        }
        public void OpenConnection() => npgsqlConnection.Open();
        public void CloseConnection() => npgsqlConnection.Close();
    }
}
