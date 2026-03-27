using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class SubmissionDetailDAO
    {
        private static SubmissionDetailDAO instance = null;
        private static readonly object instanceLock = new object();

        private SubmissionDetailDAO() { }

        public static SubmissionDetailDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SubmissionDetailDAO();
                    }
                    return instance;
                }
            }
        }

        // Lấy chi tiết bài làm để hiện lại đáp án đã chọn
        public List<SubmissionDetail> GetDetailsBySubmissionId(int submissionId)
        {
            using var db = new SatouEduDbContext();
            return db.SubmissionDetails.Where(sd => sd.SubmissionId == submissionId).ToList();
        }

        // Lấy danh sách những lần trả lời sai của một câu hỏi cụ thể trong 1 bài kiểm tra (Phục vụ thống kê)
        public List<SubmissionDetail> GetWrongAnswersByQuestionId(int examId, int questionId)
        {
            using var db = new SatouEduDbContext();
            return db.SubmissionDetails
                     .Include(sd => sd.Submission)          // Lấy kèm thông tin Bài Nộp
                     .ThenInclude(s => s.Student)           // Lấy kèm luôn thông tin User (Học sinh)
                     .Where(sd => sd.QuestionId == questionId
                               && sd.IsCorrect == false
                               && sd.Submission.ExamId == examId)
                     .ToList();
        }

        // Lưu từng câu trả lời khi học sinh thi
        public void SaveSubmissionDetail(SubmissionDetail detail)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.SubmissionDetails.Add(detail);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}