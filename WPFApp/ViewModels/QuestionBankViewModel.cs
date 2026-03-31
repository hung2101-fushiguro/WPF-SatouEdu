using BusinessObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;
using WPFApp.Views;

namespace WPFApp.ViewModels
{
    public class QuestionBankViewModel : ViewModelBase
    {
        private readonly IQuestionBankService _questionService;
        private readonly ISubjectService _subjectService;
        private readonly User _teacher;

        public ObservableCollection<QuestionBank> QuestionsList { get; set; }
        public ObservableCollection<Subject> Subjects { get; set; }

        public List<int> AvailableGrades { get; set; } = new List<int> { 10, 11, 12 };

        private Subject _selectedSubject;
        public Subject SelectedSubject
        {
            get => _selectedSubject;
            set
            {
                _selectedSubject = value;
                OnPropertyChanged(nameof(SelectedSubject));
                LoadQuestions();
            }
        }

        private int _selectedGradeLevel;
        public int SelectedGradeLevel
        {
            get => _selectedGradeLevel;
            set
            {
                _selectedGradeLevel = value;
                OnPropertyChanged(nameof(SelectedGradeLevel));
                LoadQuestions();
            }
        }

        // --- MỚI: CÁC LỆNH THÊM, SỬA, XÓA ---
        public ICommand AddQuestionCommand { get; }
        public ICommand EditQuestionCommand { get; }
        public ICommand DeleteQuestionCommand { get; }

        public QuestionBankViewModel(User teacher)
        {
            _teacher = teacher;
            _questionService = new QuestionBankService();
            _subjectService = new SubjectService();

            QuestionsList = new ObservableCollection<QuestionBank>();
            Subjects = new ObservableCollection<Subject>();

            // Khởi tạo các Command
            AddQuestionCommand = new RelayCommand<object>(ExecuteAddQuestion);
            EditQuestionCommand = new RelayCommand<QuestionBank>(ExecuteEditQuestion);
            DeleteQuestionCommand = new RelayCommand<QuestionBank>(ExecuteDeleteQuestion);

            var allSubjects = _subjectService.GetSubjects();
            if (allSubjects != null)
            {
                foreach (var s in allSubjects) Subjects.Add(s);
            }

            if (Subjects.Count > 0)
            {
                _selectedSubject = Subjects.First();
            }
            _selectedGradeLevel = 10;

            LoadQuestions();
        }

        private void LoadQuestions()
        {
            QuestionsList.Clear();
            if (SelectedSubject == null) return;

            var list = _questionService.GetQuestions(_teacher.UserId, SelectedSubject.SubjectId, SelectedGradeLevel);

            if (list != null)
            {
                foreach (var q in list)
                {
                    QuestionsList.Add(q);
                }
            }
        }

        // --- CÁC HÀM THỰC THI THÊM, SỬA, XÓA ---
        private void ExecuteAddQuestion(object obj)
        {
            if (SelectedSubject == null)
            {
                MessageBox.Show("Vui lòng chọn môn học trước khi thêm câu hỏi!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Mở cửa sổ Thêm câu hỏi (Bạn sẽ cần tạo file AddQuestionWindow.xaml sau)
            MessageBox.Show($"Mở cửa sổ thêm câu hỏi cho môn {SelectedSubject.SubjectName} - Lớp {SelectedGradeLevel}");
            AddEditQuestionWindow addWindow = new AddEditQuestionWindow(_teacher.UserId, SelectedSubject.SubjectId, SelectedGradeLevel);
            addWindow.ShowDialog();
            LoadQuestions();
        }

        private void ExecuteEditQuestion(QuestionBank questionToEdit)
        {
            if (questionToEdit == null) return;

            // Mở cửa sổ Sửa câu hỏi
            MessageBox.Show($"Mở cửa sổ sửa câu hỏi ID: {questionToEdit.QuestionId}");
            AddEditQuestionWindow editWindow = new AddEditQuestionWindow(questionToEdit);
            editWindow.ShowDialog();
            LoadQuestions();
        }

        private void ExecuteDeleteQuestion(QuestionBank questionToDelete)
        {
            if (questionToDelete == null) return;

            var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa câu hỏi:\n\"{questionToDelete.Content}\" không?",
                                         "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _questionService.DeleteQuestion(questionToDelete);
                    MessageBox.Show("Đã xóa câu hỏi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadQuestions(); // Tải lại danh sách
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa câu hỏi do lỗi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}