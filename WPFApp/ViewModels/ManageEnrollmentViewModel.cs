using BusinessObjects;
using Services;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class ManageEnrollmentViewModel : ViewModelBase
    {
        private readonly IClassEnrollmentService _enrollmentService;
        private readonly int _classId;

        public ObservableCollection<ClassEnrollment> PendingStudents { get; set; }

        public ICommand ApproveCommand { get; }
        public ICommand RejectCommand { get; }

        public ManageEnrollmentViewModel(int classId)
        {
            _classId = classId;
            _enrollmentService = new ClassEnrollmentService();
            PendingStudents = new ObservableCollection<ClassEnrollment>();

            ApproveCommand = new RelayCommand<ClassEnrollment>(ExecuteApprove);
            RejectCommand = new RelayCommand<ClassEnrollment>(ExecuteReject);

            LoadPendingStudents();
        }

        private void LoadPendingStudents()
        {
            PendingStudents.Clear();
            var list = _enrollmentService.GetPendingEnrollmentsByClass(_classId);
            if (list != null)
            {
                foreach (var item in list)
                {
                    PendingStudents.Add(item);
                }
            }
        }

        private void ExecuteApprove(ClassEnrollment enrollment)
        {
            if (enrollment != null)
            {
                enrollment.Status = 2; // 2: Đã duyệt
                _enrollmentService.UpdateEnrollment(enrollment);
                LoadPendingStudents(); // Refresh lại danh sách
            }
        }

        private void ExecuteReject(ClassEnrollment enrollment)
        {
            if (enrollment != null)
            {
                var result = MessageBox.Show("Bạn có chắc chắn muốn từ chối học sinh này?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    enrollment.Status = 3; // 3: Từ chối
                    _enrollmentService.UpdateEnrollment(enrollment);
                    LoadPendingStudents(); // Refresh lại danh sách
                }
            }
        }
    }
}