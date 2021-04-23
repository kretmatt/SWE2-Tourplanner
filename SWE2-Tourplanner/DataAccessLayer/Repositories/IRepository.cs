using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Repositories
{
    public interface IRepository<T> where T:class
    {
        void Insert(T entity);
        T Read(int id);
        List<T> ReadAll();
        void Update(T entity);
        void Delete(int id);
    }
}
