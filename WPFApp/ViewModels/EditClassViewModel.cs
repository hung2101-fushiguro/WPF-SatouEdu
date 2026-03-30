using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class EditClassViewModel : ViewModelBase
    {
        private readonly IClassService _classService;
        private readonly ISubjectService _subjectService;
        private Class _editingClass;

        public Action CloseAction { get; set; }

        public string ClassName { get; set; }
        public string TargetClassName { get; set; }
        public string Description { get; set; }

        public ObservableCollection<Subject> Subjects { get; set; }
        public Subject SelectedSubject { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public EditClassViewModel(int classId)
        {
            _classService = new ClassService();
            _subjectService = new SubjectService();

            Subjects = new ObservableCollection<Subject>(_subjectService.GetSubjects());

            // Tải thông tin lớp học cũ lên
            _editingClass = _classService.GetClassById(classId);
            if (_editingClass != null)
            {
                ClassName = _editingClass.ClassName;
                TargetClassName = _editingClass.TargetClassName;
                Description = _editingClass.Description;
                SelectedSubject = Subjects.FirstOrDefault(s => s.SubjectId == _editingClass.SubjectId);
            }

            SaveCommand = new RelayCommand<object>(ExecuteSave);
            CancelCommand = new RelayCommand<object>(ExecuteCancel);
        }

        private void ExecuteSave(object obj)
        {
            if (string.IsNullOrWhiteSpace(ClassName) || SelectedSubject == null)
            {
                MessageBox.Show("Vui lòng nhập tên lớp và chọn môn học!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                _editingClass.ClassName = this.ClassName.Trim();
                _editingClass.TargetClassName = string.IsNullOrWhiteSpace(this.TargetClassName) ? null : this.TargetClassName.Trim().ToUpper();
                _editingClass.SubjectId = SelectedSubject.SubjectId;
                _editingClass.Description = this.Description;

                // Gọi hàm cập nhật từ Service của bạn (tên hàm UpdateUpdateClass dựa theo file bạn cung cấp trước đó)
                _classService.UpdateUpdateClass(_editingClass);

                MessageBox.Show("Cập nhật lớp học thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                CloseAction?.Invoke();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExecuteCancel(object obj) => CloseAction?.Invoke();
    }
}