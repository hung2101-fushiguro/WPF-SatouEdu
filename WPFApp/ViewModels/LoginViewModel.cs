using BusinessObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFApp.Commands;

namespace WPFApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }
        public ICommand LoginCommand { get; }
        public ICommand CloseCommand { get; }
        public LoginViewModel()
        {
            _userService = new UserService();
            LoginCommand = new RelayCommand<PasswordBox>(ExecuteLogin);
            CloseCommand = new RelayCommand<object>(ExecuteClose);
        }
        private void ExecuteLogin(PasswordBox passwordBox)
        {
            string password = passwordBox?.Password;
            if (string.IsNullOrEmpty(Email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Vui lòng nhập Email và Mật khẩu!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            User account = _userService.GetUserByEmail(Email);
            if (account != null && account.Password == password)
            {
                if (account.Status == 2)
                {
                    MessageBox.Show("Tài khoản của bạn đã bị khóa! Vui lòng liên hệ Admin.", "Lỗi truy cập", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                MessageBox.Show($"Đăng nhập thành công!\nXin chào: {account.FullName}\nVai trò: {(account.Role == 0 ? "Admin" : account.Role == 1 ? "Giáo viên" : "Học sinh")}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else 
            {
                MessageBox.Show("Email hoặc Mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteClose(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
