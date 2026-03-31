using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class AddEditQuestionViewModel : ViewModelBase
    {
        private readonly IQuestionBankService _questionService;
        public Action CloseAction { get; set; }

        private bool _isEditMode;
        private QuestionBank _currentQuestion;

        // Các trường nhập liệu
        public string TitleText => _isEditMode ? "Chỉnh sửa Câu hỏi" : "Thêm Câu hỏi mới";
        public string Content { get; set; }
        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        // Danh sách đáp án để chọn A, B, C, D
        public ObservableCollection<string> CorrectOptionsList { get; set; } = new ObservableCollection<string> { "A", "B", "C", "D" };
        public string SelectedCorrectOption { get; set; }

        public ICommand SaveCommand { get; }

        // CONSTRUCTOR 1: Dành cho THÊM MỚI
        public AddEditQuestionViewModel(int teacherId, int subjectId, int gradeLevel)
        {
            _questionService = new QuestionBankService();
            _isEditMode = false;

            _currentQuestion = new QuestionBank
            {
                TeacherId = teacherId,
                SubjectId = subjectId,
                GradeLevel = gradeLevel
            };

            SelectedCorrectOption = "A"; // Mặc định
            SaveCommand = new RelayCommand<object>(ExecuteSave);
        }

        // CONSTRUCTOR 2: Dành cho CHỈNH SỬA
        public AddEditQuestionViewModel(QuestionBank editQuestion)
        {
            _questionService = new QuestionBankService();
            _isEditMode = true;
            _currentQuestion = editQuestion;

            // Load dữ liệu cũ lên form
            Content = editQuestion.Content;
            OptionA = editQuestion.OptionA;
            OptionB = editQuestion.OptionB;
            OptionC = editQuestion.OptionC;
            OptionD = editQuestion.OptionD;
            SelectedCorrectOption = editQuestion.CorrectOption.ToString().Trim();

            SaveCommand = new RelayCommand<object>(ExecuteSave);
        }

        private void ExecuteSave(object obj)
        {
            if (string.IsNullOrWhiteSpace(Content) || string.IsNullOrWhiteSpace(OptionA) ||
                string.IsNullOrWhiteSpace(OptionB) || string.IsNullOrWhiteSpace(OptionC) ||
                string.IsNullOrWhiteSpace(OptionD))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ nội dung câu hỏi và 4 đáp án!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Gán dữ liệu từ Form vào Object
                _currentQuestion.Content = this.Content;
                _currentQuestion.OptionA = this.OptionA;
                _currentQuestion.OptionB = this.OptionB;
                _currentQuestion.OptionC = this.OptionC;
                _currentQuestion.OptionD = this.OptionD;
                _currentQuestion.CorrectOption = this.SelectedCorrectOption; // Lấy ký tự đầu tiên 'A', 'B'...

                if (_isEditMode)
                {
                    _questionService.UpdateQuestion(_currentQuestion);
                    MessageBox.Show("Cập nhật câu hỏi thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    _questionService.SaveQuestion(_currentQuestion);
                    MessageBox.Show("Thêm câu hỏi mới thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}