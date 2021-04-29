using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    /// <summary>
    /// The IDbConnection interface defines several methods for setting parameters in IDBCommand instances, opening/closing the database connection and for querying / executing statements.
    /// </summary>
    public interface IDBConnection
    {
        /// <summary>
        /// Executes a statement (!=Query).
        /// </summary>
        /// <param name="command">Command to be executed.</param>
        /// <returns>Amount of rows affected by the statement.</returns>
        int ExecuteStatement(IDbCommand command);
        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="command">Query to be executed.</param>
        /// <returns>Collection of data(=rows) returned by the query.</returns>
        List<object[]> QueryDatabase(IDbCommand command);
        /// <summary>
        /// Declares a parameter of the command
        /// </summary>
        /// <param name="command">Command where the parameter is being set.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the new parameter.</param>
        /// <returns></returns>
        int DeclareParameter(IDbCommand command, string name, DbType type);
        /// <summary>
        /// Declares a parameter and defines it afterwards.
        /// </summary>
        /// <param name="command">Command where the parameter is being set.</param>
        /// <param name="name">Name of the parameter.</param>
        /// <param name="type">Type of the parameter.</param>
        /// <param name="value">Value to be inserted.</param>
        void DefineParameter(IDbCommand command, string name, DbType type, object value);
        /// <summary>
        /// Opens the connection to the database.
        /// </summary>
        void OpenConnection();
        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        void CloseConnection();
    }
}
