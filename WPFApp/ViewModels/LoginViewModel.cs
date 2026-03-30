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
using WPFApp.Views;

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
        public ICommand OpenRegisterCommand { get; }
        public LoginViewModel()
        {
            _userService = new UserService();
            LoginCommand = new RelayCommand<PasswordBox>(ExecuteLogin);
            CloseCommand = new RelayCommand<object>(ExecuteClose);
            OpenRegisterCommand = new RelayCommand<object>(ExecuteOpenRegister);
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
                MainWindow mainWindow = new MainWindow(account);
                mainWindow.Show();

                // Đóng LoginWindow hiện tại
                foreach (Window window in Application.Current.Windows)
                {
                    if (window is LoginWindow)
                    {
                        window.Close();
                        break;
                    }
                }
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
        private void ExecuteOpenRegister(object parameter) 
        {
            RegisterWindow registerWindow = new RegisterWindow();
            registerWindow.Show();
            foreach(Window window in Application.Current.Windows)
            {
                if (window is LoginWindow) 
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
