using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ExamDAO
    {
        private static ExamDAO instance = null;
        private static readonly object instanceLock = new object();

        private ExamDAO() { }

        public static ExamDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ExamDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Exam> GetExamsByClassId(int classId)
        {
            using var db = new SatouEduDbContext();
            return db.Exams.Where(e => e.ClassId == classId).ToList();
        }

        public Exam GetExamById(int examId)
        {
            using var db = new SatouEduDbContext();
            return db.Exams.FirstOrDefault(e => e.ExamId == examId);
        }

        public void SaveExam(Exam exam)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Exams.Add(exam);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void UpdateExam(Exam exam)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Entry<Exam>(exam).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void DeleteExam(Exam exam)
        {
            try
            {
                using var context = new SatouEduDbContext();
                var e1 = context.Exams.SingleOrDefault(x => x.ExamId == exam.ExamId);
                if (e1 != null)
                {
                    context.Exams.Remove(e1);
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