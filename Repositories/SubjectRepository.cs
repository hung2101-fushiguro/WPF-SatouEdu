using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        public List<Subject> GetSubjects() => SubjectDAO.Instance.GetSubjects();
        public Subject GetSubjectById(int id) => SubjectDAO.Instance.GetSubjectById(id);
    }
}
