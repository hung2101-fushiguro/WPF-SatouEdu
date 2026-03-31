using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WPFApp.Commands;
using WPFApp.Views;

namespace WPFApp.ViewModels
{
    // Model cho danh sách học sinh
    public class StudentProgressModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public int TotalExamsDone { get; set; }
        public double AverageScore { get; set; }
    }

    public class ClassDetailViewModel : ViewModelBase
    {
        private readonly int _classId;

        public Class ClassInfo { get; set; }
        public ObservableCollection<Exam> ExamsList { get; set; }
        public ObservableCollection<StudentProgressModel> StudentsList { get; set; }

        public ICommand CreateExamCommand { get; }
        public ICommand ViewExamCommand { get; }
        public ICommand ViewStudentProgressCommand { get; }

        public ClassDetailViewModel(int classId)
        {
            _classId = classId;
            ExamsList = new ObservableCollection<Exam>();
            StudentsList = new ObservableCollection<StudentProgressModel>();

            // Gắn lệnh mở cửa sổ Ra Bài Tập mà chúng ta đã làm ở bước trước
            CreateExamCommand = new RelayCommand<object>(obj =>
            {
                new CreateExamWindow(_classId).ShowDialog();
                LoadData(); // Nạp lại danh sách bài tập sau khi tạo xong
            });
            ViewExamCommand = new RelayCommand<int>(ExecuteViewExam);
            ViewStudentProgressCommand = new RelayCommand<int>(ExecuteViewStudentProgress);

            LoadData();
        }

        private void LoadData()
        {
            ExamsList.Clear();
            StudentsList.Clear();

            // SỬ DỤNG DB CONTEXT ĐỂ TRUY VẤN LINQ PHỨC TẠP
            using var db = new SatouEduDbContext();

            // 1. Lấy thông tin lớp
            ClassInfo = db.Classes.FirstOrDefault(c => c.ClassId == _classId);
            OnPropertyChanged(nameof(ClassInfo));

            // 2. Lấy danh sách Bài tập / Kiểm tra của lớp này
            var exams = db.Exams.Where(e => e.ClassId == _classId).OrderByDescending(e => e.StartTime).ToList();
            foreach (var ex in exams) ExamsList.Add(ex);

            // 3. Lấy danh sách Học sinh (Đã duyệt: Status = 2) và tính điểm
            var enrolledStudents = db.ClassEnrollments
                                     .Include(e => e.Student)
                                     .Where(e => e.ClassId == _classId && e.Status == 2)
                                     .ToList();

            foreach (var enrollment in enrolledStudents)
            {
                // Lấy tất cả bài đã nộp của học sinh này trong lớp hiện tại (Status = 3 là Đã nộp)
                var submissions = db.ExamSubmissions
                                    .Include(s => s.Exam)
                                    .Where(s => s.StudentId == enrollment.StudentId && s.Exam.ClassId == _classId && s.Status == 3)
                                    .ToList();

                int doneCount = submissions.Count;
                // Tính trung bình cộng cột Score, nếu chưa làm bài nào thì bằng 0
                double avgScore = doneCount > 0 ? (double)submissions.Average(s => s.Score) : 0;

                StudentsList.Add(new StudentProgressModel
                {
                    StudentId = enrollment.StudentId,
                    FullName = enrollment.Student?.FullName ?? "Unknown",
                    TotalExamsDone = doneCount,
                    AverageScore = avgScore
                });
            }
        }
        private void ExecuteViewExam(int examId)
        {
            ExamDetailWindow window = new ExamDetailWindow(examId);
            window.ShowDialog();
        }
        private void ExecuteViewStudentProgress(int studentId)
        {
            // Lấy tên học sinh để hiển thị lên tiêu đề
            var student = StudentsList.FirstOrDefault(s => s.StudentId == studentId);
            string studentName = student != null ? student.FullName : "Học sinh";

            // Sẽ tạo cửa sổ này ở bước dưới
            StudentProgressDetailWindow window = new StudentProgressDetailWindow(_classId, studentId, studentName);
            window.ShowDialog();
        }
    }
}