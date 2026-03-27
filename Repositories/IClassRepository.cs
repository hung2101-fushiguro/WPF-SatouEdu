using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories
{
    public interface IClassRepository
    {
        public List<Class> GetClasses();
        public void SaveClass(Class c);
        public void UpdateUpdateClass(Class c);
        public void DeleteClass(Class c);
        Class GetClassById(int id);
    }
}
