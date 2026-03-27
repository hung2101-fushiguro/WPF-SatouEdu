using BusinessObjects;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public class ClassEnrollmentRepository : IClassEnrollmentRepository
    {
        public List<ClassEnrollment> GetPendingEnrollmentsByClass(int classId) => ClassEnrollmentDAO.Instance.GetPendingEnrollmentsByClass(classId);
        public void SaveEnrollment(ClassEnrollment enrollment) => ClassEnrollmentDAO.Instance.SaveEnrollment(enrollment);
        public void UpdateEnrollment(ClassEnrollment enrollment) => ClassEnrollmentDAO.Instance.UpdateEnrollment(enrollment);
    }
}
