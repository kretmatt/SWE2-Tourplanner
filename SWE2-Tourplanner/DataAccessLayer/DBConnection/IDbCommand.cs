using System.Data.Common;

namespace DataAccessLayer.DBConnection
{
    /// <summary>
    /// Interface used for exposing only necessary methods / properties to the application.
    /// </summary>
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
        /// <value>
        /// Parameters of the command.
        /// </value>
        DbParameterCollection Parameters { get; }
        /// <value>
        /// The connection to the database, where the command gets executed.
        /// </value>
        DbConnection Connection { get; set; }
    }
}
