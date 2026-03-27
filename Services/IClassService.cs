using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IClassService
    {
        List<Class> GetClasses();
        void SaveClass(Class c);
        void UpdateUpdateClass(Class c);
        void DeleteClass(Class c);
        Class GetClassById(int id);
    }
}
