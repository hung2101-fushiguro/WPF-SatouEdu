using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ExamQuestionDAO
    {
        private static ExamQuestionDAO instance = null;
        private static readonly object instanceLock = new object();

        private ExamQuestionDAO() { }

        public static ExamQuestionDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ExamQuestionDAO();
                    }
                    return instance;
                }
            }
        }

        // Lấy danh sách câu hỏi của một đề thi, sắp xếp theo OrderIndex
        public List<ExamQuestion> GetExamQuestions(int examId)
        {
            using var db = new SatouEduDbContext();
            return db.ExamQuestions.Where(eq => eq.ExamId == examId).OrderBy(eq => eq.OrderIndex).ToList();
        }

        // Thêm câu hỏi vào đề thi
        public void SaveExamQuestion(ExamQuestion eq)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.ExamQuestions.Add(eq);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Xóa một câu hỏi khỏi đề thi (Dùng khóa chính kép: examId, questionId)
        public void DeleteExamQuestion(int examId, int questionId)
        {
            try
            {
                using var context = new SatouEduDbContext();
                var eq = context.ExamQuestions.SingleOrDefault(x => x.ExamId == examId && x.QuestionId == questionId);
                if (eq != null)
                {
                    context.ExamQuestions.Remove(eq);
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}