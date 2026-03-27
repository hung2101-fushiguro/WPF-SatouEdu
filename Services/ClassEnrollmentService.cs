using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repositories;
using BusinessObjects;

namespace Services
{
    public class ClassEnrollmentService : IClassEnrollmentService
    {
        private readonly IClassEnrollmentRepository iClassEnrollmentRepository;
        public ClassEnrollmentService()
        {
            iClassEnrollmentRepository = new ClassEnrollmentRepository();
        }
        public List<ClassEnrollment> GetPendingEnrollmentsByClass(int classId) => iClassEnrollmentRepository.GetPendingEnrollmentsByClass(classId);
        public void SaveEnrollment(ClassEnrollment enrollment) => iClassEnrollmentRepository.SaveEnrollment(enrollment);
        public void UpdateEnrollment(ClassEnrollment enrollment) => iClassEnrollmentRepository.UpdateEnrollment(enrollment);
    }
}
