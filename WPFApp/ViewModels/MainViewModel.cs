using BusinessObjects;
using System.Windows;
using System.Windows.Input;
using WPFApp.Commands;
using WPFApp.Views;

namespace WPFApp.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            set { _currentUser = value; OnPropertyChanged(); }
        }

        public string RoleName
        {
            get
            {
                if (_currentUser == null) return "";
                return _currentUser.Role == 0 ? "Administrator" : _currentUser.Role == 1 ? "Giáo viên" : "Học sinh";
            }
        }

        // --- 1. THÊM LOGIC ẨN/HIỆN MENU (Dùng Collapsed để mất hẳn khoảng trắng) ---

        // Hiện menu cho Giáo viên & Admin
        public Visibility TeacherMenuVisibility =>
            (_currentUser != null && (_currentUser.Role == 0 || _currentUser.Role == 1))
            ? Visibility.Visible : Visibility.Collapsed;

        // Hiện menu dành riêng cho Học sinh
        public Visibility StudentMenuVisibility =>
            (_currentUser != null && _currentUser.Role == 2)
            ? Visibility.Visible : Visibility.Collapsed;

        // -----------------------------------------------------------------------

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set { _currentView = value; OnPropertyChanged(); }
        }

        public ICommand LogoutCommand { get; }
        public ICommand ExitCommand { get; }

        public MainViewModel(User user)
        {
            CurrentUser = user;

            LogoutCommand = new RelayCommand<object>(ExecuteLogout);
            ExitCommand = new RelayCommand<object>(ExecuteExit);

            // --- 2. THAY ĐỔI LOGIC NẠP DASHBOARD MẶC ĐỊNH ---
            if (_currentUser.Role == 1) // Nếu là Giáo viên
            {
                CurrentView = new TeacherDashboardViewModel(_currentUser);
            }
            else if (_currentUser.Role == 2) // Nếu là Học sinh
            {
                CurrentView = new StudentDashboardViewModel(_currentUser);
            }
            else // Admin
            {
                CurrentView = null;
            }
        }

        private void ExecuteLogout(object obj)
        {
            var result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginWindow loginWindow = new LoginWindow();
                loginWindow.Show();

                foreach (Window window in Application.Current.Windows)
                {
                    if (window is MainWindow)
                    {
                        window.Close();
                        break;
                    }
                }
            }
        }

        private void ExecuteExit(object obj)
        {
            Application.Current.Shutdown();
        }
    }
}