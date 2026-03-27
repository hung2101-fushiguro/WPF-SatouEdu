using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using BusinessObjects;

namespace Services
{
    public class ExamQuestionService : IExamQuestionService
    {
        private readonly IExamQuestionRepository iExamQuestionRepository;
        public ExamQuestionService()
        {
            iExamQuestionRepository = new ExamQuestionRepository();
        }
        public List<ExamQuestion> GetExamQuestions(int examId) => iExamQuestionRepository.GetExamQuestions(examId);
        public void SaveExamQuestion(ExamQuestion eq) => iExamQuestionRepository.SaveExamQuestion(eq);
        public void DeleteExamQuestion(int examId, int questionId) => iExamQuestionRepository.DeleteExamQuestion(examId, questionId);
    }
}
