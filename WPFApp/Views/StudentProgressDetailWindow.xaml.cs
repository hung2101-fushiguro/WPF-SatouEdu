using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Windows;

namespace WPFApp.Views
{
    public partial class StudentProgressDetailWindow : Window
    {
        public StudentProgressDetailWindow(int classId, int studentId, string studentName)
        {
            InitializeComponent();
            txtTitle.Text = $"Bảng điểm: {studentName}";

            using var db = new SatouEduDbContext();

            // Lấy danh sách bài thi của lớp
            var exams = db.Exams.Where(e => e.ClassId == classId).ToList();
            var submissions = db.ExamSubmissions.Where(s => s.StudentId == studentId).ToList();

            var progressData = exams.Select(ex => {
                var sub = submissions.FirstOrDefault(s => s.ExamId == ex.ExamId);
                return new
                {
                    ExamName = ex.ExamName,
                    StatusText = sub == null ? "Chưa làm" : (sub.Status == 2 ? "Đang làm" : "Đã nộp"),
                    ScoreText = sub != null && sub.Status == 3 ? $"{sub.Score:0.0}" : "-"
                };
            }).ToList();

            dgProgress.ItemsSource = progressData;
        }
    }
}