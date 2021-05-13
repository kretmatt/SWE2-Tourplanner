using Common.Config;
using Common.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    /// <summary>
    /// DatabaseConnection implements IDBConnection and is the DBConnection-Class for Npgsql. Is a singleton.
    /// </summary>
    public class DatabaseConnection : IDBConnection
    {
        /// <summary>
        /// Only instance of the DatabaseConnection class. Gets set once.
        /// </summary>
        private static DatabaseConnection dB;
        /// <summary>
        /// Connection to the Postgres database
        /// </summary>
        private Npgsql.NpgsqlConnection npgsqlConnection;

        private ITourPlannerConfig config;

        private log4net.ILog logger;
        /// <summary>
        /// Creates a DatabaseConnection instance. Is only called once due to GetDBConnection() and the private access modifier.
        /// </summary>
        private DatabaseConnection()
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            config = TourPlannerConfig.GetTourPlannerConfig();
            npgsqlConnection = new Npgsql.NpgsqlConnection(config.DatabaseConnectionString);
        }
        /// <summary>
        /// Ensures that the DatabaseConnection constructor only gets called once. Provides the DatabaseConnection instance.
        /// </summary>
        /// <returns>The only DatabaseConnection instance.</returns>
        public static IDBConnection GetDBConnection()
        {
            if (dB == null)
            {
                dB = new DatabaseConnection();
            }
            return dB;
        }
        /// <summary>
        /// Executes a DML statement and returns the amount of affected rows
        /// </summary>
        /// <param name="command">DML statement to be executed</param>
        /// <returns>Amount of affected rows</returns>
        public int ExecuteStatement(IDbCommand command)
        {
            command.Connection = npgsqlConnection;
            int affectedRows = command.ExecuteNonQuery();
            return affectedRows;
        }
        /// <summary>
        /// Executes a query and retrievs the results
        /// </summary>
        /// <param name="command">Query to be executed</param>
        /// <returns>Results of the query as a collection of object arrays</returns>
        public List<object[]> QueryDatabase(IDbCommand command)
        {
            List<object[]> results = new List<object[]>();
            command.Connection = npgsqlConnection;
            using (IDataReader resultReader = command.ExecuteReader())
            {
                while (resultReader.Read())
                {
                    object[] row = new object[resultReader.FieldCount];
                    for (int i = 0; i < row.Length; i++)
                    {
                        row[i] = resultReader.GetValue(i);
                    }
                    results.Add(row);
                }
            }
            return results;
        }
        /// <summary>
        /// Opens the database connection
        /// </summary>
        public void OpenConnection() 
        {
            npgsqlConnection.ConnectionString = config.DatabaseConnectionString;
            npgsqlConnection.Open();
        }
        /// <summary>
        /// Closes the database connection
        /// </summary>
        public void CloseConnection() => npgsqlConnection.Close();
        /// <summary>
        /// Declares a parameter in a command
        /// </summary>
        /// <param name="command">Command where the parameter gets declared</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="type">Type of the parameter</param>
        /// <returns>Parameter index</returns>
        public int DeclareParameter(IDbCommand command, string name, System.Data.DbType type)
        {
            if (!command.Parameters.Contains(name))
            {
                int index = command.Parameters.Add(new Npgsql.NpgsqlParameter(name, type));
                return index;
            }
            logger.Error($"Error occured during declaration of parameter. The parameter {name} already exists and can therefore not be declared again.");
            throw new ArgumentException(string.Format("Parameter {0} already exists", name));
        }
        /// <summary>
        /// Declares and defines a parmeter in a command.
        /// </summary>
        /// <param name="command">Command where the parameter gets defined</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="type">Type of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        public void DefineParameter(IDbCommand command, string name, System.Data.DbType type, object value)
        {
            int index = DeclareParameter(command, name, type);
            command.Parameters[index].Value = value;
        }
    }
}
