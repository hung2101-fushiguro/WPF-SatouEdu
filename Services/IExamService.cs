using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IExamService
    {
        List<Exam> GetExamsByClassId(int classId);
        Exam GetExamById(int examId);
        void SaveExam(Exam exam);
        void UpdateExam(Exam exam);
        void DeleteExam(Exam exam);
    }
}
