using System.Windows;
using TWKVideoTools.ViewModels;
using TWKVideoTools.Views;

namespace TWKVideoTools
{
    public partial class MainWindow : Window
    {
        public EdlToYoutubeChapterViewModel EdlToYoutubeChapterViewModel { get; set; }
        public SrtToTtsToFCPXMLViewModel SrtToTtsToFCPXMLViewModel { get; set; }

        public MemberCSVtoTextViewModel MemberCSVtoTextViewModel { get; set; }
        public SrtToTxtViewModel SrtToTxtViewModel { get; set; }

        public MainWindow()
        {
            EdlToYoutubeChapterViewModel = new EdlToYoutubeChapterViewModel();
            SrtToTtsToFCPXMLViewModel = new SrtToTtsToFCPXMLViewModel();
            MemberCSVtoTextViewModel = new MemberCSVtoTextViewModel();
            SrtToTxtViewModel = new SrtToTxtViewModel();
            DataContext = this;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var settingsView = new SettingsView();
            settingsView.DataContext = new SettingsViewModel();
            settingsView.ShowDialog();
        }
    }
}