using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.ExporterImporter
{
    public interface IExporterImporter<T>
    {
        Task<List<T>> Import(string filePath);
        Task Export(List<T> entites);
    }
}
