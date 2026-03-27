using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ExamSubmissionDAO
    {
        private static ExamSubmissionDAO instance = null;
        private static readonly object instanceLock = new object();

        private ExamSubmissionDAO() { }

        public static ExamSubmissionDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ExamSubmissionDAO();
                    }
                    return instance;
                }
            }
        }

        // Giáo viên xem toàn bộ kết quả của 1 bài kiểm tra để tạo bảng xếp hạng
        public List<ExamSubmission> GetSubmissionsByExamId(int examId)
        {
            using var db = new SatouEduDbContext();
            return db.ExamSubmissions.Where(s => s.ExamId == examId).OrderByDescending(s => s.Score).ToList();
        }

        // Học sinh xem lại bài làm của chính mình
        public ExamSubmission GetSubmission(int examId, int studentId)
        {
            using var db = new SatouEduDbContext();
            return db.ExamSubmissions.FirstOrDefault(s => s.ExamId == examId && s.StudentId == studentId);
        }

        // Lưu lượt làm bài mới
        public void SaveSubmission(ExamSubmission submission)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.ExamSubmissions.Add(submission);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Cập nhật điểm số khi nộp bài
        public void UpdateSubmission(ExamSubmission submission)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Entry<ExamSubmission>(submission).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}