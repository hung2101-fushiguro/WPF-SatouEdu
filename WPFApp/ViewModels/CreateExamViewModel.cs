using BusinessObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class QuestionSelectModel : ViewModelBase
    {
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public int GradeLevel { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }

    public class CreateExamViewModel : ViewModelBase
    {
        private readonly IExamService _examService;
        private readonly IQuestionBankService _questionBankService;
        private readonly IExamQuestionService _examQuestionService;
        private readonly IClassService _classService;

        private readonly int _classId;
        private int _teacherId;
        private int _subjectId;

        public Action CloseAction { get; set; }

        // Thông tin bài thi 
        public string ExamName { get; set; }
        public int ExamType { get; set; } = 1;
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(3);
        public int DurationMinutes { get; set; } = 45;
        public bool IsScoreVisible { get; set; } = true;

        // MỚI: Danh sách Khối lớp để giáo viên lọc câu hỏi
        public List<int> AvailableGrades { get; set; } = new List<int> { 10, 11, 12 };

        private int _selectedGradeLevel;
        public int SelectedGradeLevel
        {
            get => _selectedGradeLevel;
            set
            {
                _selectedGradeLevel = value;
                OnPropertyChanged(nameof(SelectedGradeLevel));
                LoadQuestionsForThisClass();
            }
        }

        public ObservableCollection<QuestionSelectModel> AvailableQuestions { get; set; }

        public ICommand SaveExamCommand { get; }

        // 1. THÊM CÁC BIẾN NÀY VÀO DƯỚI CÙNG DANH SÁCH THUỘC TÍNH
        private int _randomQuestionCount;
        public int RandomQuestionCount
        {
            get => _randomQuestionCount;
            set
            {
                _randomQuestionCount = value;
                OnPropertyChanged(nameof(RandomQuestionCount));
            }
        }

        public ICommand RandomSelectCommand { get; }

        public CreateExamViewModel(int classId)
        {
            _classId = classId;
            _examService = new ExamService();
            _questionBankService = new QuestionBankService();
            _examQuestionService = new ExamQuestionService();
            _classService = new ClassService();

            AvailableQuestions = new ObservableCollection<QuestionSelectModel>();
            SaveExamCommand = new RelayCommand<object>(ExecuteSaveExam);

            // 2. TRONG CONSTRUCTOR, KHỞI TẠO LỆNH:
            RandomSelectCommand = new RelayCommand<object>(ExecuteRandomSelect);

            // Khởi tạo thông tin cơ bản của lớp
            var currentClass = _classService.GetClassById(_classId);
            if (currentClass != null)
            {
                _teacherId = currentClass.TeacherId;
                _subjectId = currentClass.SubjectId;

                // Cố gắng đoán khối lớp dựa trên tên lớp (Ví dụ "Toán 10A1" -> Lớp 10)
                _selectedGradeLevel = ExtractGradeFromClassName(currentClass.ClassName);
            }

            // Gọi lần đầu để load danh sách
            LoadQuestionsForThisClass();
        }

        // Hàm phụ trợ: Tự nhận diện lớp 10, 11 hay 12 từ tên lớp
        private int ExtractGradeFromClassName(string className)
        {
            if (string.IsNullOrEmpty(className)) return 10;
            if (className.Contains("12")) return 12;
            if (className.Contains("11")) return 11;
            return 10; // Mặc định
        }

        private void LoadQuestionsForThisClass()
        {
            AvailableQuestions.Clear();

            // GỌI HÀM THEO ĐÚNG LOGIC TRONG DAO CỦA BẠN: (teacherId, subjectId, gradeLevel)
            // Lưu ý: Đảm bảo IQuestionBankService của bạn cũng có hàm GetQuestions khớp với tham số này
            var subjectQuestions = _questionBankService.GetQuestions(_teacherId, _subjectId, SelectedGradeLevel);

            if (subjectQuestions != null)
            {
                foreach (var q in subjectQuestions)
                {
                    AvailableQuestions.Add(new QuestionSelectModel
                    {
                        QuestionId = q.QuestionId,
                        Content = q.Content,
                        GradeLevel = q.GradeLevel,
                        IsSelected = false
                    });
                }
            }
        }

        private void ExecuteSaveExam(object obj)
        {
            if (string.IsNullOrWhiteSpace(ExamName))
            {
                MessageBox.Show("Vui lòng nhập tên bài kiểm tra/bài tập!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedQuestions = AvailableQuestions.Where(q => q.IsSelected).ToList();
            if (!selectedQuestions.Any())
            {
                MessageBox.Show("Vui lòng chọn ít nhất 1 câu hỏi từ Ngân hàng!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Exam newExam = new Exam
                {
                    ClassId = _classId,
                    ExamName = this.ExamName,
                    ExamType = this.ExamType,
                    StartTime = this.StartDate,
                    EndTime = this.EndDate.AddHours(23).AddMinutes(59),
                    DurationMinutes = this.DurationMinutes,
                    IsScoreVisible = this.IsScoreVisible,
                    Status = 2
                };

                _examService.SaveExam(newExam);

                int order = 1;
                foreach (var q in selectedQuestions)
                {
                    ExamQuestion eq = new ExamQuestion
                    {
                        ExamId = newExam.ExamId,
                        QuestionId = q.QuestionId,
                        OrderIndex = order++
                    };
                    _examQuestionService.SaveExamQuestion(eq);
                }

                MessageBox.Show($"Tạo bài thành công với {selectedQuestions.Count} câu hỏi!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu Database: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 3. THÊM HÀM THỰC THI NÀY XUỐNG DƯỚI CÙNG CỦA CLASS
        private void ExecuteRandomSelect(object obj)
        {
            // Bỏ chọn tất cả các câu trước khi random mới
            foreach (var q in AvailableQuestions)
            {
                q.IsSelected = false;
            }

            if (RandomQuestionCount <= 0)
            {
                MessageBox.Show("Vui lòng nhập số lượng câu hỏi lớn hơn 0!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (RandomQuestionCount > AvailableQuestions.Count)
            {
                MessageBox.Show($"Ngân hàng hiện chỉ có {AvailableQuestions.Count} câu. Không thể random {RandomQuestionCount} câu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                RandomQuestionCount = AvailableQuestions.Count; // Tự sửa lại bằng số max
            }

            // Thuật toán xáo trộn (Shuffle) và lấy ra N câu
            var random = new Random();
            var randomQuestions = AvailableQuestions.OrderBy(x => random.Next()).Take(RandomQuestionCount).ToList();

            // Tự động Tick chọn các câu vừa được random
            foreach (var q in randomQuestions)
            {
                q.IsSelected = true;
            }
        }
    }
}