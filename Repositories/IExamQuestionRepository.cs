using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IExamQuestionRepository
    {
        public List<ExamQuestion> GetExamQuestions(int examId);
        public void SaveExamQuestion(ExamQuestion eq);
        public void DeleteExamQuestion(int examId, int questionId);
    }
}
