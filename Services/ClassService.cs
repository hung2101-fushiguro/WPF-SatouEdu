using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ClassService : IClassService 
    {
        private readonly IClassRepository iClassRepository;
        public ClassService()
        {
            iClassRepository = new ClassRepository();
        }
        public List<Class> GetClasses() => iClassRepository.GetClasses();
        public void SaveClass(Class c) => iClassRepository.SaveClass(c);
        public void UpdateUpdateClass(Class c) => iClassRepository.UpdateUpdateClass(c);
        public void DeleteClass(Class c) => iClassRepository?.DeleteClass(c);
        public Class GetClassById(int id) => iClassRepository.GetClassById(id);
    }
}
