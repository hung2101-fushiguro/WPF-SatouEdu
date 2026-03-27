using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class UserDAO
    {
        private static UserDAO instance = null;
        private static readonly object instanceLock = new object();

        private UserDAO() { }

        public static UserDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new UserDAO();
                    }
                    return instance;
                }
            }
        }

       
        public User GetUserByEmail(string email)
        {
            using var db = new SatouEduDbContext();
            return db.Users.FirstOrDefault(c => c.Email.Equals(email));
        }

        public void SaveUser(User u)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Users.Add(u);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<User> GetStudents()
        {
            var listStudents = new List<User>();
            try
            {
                using var context = new SatouEduDbContext();
                listStudents = context.Users.Where(u => u.Role == 2).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listStudents;
        }
    }
}