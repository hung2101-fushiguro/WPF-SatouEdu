using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;

namespace WPFApp.ViewModels
{
    // Model hiển thị trạng thái của từng học sinh
    public class StudentExamStatusModel
    {
        public string StudentName { get; set; }
        public string StatusText { get; set; }
        public string ScoreText { get; set; }
        public string StatusColor { get; set; }
        public string ScoreColor { get; set; }
    }

    // Model hiển thị thống kê câu sai
    public class QuestionStatModel
    {
        public string QuestionContent { get; set; }
        public int WrongCount { get; set; }
    }

    public class ExamDetailViewModel : ViewModelBase
    {
        private readonly int _examId;

        public Exam ExamInfo { get; set; }
        public int TotalSubmitted { get; set; }
        public int TotalStudents { get; set; }

        public ObservableCollection<StudentExamStatusModel> StudentStatuses { get; set; }
        public ObservableCollection<QuestionStatModel> HardestQuestions { get; set; }

        public ExamDetailViewModel(int examId)
        {
            _examId = examId;
            StudentStatuses = new ObservableCollection<StudentExamStatusModel>();
            HardestQuestions = new ObservableCollection<QuestionStatModel>();

            LoadExamDetails();
        }

        private void LoadExamDetails()
        {
            using var db = new SatouEduDbContext();

            // 1. Lấy thông tin bài thi
            ExamInfo = db.Exams.FirstOrDefault(e => e.ExamId == _examId);
            if (ExamInfo == null) return;
            OnPropertyChanged(nameof(ExamInfo));

            // 2. Lấy danh sách học sinh đã được duyệt vào lớp (Status = 2)
            var enrolledStudents = db.ClassEnrollments
                                     .Include(e => e.Student)
                                     .Where(e => e.ClassId == ExamInfo.ClassId && e.Status == 2)
                                     .ToList();
            TotalStudents = enrolledStudents.Count;

            // 3. Lấy tất cả bài nộp của bài thi này
            var submissions = db.ExamSubmissions
                                .Where(s => s.ExamId == _examId)
                                .ToList();

            TotalSubmitted = submissions.Count(s => s.Status == 3); // 3: Đã nộp
            OnPropertyChanged(nameof(TotalSubmitted));
            OnPropertyChanged(nameof(TotalStudents));

            // 4. Ráp dữ liệu học sinh với bài nộp
            foreach (var enrollment in enrolledStudents)
            {
                var sub = submissions.FirstOrDefault(s => s.StudentId == enrollment.StudentId);

                if (sub == null || sub.Status == 1) // Chưa làm
                {
                    StudentStatuses.Add(new StudentExamStatusModel
                    {
                        StudentName = enrollment.Student.FullName,
                        StatusText = "Chưa làm",
                        StatusColor = "#fca5a5", // Đỏ nhạt
                        ScoreText = "- / 10",
                        ScoreColor = "#9ca3af"
                    });
                }
                else if (sub.Status == 2) // Đang làm
                {
                    StudentStatuses.Add(new StudentExamStatusModel
                    {
                        StudentName = enrollment.Student.FullName,
                        StatusText = "Đang làm",
                        StatusColor = "#fcd34d", // Vàng
                        ScoreText = "Chưa nộp",
                        ScoreColor = "#d97706"
                    });
                }
                else if (sub.Status == 3) // Đã nộp
                {
                    StudentStatuses.Add(new StudentExamStatusModel
                    {
                        StudentName = enrollment.Student.FullName,
                        StatusText = "Đã nộp",
                        StatusColor = "#86efac", // Xanh lá
                        ScoreText = $"{sub.Score:0.0} / 10", // Điểm hệ 10
                        ScoreColor = "#16a34a"
                    });
                }
            }

            // 5. THỐNG KÊ CÂU SAI NHIỀU NHẤT
            // Lọc các chi tiết nộp bài sai (IsCorrect == false) của bài thi này
            var wrongAnswersData = db.SubmissionDetails
                .Include(sd => sd.Submission)
                .Where(sd => sd.Submission.ExamId == _examId && sd.IsCorrect == false)
                .GroupBy(sd => sd.QuestionId)
                .Select(g => new { QuestionId = g.Key, WrongCount = g.Count() })
                .OrderByDescending(x => x.WrongCount)
                .Take(5) // Lấy top 5 câu sai nhiều nhất
                .ToList();

            foreach (var item in wrongAnswersData)
            {
                var question = db.QuestionBanks.FirstOrDefault(q => q.QuestionId == item.QuestionId);
                if (question != null)
                {
                    HardestQuestions.Add(new QuestionStatModel
                    {
                        QuestionContent = question.Content,
                        WrongCount = item.WrongCount
                    });
                }
            }
        }
    }
}