using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer;
using BusinessObjects;

namespace Repositories
{
    public class ExamQuestionRepository : IExamQuestionRepository
    {
        public List<ExamQuestion> GetExamQuestions(int examId) => ExamQuestionDAO.Instance.GetExamQuestions(examId);
        public void SaveExamQuestion(ExamQuestion eq) => ExamQuestionDAO.Instance.SaveExamQuestion(eq);
        public void DeleteExamQuestion(int examId, int questionId) => ExamQuestionDAO.Instance.DeleteExamQuestion(examId, questionId);
    }
}
