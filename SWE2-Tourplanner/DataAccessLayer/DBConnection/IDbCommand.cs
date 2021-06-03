using System.Data.Common;

namespace DataAccessLayer.DBConnection
{
    public interface IDbCommand
    {
        /// <summary>
        /// Executes the command (DML - INSERT, UPDATE, DELETE)
        /// </summary>
        /// <returns>Amount of affected rows.</returns>
        int ExecuteNonQuery();
        /// <summary>
        /// Executes the command (query) and returns a reader for retrieving the results. 
        /// </summary>
        /// <returns>DataReader which can be used for getting the results of a query.</returns>
        IDataReader ExecuteReader();
        /// <summary>
        /// Parameters of the command.
        /// </summary>
        DbParameterCollection Parameters { get; }
        /// <summary>
        /// The connection to the database, where the command gets executed.
        /// </summary>
        DbConnection Connection { get; set; }
    }
}
