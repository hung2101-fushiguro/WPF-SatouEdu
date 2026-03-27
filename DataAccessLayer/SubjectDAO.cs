using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class SubjectDAO
    {
        private static SubjectDAO instance = null;
        private static readonly object instanceLock = new object();

        private SubjectDAO() { }

        public static SubjectDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SubjectDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Subject> GetSubjects()
        {
            var listSubjects = new List<Subject>();
            try
            {
                using var context = new SatouEduDbContext();
                listSubjects = context.Subjects.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listSubjects;
        }

        public Subject GetSubjectById(int id)
        {
            using var db = new SatouEduDbContext();
            return db.Subjects.FirstOrDefault(c => c.SubjectId == id);
        }
    }
}