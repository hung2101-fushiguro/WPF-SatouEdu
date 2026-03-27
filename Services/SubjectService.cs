using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using BusinessObjects;

namespace Services
{
    public class SubjectService : ISubjectService
    {
        private readonly SubjectRepository iSubjectRepository;
        public SubjectService()
        {
            iSubjectRepository = new SubjectRepository();
        }
        public List<Subject> GetSubjects() => iSubjectRepository.GetSubjects();
        public Subject GetSubjectById(int id) => iSubjectRepository.GetSubjectById(id);
    }
}
