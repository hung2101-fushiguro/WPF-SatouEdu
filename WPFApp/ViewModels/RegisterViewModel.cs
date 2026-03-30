using BusinessObjects;
using Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApp.Commands;
using WPFApp.Views;

namespace WPFApp.ViewModels
{
    public class RegisterViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; } = DateTime.Now;
        public int SelectedRoleIndex { get; set; } = 0;
        public string ClassName { get; set; }
        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel()
        {
            _userService = new UserService();
            RegisterCommand = new RelayCommand<PasswordBox>(ExecuteRegister);
            BackToLoginCommand = new RelayCommand<object>(ExecuteBack);
        }
        private void ExecuteRegister(PasswordBox passwordBox)
        {
            string password = passwordBox?.Password;
            if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng điền đầy đủ Họ Tên, Email và Mật khẩu!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            int roleDb = SelectedRoleIndex == 0 ? 2 : 1;
            string formattedClassName = null;
            if (roleDb == 2 && !string.IsNullOrWhiteSpace(ClassName))
            {
                formattedClassName = ClassName.Trim().ToUpper();
            }
            try
            {
                var existingUser = _userService.GetUserByEmail(Email);
                if (existingUser != null)
                {
                    MessageBox.Show("Email hoặc username này đã được đăng ký. Vui lòng dùng email hoặc username khác!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                User newUser = new User
                {
                    FullName = this.FullName,
                    Email = this.Email,
                    Password = password,
                    DateOfBirth = this.DateOfBirth.HasValue ? DateOnly.FromDateTime(this.DateOfBirth.Value) : null,
                    Role = roleDb,
                    ClassName = formattedClassName,
                    Status = 1
                };
                _userService.SaveUser(newUser);
                MessageBox.Show($"Đăng ký thành công tài khoản!\nVai trò: {(roleDb == 1 ? "Giáo viên" : $"Học sinh lớp {formattedClassName}")}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                ExecuteBack(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra khi lưu vào Database: " + ex.Message, "Lỗi Server", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteBack(object obj)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            foreach (Window window in Application.Current.Windows)
            {
                if(window is RegisterWindow)
                {
                    window.Close();
                    break;
                }
            }

        }
    }
}
