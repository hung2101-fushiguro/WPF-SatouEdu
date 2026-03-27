using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Repositories
{
    public interface IExamSubmissionRepository
    {
        public List<ExamSubmission> GetSubmissionsByExamId(int examId);
        public ExamSubmission GetSubmission(int examId, int studentId);
        public void SaveSubmission(ExamSubmission submission);
        public void UpdateSubmission(ExamSubmission submission);
    }
}
