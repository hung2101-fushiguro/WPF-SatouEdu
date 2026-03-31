using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFApp.ViewModels;
using BusinessObjects;

namespace WPFApp.Views
{
    /// <summary>
    /// Interaction logic for StudentClassDetailWindow.xaml
    /// </summary>
    public partial class StudentClassDetailWindow : Window
    {
        public StudentClassDetailWindow(int classId, User student)
        {
            InitializeComponent();
            this.DataContext = new StudentClassDetailViewModel(classId, student);
        }
    }
}
