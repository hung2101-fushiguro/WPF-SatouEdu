using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface ISubmissionDetailService
    {
        List<SubmissionDetail> GetDetailsBySubmissionId(int submissionId);
        List<SubmissionDetail> GetWrongAnswersByQuestionId(int examId, int questionId);
        void SaveSubmissionDetail(SubmissionDetail detail);
    }
}
