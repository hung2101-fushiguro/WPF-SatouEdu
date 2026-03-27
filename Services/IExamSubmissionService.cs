using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace Services
{
    public interface IExamSubmissionService
    {
        List<ExamSubmission> GetSubmissionsByExamId(int examId);
        ExamSubmission GetSubmission(int examId, int studentId);
        void SaveSubmission(ExamSubmission submission);
        void UpdateSubmission(ExamSubmission submission);
    }
}
