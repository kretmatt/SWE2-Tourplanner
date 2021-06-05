using Common.Config;
using Common.Logging;
using System;
using System.Collections.Generic;
using DataAccessLayer.Exceptions;

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
        /// <summary>
        /// Tourplanner config object. Used to retrieve the newest config data.
        /// </summary>
        private ITourPlannerConfig config;
        /// <summary>
        /// ILog object used for logging.
        /// </summary>
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
        /// Creates a DatabaseConnection instance. Is only called once due to GetDBConnection(ITourPlannerConfig testConfig) and the private access modifier.
        /// </summary>
        /// <param name="config">Tourplanner configuration used for establishing database connection</param>
        private DatabaseConnection(ITourPlannerConfig config)
        {
            logger = LogHelper.GetLogHelper().GetLogger();
            this.config = config;
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
        /// Ensures that the DatabaseConnection constructor only gets called once. Used for testing purposes. The test configuration needs to be passed
        /// </summary>
        /// <param name="testConfig">Tourplanner configuration used for establishing database connection. Most of the time just some test data</param>
        /// <returns>The only DatabaseConnection instance.</returns>
        public static IDBConnection GetDBConnection(ITourPlannerConfig testConfig)
        {
            if (dB == null)
            {
                dB = new DatabaseConnection(testConfig);
            }
            return dB;
        }
        /// <summary>
        /// Executes a DML statement and returns the amount of affected rows
        /// </summary>
        /// <param name="command">DML statement to be executed</param>
        /// <returns>Amount of affected rows</returns>
        /// <exception cref="DALDBConnectionException">Thrown, when the command can't be executed properly</exception>
        public int ExecuteStatement(IDbCommand command)
        {
            int affectedRows;
            try
            {
                command.Connection = npgsqlConnection;
                affectedRows = command.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                logger.Error($"The following error occured during execution of a command: {e.Message}");
                throw new DALDBConnectionException("Command could not be excuted properly");
            }
            return affectedRows;
        }
        /// <summary>
        /// Executes a query and retrievs the results
        /// </summary>
        /// <param name="command">Query to be executed</param>
        /// <returns>Results of the query as a collection of object arrays</returns>
        /// <exception cref="DALDBConnectionException">Thrown, when error occurs whilst querying the database</exception>
        public List<object[]> QueryDatabase(IDbCommand command)
        {
            List<object[]> results = new List<object[]>();
            try
            {
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
            }
            catch(Exception e)
            {
                logger.Error($"The following error occured whilst querying the database: {e.Message}");
                throw new DALDBConnectionException("The wanted data could not be retrieved!");
            }
            
            return results;
        }
        /// <summary>
        /// Opens the database connection
        /// </summary>
        /// <exception cref="DALDBConnectionException">Thrown, when connection to the database can't be opened</exception>
        public void OpenConnection() 
        {
            try
            {
                npgsqlConnection.ConnectionString = config.DatabaseConnectionString;
                npgsqlConnection.Open();
            }
            catch(Exception e)
            {
                logger.Error($"Could not open connection to database with the current connection string properly. Error: {e.Message}");
                throw new DALDBConnectionException("A connection to the database could not be established!");
            }
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
        /// <exception cref="DALParameterException">Thrown, when parameter can't be declared</exception>
        public int DeclareParameter(IDbCommand command, string name, System.Data.DbType type)
        {
            try
            {
                if (!command.Parameters.Contains(name))
                {
                    int index = command.Parameters.Add(new Npgsql.NpgsqlParameter(name, type));
                    return index;
                }
                logger.Error($"Error occured during declaration of parameter. The parameter {name} already exists and can therefore not be declared again.");
                throw new ArgumentException(string.Format("Parameter {0} already exists", name));
            }
            catch(Exception e)
            {
                throw new DALParameterException($"Some error occured during declaration process of parameters: {e.Message}");
            }
            
        }
        /// <summary>
        /// Declares and defines a parmeter in a command.
        /// </summary>
        /// <param name="command">Command where the parameter gets defined</param>
        /// <param name="name">Name of the parameter</param>
        /// <param name="type">Type of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        /// <exception cref="DALParameterException">Thrown, when declaration / setting of parameter can't be conducted</exception>
        public void DefineParameter(IDbCommand command, string name, System.Data.DbType type, object value)
        {
            try
            {
                int index = DeclareParameter(command, name, type);
                command.Parameters[index].Value = value;
            }
            catch(Exception e)
            {
                if (e is DALParameterException)
                    throw;
                logger.Error("Could not set parameter properly!");
                throw new DALParameterException("Declaration of parameter was successful, but setting the value caused a problem!");
            }
            
        }
    }
}
