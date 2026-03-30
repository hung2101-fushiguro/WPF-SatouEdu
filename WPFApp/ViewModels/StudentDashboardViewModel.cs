using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class StudentClassCardModel
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string Description { get; set; }

        // MỚI: Các thuộc tính giao diện "động" cho nút bấm
        public int EnrollmentStatus { get; set; }
        public string ButtonText { get; set; }
        public string ButtonIcon { get; set; }
        public string ButtonBgColor { get; set; }
        public string ButtonTextColor { get; set; }
    }

    public class StudentDashboardViewModel : ViewModelBase
    {
        private readonly User _currentStudent;
        private readonly IClassService _classService;
        private readonly IClassEnrollmentService _enrollmentService;

        public ObservableCollection<StudentClassCardModel> AvailableClasses { get; set; }

        public ICommand JoinClassCommand { get; }

        public StudentDashboardViewModel(User student)
        {
            _currentStudent = student;
            _classService = new ClassService();
            _enrollmentService = new ClassEnrollmentService();

            AvailableClasses = new ObservableCollection<StudentClassCardModel>();
            JoinClassCommand = new RelayCommand<object>(ExecuteJoinClass);

            LoadAvailableClasses();
        }

        private void LoadAvailableClasses()
        {
            AvailableClasses.Clear();
            var allClasses = _classService.GetClasses();

            if (allClasses != null)
            {
                var validClasses = allClasses.Where(c =>
                    string.IsNullOrWhiteSpace(c.TargetClassName) ||
                    c.TargetClassName.Equals(_currentStudent.ClassName, StringComparison.OrdinalIgnoreCase)
                ).ToList();

                foreach (var c in validClasses)
                {
                    // TÌM TRẠNG THÁI CỦA HỌC SINH NÀY TRONG LỚP
                    var myEnrollment = c.ClassEnrollments?.FirstOrDefault(e => e.StudentId == _currentStudent.UserId);
                    int status = myEnrollment?.Status ?? 0;

                    // Mặc định: Chưa tham gia
                    string btnText = "Xin gia nhập";
                    string btnIcon = "\xE8FA";
                    string btnBg = "#e0f2fe";
                    string btnTextCol = "#0284c7";

                    // Cập nhật giao diện nút theo trạng thái
                    if (status == 1) // Chờ duyệt
                    {
                        btnText = "Đang chờ duyệt";
                        btnIcon = "\xE81C";
                        btnBg = "#fef3c7";
                        btnTextCol = "#d97706";
                    }
                    else if (status == 2) // Đã duyệt
                    {
                        btnText = "Vào lớp học";
                        btnIcon = "\xE8A5";
                        btnBg = "#dcfce7";
                        btnTextCol = "#16a34a";
                    }
                    else if (status == 3) // Từ chối
                    {
                        btnText = "Bị từ chối";
                        btnIcon = "\xE711";
                        btnBg = "#fee2e2";
                        btnTextCol = "#dc2626";
                    }

                    AvailableClasses.Add(new StudentClassCardModel
                    {
                        ClassId = c.ClassId,
                        ClassName = c.ClassName,
                        TeacherName = c.Teacher != null ? c.Teacher.FullName : "Giáo viên bộ môn",
                        Description = string.IsNullOrWhiteSpace(c.Description) ? "Lớp học này chưa có mô tả." : c.Description,

                        // Gán các giá trị động vào Card
                        EnrollmentStatus = status,
                        ButtonText = btnText,
                        ButtonIcon = btnIcon,
                        ButtonBgColor = btnBg,
                        ButtonTextColor = btnTextCol
                    });
                }
            }
        }

        private void ExecuteJoinClass(object obj)
        {
            if (obj is int classId)
            {
                var targetClass = AvailableClasses.FirstOrDefault(x => x.ClassId == classId);
                if (targetClass == null) return;

                // Xử lý hành động dựa trên trạng thái nút bấm hiện tại
                if (targetClass.EnrollmentStatus == 2)
                {
                    MessageBox.Show("Mở màn hình chi tiết lớp học (Bài giảng, Bài tập...) ở các bước sau!", "Vào lớp", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                else if (targetClass.EnrollmentStatus == 1)
                {
                    MessageBox.Show("Yêu cầu của bạn vẫn đang chờ giáo viên duyệt, vui lòng kiên nhẫn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else if (targetClass.EnrollmentStatus == 3)
                {
                    MessageBox.Show("Rất tiếc, yêu cầu gia nhập của bạn đã bị từ chối.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Nếu EnrollmentStatus == 0 (Chưa tham gia) thì mới lưu xuống DB
                try
                {
                    ClassEnrollment newEnrollment = new ClassEnrollment
                    {
                        ClassId = classId,
                        StudentId = _currentStudent.UserId,
                        EnrollmentDate = DateTime.Now,
                        Status = 1 // 1: Chờ duyệt
                    };

                    _enrollmentService.SaveEnrollment(newEnrollment);

                    // Nạp lại danh sách để Nút bấm lập tức "biến hình" sang trạng thái Chờ duyệt
                    LoadAvailableClasses();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi kết nối Database: " + ex.Message, "Lỗi Server", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}