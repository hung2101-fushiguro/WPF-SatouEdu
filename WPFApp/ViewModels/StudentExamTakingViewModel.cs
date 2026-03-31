using BusinessObjects;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    // Model cho 1 câu hỏi trong đề thi
    public class ExamQuestionItem : ViewModelBase
    {
        public int QuestionId { get; set; }
        public int OrderIndex { get; set; }
        public string Content { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        private string _selectedOption;
        public string SelectedOption
        {
            get => _selectedOption;
            set
            {
                _selectedOption = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAnswered));
                OnPropertyChanged(nameof(MapColor));
            }
        }

        public bool IsAnswered => !string.IsNullOrEmpty(SelectedOption);

        // Màu sắc cho bản đồ câu hỏi (Xanh lá nếu đã làm, Xám nếu chưa làm)
        public string MapColor => IsAnswered ? "#66FF33" : "#f3f4f5";
        public string MapTextColor => IsAnswered ? "#042100" : "#5f5e5e";
    }

    public class StudentExamTakingViewModel : ViewModelBase
    {
        private readonly int _examId;
        private readonly User _student;
        private DispatcherTimer _timer;
        private int _timeRemainingSeconds;

        public Action OnSubmitComplete { get; set; }

        public Exam ExamInfo { get; set; }
        public string TimeRemainingText { get; set; }
        public double ProgressPercentage { get; set; }

        public ObservableCollection<ExamQuestionItem> Questions { get; set; }

        private ExamQuestionItem _currentQuestion;
        public ExamQuestionItem CurrentQuestion
        {
            get => _currentQuestion;
            set { _currentQuestion = value; OnPropertyChanged(); }
        }

        public int CurrentQuestionIndex { get; set; } = 1;

        public ICommand NextCommand { get; }
        public ICommand PrevCommand { get; }
        public ICommand SelectQuestionCommand { get; }
        public ICommand SelectAnswerCommand { get; }
        public ICommand SubmitCommand { get; }

        public StudentExamTakingViewModel(int examId, User student)
        {
            _examId = examId;
            _student = student;
            Questions = new ObservableCollection<ExamQuestionItem>();

            NextCommand = new RelayCommand<object>(_ => ChangeQuestion(1), _ => CurrentQuestionIndex < Questions.Count);
            PrevCommand = new RelayCommand<object>(_ => ChangeQuestion(-1), _ => CurrentQuestionIndex > 1);
            SelectQuestionCommand = new RelayCommand<int>(idx => { CurrentQuestionIndex = idx; UpdateCurrentQuestion(); });

            // Lệnh khi học sinh chọn đáp án A, B, C, D
            SelectAnswerCommand = new RelayCommand<string>(ans => {
                if (CurrentQuestion != null) CurrentQuestion.SelectedOption = ans;
            });

            SubmitCommand = new RelayCommand<object>(ExecuteSubmit);

            LoadExamData();
            StartTimer();
        }

        private void LoadExamData()
        {
            using var db = new SatouEduDbContext();
            ExamInfo = db.Exams.FirstOrDefault(e => e.ExamId == _examId);

            // Lấy danh sách câu hỏi của bài thi này
            var examQuestions = db.ExamQuestions
                                  .Include(eq => eq.Question)
                                  .Where(eq => eq.ExamId == _examId)
                                  .OrderBy(eq => eq.OrderIndex)
                                  .ToList();

            foreach (var eq in examQuestions)
            {
                Questions.Add(new ExamQuestionItem
                {
                    QuestionId = eq.QuestionId,
                    OrderIndex = eq.OrderIndex,
                    Content = eq.Question.Content,
                    OptionA = eq.Question.OptionA,
                    OptionB = eq.Question.OptionB,
                    OptionC = eq.Question.OptionC,
                    OptionD = eq.Question.OptionD
                });
            }

            if (Questions.Any())
            {
                CurrentQuestion = Questions.First();
            }

            _timeRemainingSeconds = ExamInfo.DurationMinutes * 60;
            UpdateTimeDisplay();

            // Tạo bản ghi đang làm bài (Status = 2)
            var submission = db.ExamSubmissions.FirstOrDefault(s => s.ExamId == _examId && s.StudentId == _student.UserId);
            if (submission == null)
            {
                db.ExamSubmissions.Add(new ExamSubmission
                {
                    ExamId = _examId,
                    StudentId = _student.UserId,
                    Status = 2, // Đang làm
                    SubmitTime = DateTime.Now
                });
                db.SaveChanges();
            }
        }

        private void ChangeQuestion(int step)
        {
            CurrentQuestionIndex += step;
            UpdateCurrentQuestion();
        }

        private void UpdateCurrentQuestion()
        {
            CurrentQuestion = Questions[CurrentQuestionIndex - 1];
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) =>
            {
                if (_timeRemainingSeconds > 0)
                {
                    _timeRemainingSeconds--;
                    UpdateTimeDisplay();
                }
                else
                {
                    _timer.Stop();
                    MessageBox.Show("Đã hết thời gian làm bài. Hệ thống sẽ tự động nộp bài!", "Hết giờ", MessageBoxButton.OK, MessageBoxImage.Information);
                    ExecuteSubmit(null);
                }
            };
            _timer.Start();
        }

        private void UpdateTimeDisplay()
        {
            TimeSpan time = TimeSpan.FromSeconds(_timeRemainingSeconds);
            TimeRemainingText = time.ToString(@"mm\:ss");
            ProgressPercentage = ((double)_timeRemainingSeconds / (ExamInfo.DurationMinutes * 60)) * 100;
            OnPropertyChanged(nameof(TimeRemainingText));
            OnPropertyChanged(nameof(ProgressPercentage));
        }

        private void ExecuteSubmit(object obj)
        {
            if (obj != null && Questions.Any(q => !q.IsAnswered))
            {
                var result = MessageBox.Show("Bạn vẫn còn câu hỏi chưa trả lời. Bạn có chắc chắn muốn nộp bài không?", "Cảnh báo", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No) return;
            }

            _timer.Stop();

            using var db = new SatouEduDbContext();
            var submission = db.ExamSubmissions.FirstOrDefault(s => s.ExamId == _examId && s.StudentId == _student.UserId);

            if (submission != null)
            {
                int correctCount = 0;
                // Chấm điểm từng câu
                foreach (var q in Questions)
                {
                    var trueQuestion = db.QuestionBanks.FirstOrDefault(qb => qb.QuestionId == q.QuestionId);
                    bool isCorrect = trueQuestion != null && trueQuestion.CorrectOption.ToString() == q.SelectedOption;

                    if (isCorrect) correctCount++;

                    // Lưu chi tiết câu trả lời
                    var detail = db.SubmissionDetails.FirstOrDefault(sd => sd.SubmissionId == submission.SubmissionId && sd.QuestionId == q.QuestionId);
                    if (detail == null)
                    {
                        db.SubmissionDetails.Add(new SubmissionDetail
                        {
                            SubmissionId = submission.SubmissionId,
                            QuestionId = q.QuestionId,
                            SelectedOption = q.SelectedOption,
                            IsCorrect = isCorrect
                        });
                    }
                    else
                    {
                        detail.SelectedOption = q.SelectedOption;
                        detail.IsCorrect = isCorrect;
                    }
                }

                // Tính điểm hệ 10
                submission.TotalCorrect = correctCount;
                submission.Score = Questions.Count > 0 ? (decimal)correctCount / Questions.Count * 10 : 0;
                submission.Status = 3; // Đã nộp
                submission.SubmitTime = DateTime.Now;

                db.SaveChanges();
            }

            MessageBox.Show("Nộp bài thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            OnSubmitComplete?.Invoke(); // Báo cho màn hình ClassDetail biết để đóng giao diện thi lại
        }
    }
}