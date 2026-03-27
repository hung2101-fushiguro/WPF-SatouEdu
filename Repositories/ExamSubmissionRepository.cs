using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace Repositories
{
    public class ExamSubmissionRepository : IExamSubmissionRepository
    {
        public List<ExamSubmission> GetSubmissionsByExamId(int examId) => ExamSubmissionDAO.Instance.GetSubmissionsByExamId(examId);
        public ExamSubmission GetSubmission(int examId, int studentId) => ExamSubmissionDAO.Instance.GetSubmission(examId, studentId);
        public void SaveSubmission(ExamSubmission submission) => ExamSubmissionDAO.Instance.SaveSubmission(submission);
        public void UpdateSubmission(ExamSubmission submission) => ExamSubmissionDAO.Instance.UpdateSubmission(submission);
    }
}
