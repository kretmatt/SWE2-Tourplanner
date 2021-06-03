namespace DataAccessLayer.DBCommands
{
    /// <summary>
    /// The IDBCommand interfaces defines two functions - Execute() and Undo() - which are needed for commits and rollbacks. In a commit of a IUnitOfWork, the Execute() function of every issued command is called. In a rollback, the Undo() function is executed. Is the interface for the command pattern.
    /// </summary>
    public interface IDBCommand
    {
        /// <summary>
        /// In the Execute() method, the data in the database is altered or new data gets inserted.
        /// </summary>
        /// <returns>Amount of rows affected by an IDBCommand instance's Execute() method.</returns>
        int Execute();
        /// <summary>
        /// The Undo() function is basically the opposite of the Execute() method functionality-wise.
        /// </summary>
        /// <returns>Amount of rows affected by an IDBCommand instance's Undo() method.</returns>
        int Undo();
    }
}
