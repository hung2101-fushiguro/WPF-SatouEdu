using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    // Lớp phụ trợ để hiển thị từng câu hỏi kèm màu sắc Đúng/Sai
    public class QuestionResultItem
    {
        public int OrderIndex { get; set; }
        public string Content { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }
        public string StudentAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }

        // Logic tự động tính màu nền và viền cho các đáp án (Xanh = Đúng, Đỏ = Sai)
        public string BgA => GetBgColor("A"); public string BorderA => GetBorderColor("A");
        public string BgB => GetBgColor("B"); public string BorderB => GetBorderColor("B");
        public string BgC => GetBgColor("C"); public string BorderC => GetBorderColor("C");
        public string BgD => GetBgColor("D"); public string BorderD => GetBorderColor("D");

        private string GetBgColor(string option)
        {
            if (option == CorrectAnswer) return "#dcfce7"; // Xanh lá nhạt
            if (option == StudentAnswer && !IsCorrect) return "#fee2e2"; // Đỏ nhạt
            return "Transparent";
        }

        private string GetBorderColor(string option)
        {
            if (option == CorrectAnswer) return "#22c55e"; // Viền xanh lá
            if (option == StudentAnswer && !IsCorrect) return "#ef4444"; // Viền đỏ
            return "#e1e3e4"; // Xám mặc định
        }
    }

    public class StudentExamResultViewModel : ViewModelBase
    {
        public Action OnClose { get; set; }

        public Exam ExamInfo { get; set; }
        public ExamSubmission Submission { get; set; }

        // Kiểm tra xem giáo viên có cho xem điểm không
        public bool IsScoreVisible { get; set; }
        public bool IsScoreHidden => !IsScoreVisible;

        public int TotalQuestions { get; set; }
        public ObservableCollection<QuestionResultItem> QuestionResults { get; set; }

        public ICommand CloseCommand { get; }

        public StudentExamResultViewModel(int examId, User student)
        {
            QuestionResults = new ObservableCollection<QuestionResultItem>();
            CloseCommand = new RelayCommand<object>(_ => OnClose?.Invoke());

            LoadResult(examId, student.UserId);
        }

        private void LoadResult(int examId, int studentId)
        {
            using var db = new SatouEduDbContext();

            ExamInfo = db.Exams.FirstOrDefault(e => e.ExamId == examId);
            if (ExamInfo == null) return;

            IsScoreVisible = ExamInfo.IsScoreVisible ?? false;

            Submission = db.ExamSubmissions.FirstOrDefault(s => s.ExamId == examId && s.StudentId == studentId);
            if (Submission == null) return;

            var examQuestions = db.ExamQuestions.Include(eq => eq.Question)
                                  .Where(eq => eq.ExamId == examId)
                                  .OrderBy(eq => eq.OrderIndex).ToList();

            TotalQuestions = examQuestions.Count;

            var details = db.SubmissionDetails.Where(sd => sd.SubmissionId == Submission.SubmissionId).ToList();

            foreach (var eq in examQuestions)
            {
                var detail = details.FirstOrDefault(d => d.QuestionId == eq.QuestionId);
                var qb = eq.Question;

                string studentAns = detail?.SelectedOption ?? "";
                string correctAns = qb.CorrectOption.ToString().Trim();

                QuestionResults.Add(new QuestionResultItem
                {
                    OrderIndex = eq.OrderIndex,
                    Content = qb.Content,
                    OptionA = qb.OptionA,
                    OptionB = qb.OptionB,
                    OptionC = qb.OptionC,
                    OptionD = qb.OptionD,
                    StudentAnswer = studentAns,
                    CorrectAnswer = correctAns,
                    IsCorrect = studentAns == correctAns
                });
            }
        }
    }
}