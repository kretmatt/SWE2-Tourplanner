using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ExporterImporter
{
    /// <summary>
    /// A generic interface that defines basic methods for importing and exporting entities
    /// </summary>
    /// <typeparam name="T">Type of data that gets imported / exported</typeparam>
    public interface IExporterImporter<T>
    {
        /// <summary>
        /// Method for importing data from JSON file
        /// </summary>
        /// <param name="filePath">Path to the file that gets imported</param>
        /// <returns>Task, that returns list of entities from file</returns>
        Task<List<T>> Import(string filePath);
        /// <summary>
        /// Method for exporting data as JSON file
        /// </summary>
        /// <param name="entites">Entities which will be exported</param>
        /// <returns>Task, which exports entities as JSON</returns>
        Task Export(List<T> entites);
    }
}
