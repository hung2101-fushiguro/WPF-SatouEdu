using BusinessObjects;
using Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;
using WPFApp.Views;

namespace WPFApp.ViewModels
{
    public class ClassCardModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string StatusColor { get; set; }
        public int StudentCount { get; set; }
        public int MaxStudents { get; set; }
        public double Progress => MaxStudents == 0 ? 0 : (double)StudentCount / MaxStudents * 100;
    }

    public class ReportModel
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public string Status { get; set; }
        public string Score { get; set; }
        public string StatusBgColor { get; set; }
        public string StatusTextColor { get; set; }
    }

    public class TeacherDashboardViewModel : ViewModelBase
    {
        private readonly User _currentTeacher;
        private readonly IClassService _classService;

        public ObservableCollection<ClassCardModel> Classes { get; set; }
        public ObservableCollection<ReportModel> RecentReports { get; set; }

        public ICommand AddClassCommand { get; }
        public ICommand ManageEnrollmentCommand { get; }
        public ICommand EditClassCommand { get; }
        public ICommand DeleteClassCommand { get; }
        public ICommand EnterClassCommand { get; }

        public TeacherDashboardViewModel(User teacher)
        {
            _currentTeacher = teacher;
            _classService = new ClassService();

            Classes = new ObservableCollection<ClassCardModel>();
            RecentReports = new ObservableCollection<ReportModel>();
            EditClassCommand = new RelayCommand<int>(ExecuteEditClass);
            DeleteClassCommand = new RelayCommand<int>(ExecuteDeleteClass);
            EnterClassCommand = new RelayCommand<int>(ExecuteEnterClass);
            AddClassCommand = new RelayCommand<object>(ExecuteAddClass);
            ManageEnrollmentCommand = new RelayCommand<int>(ExecuteManageEnrollment);

            LoadRealClasses();
        }

        private void LoadRealClasses()
        {
            Classes.Clear();

            // 1. Gọi hàm GetClasses() từ Service của bạn để lấy TẤT CẢ các lớp
            var allClasses = _classService.GetClasses();

            if (allClasses != null)
            {
                // 2. Dùng LINQ lọc ra đúng những lớp do Giáo viên này tạo (TeacherId khớp với _currentTeacher.UserId)
                var teacherClasses = allClasses.Where(c => c.TeacherId == _currentTeacher.UserId).ToList();

                // 3. Đổ dữ liệu lên giao diện
                foreach (var c in teacherClasses)
                {
                    Classes.Add(new ClassCardModel
                    {
                        ClassId = c.ClassId,
                        ClassName = c.ClassName,
                        Description = string.IsNullOrWhiteSpace(c.Description) ? "Chưa có mô tả cho lớp học này." : c.Description,
                        Status = "Active",
                        StatusColor = "#33CCFF",
                        // Lấy số lượng học sinh. Nếu ClassEnrollments bị null (do chưa Include trong DB), nó sẽ tạm hiện 0
                        StudentCount = c.ClassEnrollments?.Count(e => e.Status == 2) ?? 0,
                        MaxStudents = 40 // Tạm fix cứng Sĩ số tối đa là 40
                    });
                }
            }
        }

        private void ExecuteAddClass(object obj)
        {
            WPFApp.Views.AddClassWindow addWindow = new WPFApp.Views.AddClassWindow(_currentTeacher.UserId);
            addWindow.ShowDialog();
            LoadRealClasses();
        }
        private void ExecuteManageEnrollment(int classId)
        {
            WPFApp.Views.ManageEnrollmentWindow window = new WPFApp.Views.ManageEnrollmentWindow(classId);
            window.ShowDialog();

            LoadRealClasses();
        }
        private void ExecuteEditClass(int classId)
        {
            // Mở cửa sổ Edit
            WPFApp.Views.EditClassWindow editWindow = new WPFApp.Views.EditClassWindow(classId);
            editWindow.ShowDialog();

            // Tải lại danh sách sau khi sửa xong
            LoadRealClasses();
        }
        private void ExecuteDeleteClass(int classId)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn xóa lớp học này không? Mọi dữ liệu học sinh trong lớp sẽ bị ảnh hưởng!", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var classToDelete = _classService.GetClassById(classId);
                    if (classToDelete != null)
                    {
                        _classService.DeleteClass(classToDelete);
                        MessageBox.Show("Đã xóa lớp học thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadRealClasses(); // Cập nhật lại giao diện
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Không thể xóa lớp do có dữ liệu liên quan: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ExecuteEnterClass(int classId)
        {
            ClassDetailWindow detailWindow = new ClassDetailWindow(classId);
            detailWindow.ShowDialog();
            // Refresh lại Dashboard nếu cần
            LoadRealClasses();
        }
    }
}