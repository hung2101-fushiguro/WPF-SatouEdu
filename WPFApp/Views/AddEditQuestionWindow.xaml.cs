using BusinessObjects;
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

namespace WPFApp.Views
{
    /// <summary>
    /// Interaction logic for AddEditQuestionWindow.xaml
    /// </summary>
    public partial class AddEditQuestionWindow : Window
    {
        // Constructor cho Thêm mới
        public AddEditQuestionWindow(int teacherId, int subjectId, int gradeLevel)
        {
            InitializeComponent();
            var vm = new AddEditQuestionViewModel(teacherId, subjectId, gradeLevel);
            vm.CloseAction = () => this.Close();
            this.DataContext = vm;
        }

        // Constructor cho Chỉnh sửa
        public AddEditQuestionWindow(QuestionBank question)
        {
            InitializeComponent();
            var vm = new AddEditQuestionViewModel(question);
            vm.CloseAction = () => this.Close();
            this.DataContext = vm;
        }
    }
}
