using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IExamRepository
    {
        public List<Exam> GetExamsByClassId(int classId);
        public Exam GetExamById(int examId);
        public void SaveExam(Exam exam);
        public void UpdateExam(Exam exam);
        public void DeleteExam(Exam exam);
    }
}
