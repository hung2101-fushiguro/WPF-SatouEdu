using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel; 
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class AddClassViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService; 
        private readonly int _teacherId;

        public Action CloseAction { get; set; }

        public string ClassName { get; set; }
        public string TargetClassName { get; set; }
        public string Description { get; set; }

        // MỚI: Danh sách môn học và Môn học được người dùng chọn trên giao diện
        public ObservableCollection<Subject> Subjects { get; set; }
        public Subject SelectedSubject { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddClassViewModel(int teacherId)
        {
            _teacherId = teacherId;
            _classService = new ClassService();
            _subjectService = new SubjectService(); // Khởi tạo

            // Lấy toàn bộ môn học từ DB và đổ vào danh sách
            Subjects = new ObservableCollection<Subject>(_subjectService.GetSubjects());

            SaveCommand = new RelayCommand<object>(ExecuteSave);
            CancelCommand = new RelayCommand<object>(ExecuteCancel);
        }

        private void ExecuteSave(object obj)
        {
            if (string.IsNullOrWhiteSpace(ClassName))
            {
                MessageBox.Show("Vui lòng nhập tên lớp học!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Kiểm tra xem giáo viên đã chọn môn học chưa
            if (SelectedSubject == null)
            {
                MessageBox.Show("Vui lòng chọn môn học cho lớp!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                Class newClass = new Class
                {
                    ClassName = this.ClassName.Trim(),
                    TargetClassName = string.IsNullOrWhiteSpace(this.TargetClassName) ? null : this.TargetClassName.Trim().ToUpper(),
                    TeacherId = _teacherId,

                    // Lấy ID động dựa trên môn học giáo viên chọn từ UI
                    SubjectId = SelectedSubject.SubjectId,

                    Description = this.Description,
                    CreatedDate = DateTime.Now
                };

                _classService.SaveClass(newClass);
                MessageBox.Show("Tạo lớp học mới thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);

                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào Database: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel(object obj)
        {
            CloseAction?.Invoke();
        }
    }
}