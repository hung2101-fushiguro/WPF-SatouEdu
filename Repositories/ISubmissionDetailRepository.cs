using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories
{
    public interface ISubmissionDetailRepository
    {
        public List<SubmissionDetail> GetDetailsBySubmissionId(int submissionId);
        public List<SubmissionDetail> GetWrongAnswersByQuestionId(int examId, int questionId);
        public void SaveSubmissionDetail(SubmissionDetail detail);
    }
}
