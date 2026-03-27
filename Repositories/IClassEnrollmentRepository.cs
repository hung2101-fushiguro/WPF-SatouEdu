using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface IClassEnrollmentRepository
    {
        public List<ClassEnrollment> GetPendingEnrollmentsByClass(int classId);
        public void SaveEnrollment(ClassEnrollment enrollment);
        public void UpdateEnrollment(ClassEnrollment enrollment);
    }
}
