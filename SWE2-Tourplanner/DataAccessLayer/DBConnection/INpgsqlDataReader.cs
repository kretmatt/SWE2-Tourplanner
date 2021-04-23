using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBConnection
{
    public interface INpgsqlDataReader:IDisposable
    {
        bool Read();
        object GetValue(int i);
        int FieldCount();
    }
}
