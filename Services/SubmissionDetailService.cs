using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Repositories;

namespace Services
{
    public class SubmissionDetailService : ISubmissionDetailService
    {
        private readonly ISubmissionDetailRepository iSubmissionDetailRepository;
        public SubmissionDetailService()
        {
            iSubmissionDetailRepository = new SubmissionDetailRepository();
        }
        public List<SubmissionDetail> GetDetailsBySubmissionId(int submissionId) => iSubmissionDetailRepository.GetDetailsBySubmissionId(submissionId);
        public List<SubmissionDetail> GetWrongAnswersByQuestionId(int examId, int questionId) => iSubmissionDetailRepository.GetWrongAnswersByQuestionId(examId, questionId);
        public void SaveSubmissionDetail(SubmissionDetail detail) => iSubmissionDetailRepository.SaveSubmissionDetail(detail);

    }
}
