using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IQuestionBankRepository iQuestionBankRepository;
        public QuestionBankService()
        {
            iQuestionBankRepository = new QuestionBankRepository();
        }
        public List<QuestionBank> GetQuestions(int teacherId, int subjectId, int gradeLevel) => iQuestionBankRepository.GetQuestions(teacherId, subjectId, gradeLevel);
        public void SaveQuestion(QuestionBank question) => iQuestionBankRepository.SaveQuestion(question);

        public void UpdateQuestion(QuestionBank question) => iQuestionBankRepository.UpdateQuestion(question);

        public void DeleteQuestion(QuestionBank question) => iQuestionBankRepository.DeleteQuestion(question);
    }
}
