using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class StudentExamItemModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public int DurationMinutes { get; set; }
        public string EndTimeText { get; set; }
        public string StatusText { get; set; }
        public string StatusColor { get; set; }
        public string ActionButtonText { get; set; }
        public string ActionButtonColor { get; set; }
    }

    public class StudentClassDetailViewModel : ViewModelBase
    {
        private readonly int _classId;
        private readonly User _currentStudent;

        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string Description { get; set; }

        public ObservableCollection<StudentExamItemModel> ExamsList { get; set; }

        public ICommand OpenExamCommand { get; }

        // THÊM CÁC BIẾN NÀY VÀO TRONG CLASS StudentClassDetailViewModel:
        private Visibility _listVisibility = Visibility.Visible;
        public Visibility ListVisibility
        {
            get => _listVisibility;
            set
            {
                _listVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _examVisibility = Visibility.Collapsed;
        public Visibility ExamVisibility
        {
            get => _examVisibility;
            set
            {
                _examVisibility = value;
                OnPropertyChanged();
            }
        }

        private ViewModelBase _activeExamVM;
        public ViewModelBase ActiveExamVM
        {
            get => _activeExamVM;
            set
            {
                _activeExamVM = value;
                OnPropertyChanged();
            }
        }

        public StudentClassDetailViewModel(int classId, User student)
        {
            _classId = classId;
            _currentStudent = student;
            ExamsList = new ObservableCollection<StudentExamItemModel>();

            OpenExamCommand = new RelayCommand<int>(ExecuteOpenExam);

            LoadClassAndExams();
        }

        private void LoadClassAndExams()
        {
            using var db = new SatouEduDbContext();

            // 1. Lấy thông tin lớp và giáo viên
            var classInfo = db.Classes.Include(c => c.Teacher).FirstOrDefault(c => c.ClassId == _classId);
            if (classInfo != null)
            {
                ClassName = classInfo.ClassName;
                TeacherName = classInfo.Teacher != null ? $"Giáo viên: {classInfo.Teacher.FullName}" : "Giáo viên bộ môn";
                Description = classInfo.Description;
            }

            // 2. Lấy danh sách bài tập đã xuất bản (Status = 2)
            var exams = db.Exams.Where(e => e.ClassId == _classId && e.Status == 2).OrderByDescending(e => e.StartTime).ToList();

            // 3. Lấy lịch sử nộp bài của học sinh này
            var submissions = db.ExamSubmissions.Where(s => s.StudentId == _currentStudent.UserId).ToList();

            ExamsList.Clear();

            foreach (var exam in exams)
            {
                var sub = submissions.FirstOrDefault(s => s.ExamId == exam.ExamId);

                string statusTxt = "Chưa làm";
                string statusCol = "#fca5a5"; // Đỏ nhạt
                string btnTxt = "Vào làm bài";
                string btnCol = "#006683"; // Primary color từ UI của bạn

                if (sub != null)
                {
                    if (sub.Status == 2)
                    {
                        statusTxt = "Đang làm";
                        statusCol = "#fcd34d"; // Vàng
                        btnTxt = "Tiếp tục làm";
                        btnCol = "#d97706";
                    }
                    else if (sub.Status == 3)
                    {
                        statusTxt = "Đã nộp";
                        statusCol = "#86efac"; // Xanh lá
                        btnTxt = "Xem kết quả";
                        btnCol = "#16a34a";
                    }
                }

                ExamsList.Add(new StudentExamItemModel
                {
                    ExamId = exam.ExamId,
                    ExamName = exam.ExamName,
                    DurationMinutes = exam.DurationMinutes,
                    EndTimeText = exam.EndTime.ToString("dd/MM/yyyy HH:mm"),
                    StatusText = statusTxt,
                    StatusColor = statusCol,
                    ActionButtonText = btnTxt,
                    ActionButtonColor = btnCol
                });
            }
        }
        private void ExecuteOpenExam(int examId)
        {
            using var db = new SatouEduDbContext();
            // Kiểm tra xem học sinh đã nộp bài chưa
            var sub = db.ExamSubmissions.FirstOrDefault(s => s.ExamId == examId && s.StudentId == _currentStudent.UserId);

            if (sub != null && sub.Status == 3) // TRẠNG THÁI = 3 (ĐÃ NỘP) -> MỞ MÀN HÌNH XEM KẾT QUẢ
            {
                var resultVM = new StudentExamResultViewModel(examId, _currentStudent);
                resultVM.OnClose = () =>
                {
                    ExamVisibility = Visibility.Collapsed; // Đóng sân khấu
                    ListVisibility = Visibility.Visible;   // Mở lại danh sách
                    ActiveExamVM = null;
                };
                ActiveExamVM = resultVM;
            }
            else // CHƯA LÀM HOẶC ĐANG LÀM DỞ -> MỞ MÀN HÌNH LÀM BÀI THI
            {
                var examVM = new StudentExamTakingViewModel(examId, _currentStudent);
                examVM.OnSubmitComplete = () =>
                {
                    ExamVisibility = Visibility.Collapsed; // Đóng sân khấu
                    ListVisibility = Visibility.Visible;   // Mở lại danh sách
                    ActiveExamVM = null;
                    LoadClassAndExams(); // Tải lại danh sách để chữ đổi thành "Xem kết quả"
                };
                ActiveExamVM = examVM;
            }

            // Trượt đổi giao diện
            ListVisibility = Visibility.Collapsed;
            ExamVisibility = Visibility.Visible;
        }
    }
}