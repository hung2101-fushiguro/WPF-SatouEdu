using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer
{
    public class ClassEnrollmentDAO
    {
        private static ClassEnrollmentDAO instance = null;
        private static readonly object instanceLock = new object();

        private ClassEnrollmentDAO() { }

        public static ClassEnrollmentDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new ClassEnrollmentDAO();
                    }
                    return instance;
                }
            }
        }
        public List<ClassEnrollment> GetPendingEnrollmentsByClass(int classId)
        {
            using var db = new SatouEduDbContext();
            // Status = 1: Chờ duyệt
            return db.ClassEnrollments
                     .Include(e => e.Student) 
                     .Where(e => e.ClassId == classId && e.Status == 1)
                     .ToList();
        }

        // Học sinh gửi yêu cầu tham gia lớp
        public void SaveEnrollment(ClassEnrollment enrollment)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.ClassEnrollments.Add(enrollment);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        // Giáo viên cập nhật trạng thái (Duyệt/Từ chối)
        public void UpdateEnrollment(ClassEnrollment enrollment)
        {
            try
            {
                using var context = new SatouEduDbContext();
                context.Entry<ClassEnrollment>(enrollment).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}