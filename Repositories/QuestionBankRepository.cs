using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class QuestionBankRepository : IQuestionBankRepository
    {
        public List<QuestionBank> GetQuestions(int teacherId, int subjectId, int gradeLevel) => QuestionBankDAO.Instance.GetQuestions(teacherId, subjectId, gradeLevel);
        public void SaveQuestion(QuestionBank question) => QuestionBankDAO.Instance.SaveQuestion(question);
        public void UpdateQuestion(QuestionBank question) => QuestionBankDAO.Instance.UpdateQuestion(question);
        public void DeleteQuestion(QuestionBank question) => QuestionBankDAO.Instance.DeleteQuestion(question);
    }
}
