using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class ClassRepository : IClassRepository
    {
        public List<Class> GetClasses() => ClassDAO.Instance.GetClasses();
        public void SaveClass(Class c) => ClassDAO.Instance.SaveClass(c);
        public void UpdateUpdateClass(Class c) => ClassDAO.Instance.UpdateUpdateClass(c);
        public void DeleteClass(Class c) => ClassDAO.Instance.DeleteClass(c);
        public Class GetClassById(int id) => ClassDAO.Instance.GetClassById(id);

    }
}
