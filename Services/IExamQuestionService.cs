using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IExamQuestionService
    {
        List<ExamQuestion> GetExamQuestions(int examId);
        void SaveExamQuestion(ExamQuestion eq);
        void DeleteExamQuestion(int examId, int questionId);
    }
}
