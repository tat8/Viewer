using System.Windows;
using System.Windows.Controls;
using Viewer.ViewModels;

namespace Viewer.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        public void YearCombobox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var obj = (ComboBox)sender;
            var myTextBox = (TextBox)obj?.Template.FindName("PART_EditableTextBox", obj);
            if (myTextBox != null)
            {
                myTextBox.MaxLength = 4;
            }
        }
    }
}
