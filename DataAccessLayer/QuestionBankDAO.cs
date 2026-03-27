using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class QuestionBankDAO
    {
        private static QuestionBankDAO instance = null;
        private static readonly object instanceLock = new object();

        private QuestionBankDAO() { }

        public static QuestionBankDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new QuestionBankDAO();
                    }
                    return instance;
                }
            }
        }

        // Trong file QuestionBankDAO.cs
        public List<QuestionBank> GetQuestions(int teacherId, int subjectId, int gradeLevel)
        {
            using var db = new SatouEduDbContext();
            return db.QuestionBanks
                     .Where(q => q.TeacherId == teacherId
                              && q.SubjectId == subjectId
                              && q.GradeLevel == gradeLevel) // Lọc thêm theo khối lớp
                     .ToList();
        }

        public void SaveQuestion(QuestionBank question)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.QuestionBanks.Add(question);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateQuestion(QuestionBank question)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Entry<QuestionBank>(question).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteQuestion(QuestionBank question)
        {
            try
            {
                using var context = new SatouEduDbContext();
                var q1 = context.QuestionBanks.SingleOrDefault(x => x.QuestionId == question.QuestionId);
                if (q1 != null)
                {
                    context.QuestionBanks.Remove(q1);
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