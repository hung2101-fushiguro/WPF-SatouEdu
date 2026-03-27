using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class SubmissionDetailRepository : ISubmissionDetailRepository
    {
        public List<SubmissionDetail> GetDetailsBySubmissionId(int submissionId) => SubmissionDetailDAO.Instance.GetDetailsBySubmissionId(submissionId);
        public List<SubmissionDetail> GetWrongAnswersByQuestionId(int examId, int questionId) => SubmissionDetailDAO.Instance.GetWrongAnswersByQuestionId(examId, questionId);
        public void SaveSubmissionDetail(SubmissionDetail detail) => SubmissionDetailDAO.Instance.SaveSubmissionDetail(detail);
    }
}
