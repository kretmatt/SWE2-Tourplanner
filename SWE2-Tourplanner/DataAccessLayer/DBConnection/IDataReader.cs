using System;

namespace DataAccessLayer.DBConnection
{
    /// <summary>
    /// Interface used for exposing only necessary methods / attributes of DataReaders to the application
    /// </summary>
    public interface IDataReader:IDisposable
    {
        /// <summary>
        /// Read() checks if rows are still left or not.
        /// </summary>
        /// <returns>True if rows are still left, false if no rows are left.</returns>
        bool Read();
        /// <summary>
        /// Returns the value of a specific field.
        /// </summary>
        /// <param name="i">Position / Field number</param>
        /// <returns>Value of the field i</returns>
        object GetValue(int i);
        /// <value>
        /// Returns the amount of fields of a row.
        /// </value>
        int FieldCount { get; }
    }
}
