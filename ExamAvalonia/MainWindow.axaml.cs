using Avalonia.Controls;
using ExamAvalonia.Pages;

namespace ExamAvalonia
{
    public partial class MainWindow : Window
    {
        public static MainWindow? Instance { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            MainControl.Content = new Products();
            SideBar.Content = new SideBar();
        }
    }
}