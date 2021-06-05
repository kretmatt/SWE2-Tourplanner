namespace DataAccessLayer.DBConnection
{
    /// <summary>
    /// NpgsqlDataReader is the concrete IDataReader implementation for Npgsql package. All functions/properties direct requests to a Npgsql.NpgsqlDataReader object.
    /// </summary>
    public class NpgsqlDataReader : IDataReader
    {
        /// <summary>
        /// The actual datareader. All requests are redirected to this object.
        /// </summary>
        private Npgsql.NpgsqlDataReader npgsqlDataReader;
        /// <summary>
        /// Creates the NpgsqlDataReader object
        /// </summary>
        /// <param name="npgsqlDataReader">The actual datareader</param>
        public NpgsqlDataReader(Npgsql.NpgsqlDataReader npgsqlDataReader)
        {
            this.npgsqlDataReader = npgsqlDataReader;
        }
        /// <summary>
        /// Specifies how many fields the rows have
        /// </summary>
        public int FieldCount { get { return npgsqlDataReader.FieldCount; } }
        /// <summary>
        /// Checks if reading is possible or not
        /// </summary>
        /// <returns>True if reading is possible, else returns false</returns>
        public bool Read() => npgsqlDataReader.Read();
        /// <summary>
        /// Retrieves the value stored in the specified field
        /// </summary>
        /// <param name="i">Field number</param>
        /// <returns>Value in the specified field</returns>
        public object GetValue(int i) => npgsqlDataReader.GetValue(i);
        /// <summary>
        /// Disposal of the npgsqlDataReader object.
        /// </summary>
        public void Dispose()
        {
            npgsqlDataReader.DisposeAsync();
        }
    }
}
