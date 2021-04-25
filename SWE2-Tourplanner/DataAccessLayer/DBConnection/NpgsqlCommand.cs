using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace DataAccessLayer.DBConnection
{
    /// <summary>
    /// NpgsqlCommmand is the command class for NPGSQL commands and it implements the functions defined in the IDbCommand interface.
    /// </summary>
    public class NpgsqlCommand : IDbCommand
    {
        /// <summary>
        /// The actual command. All functions basically direct requests to this object.
        /// </summary>
        private Npgsql.NpgsqlCommand npgsqlCommand;
        /// <summary>
        /// Executes the command (DML)
        /// </summary>
        /// <returns>Amount of affected rows</returns>
        public int ExecuteNonQuery() => Connection != null ? npgsqlCommand.ExecuteNonQuery() : 0;
        /// <summary>
        /// Executes the query
        /// </summary>
        /// <returns>DataReader for retrieval of results</returns>
        public IDataReader ExecuteReader() => Connection != null ? new NpgsqlDataReader(npgsqlCommand.ExecuteReader()) : null;
        /// <summary>
        /// Parameters of the command.
        /// </summary>
        public DbParameterCollection Parameters => npgsqlCommand.Parameters;
        /// <summary>
        /// Connection to the database where the command gets executed.
        /// </summary>
        public DbConnection Connection
        {
            get
            {
                return npgsqlCommand.Connection;
            }
            set
            {
                npgsqlCommand.Connection = (NpgsqlConnection)value;
            }
        }
        /// <summary>
        /// Creates an NpgsqlCommand with the specified text.
        /// </summary>
        /// <param name="cmdText">Text of the command.</param>
        public NpgsqlCommand(string cmdText)
        {
            npgsqlCommand = new Npgsql.NpgsqlCommand(cmdText);
        }
    }
}
