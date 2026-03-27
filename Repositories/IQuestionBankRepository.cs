using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IQuestionBankRepository
    {
        public List<QuestionBank> GetQuestions(int teacherId, int subjectId, int gradelevel);
        public void SaveQuestion(QuestionBank question);
        public void UpdateQuestion(QuestionBank question);
        public void DeleteQuestion(QuestionBank question);
    }
}
