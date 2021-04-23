using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DBCommands
{
    public interface IDBCommand
    {
        int Execute();
        int Undo();
    }
}
