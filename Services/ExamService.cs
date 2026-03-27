using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using BusinessObjects;

namespace Services
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository iExamRepository;
        public ExamService()
        {
            iExamRepository = new ExamRepository();
        }
        public List<Exam> GetExamsByClassId(int classId) => iExamRepository.GetExamsByClassId(classId);
        public Exam GetExamById(int examId) => iExamRepository.GetExamById(examId);
        public void SaveExam(Exam exam) => iExamRepository.SaveExam(exam);
        public void UpdateExam(Exam exam) => iExamRepository.UpdateExam(exam);
        public void DeleteExam(Exam exam) => iExamRepository.DeleteExam(exam);
    }
}
