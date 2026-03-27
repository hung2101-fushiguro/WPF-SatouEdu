using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISubjectRepository
    {
        public List<Subject> GetSubjects();
        public Subject GetSubjectById(int id);
    }
}
