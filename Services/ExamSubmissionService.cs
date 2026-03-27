using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using BusinessObjects;

namespace Services
{
    public class ExamSubmissionService : IExamSubmissionService
    {
        private readonly IExamSubmissionRepository iExamSubmissionRepository;
        public ExamSubmissionService()
        {
            iExamSubmissionRepository = new ExamSubmissionRepository();
        }
        public List<ExamSubmission> GetSubmissionsByExamId(int examId) => iExamSubmissionRepository.GetSubmissionsByExamId(examId);
        public ExamSubmission GetSubmission(int examId, int studentId) => iExamSubmissionRepository.GetSubmission(examId, studentId);
        public void SaveSubmission(ExamSubmission submission) => iExamSubmissionRepository.SaveSubmission(submission);
        public void UpdateSubmission(ExamSubmission submission) => iExamSubmissionRepository.UpdateSubmission(submission);
    }
}
