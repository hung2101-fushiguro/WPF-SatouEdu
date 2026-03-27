using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ExamRepository : IExamRepository
    {
        public List<Exam> GetExamsByClassId(int classId) => ExamDAO.Instance.GetExamsByClassId(classId);
        public Exam GetExamById(int examId) => ExamDAO.Instance.GetExamById(examId);
        public void SaveExam(Exam exam) => ExamDAO.Instance.SaveExam(exam);
        public void UpdateExam(Exam exam) => ExamDAO.Instance.UpdateExam(exam);
        public void DeleteExam(Exam exam) => ExamDAO.Instance.DeleteExam(exam);
    }
}
