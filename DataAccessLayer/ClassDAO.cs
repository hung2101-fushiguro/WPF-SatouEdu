using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ClassDAO
    {
        private static ClassDAO instance = null;
        private static readonly object instanceLock = new object();

        private ClassDAO() { }

        public static ClassDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ClassDAO();
                    }
                    return instance;
                }
            }
        }

        public List<Class> GetClasses()
        {
            var listClasses = new List<Class>();
            try
            {
                using var db = new SatouEduDbContext();
                listClasses = db.Classes.ToList();
            }
            catch (Exception e) { }
            return listClasses;
        }

        public void SaveClass(Class c)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Classes.Add(c);
                context.SaveChanges();
            }
            catch (Exception e) 
            {
                throw new Exception(e.Message);
            }
        }
        public void UpdateUpdateClass(Class c) 
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Entry<Class>(c).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch(Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public void DeleteClass(Class c)
        {
            try
            {
                using var context = new SatouEduDbContext();
                var c1 = context.Classes.SingleOrDefault(x => x.ClassId == c.ClassId);
                context.Classes.Remove(c1);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public Class GetClassById(int id)
        {
            using var db = new SatouEduDbContext();
            return db.Classes.FirstOrDefault(c => c.ClassId.Equals(id));
        }
    }
}