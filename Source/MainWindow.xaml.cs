using TWKVideoTools.ViewModels;
using System.Windows;

namespace TWKVideoTools
{
    public partial class MainWindow : Window
    {
        public EdlToYoutubeChapterViewModel EdlToYoutubeChapterViewModel { get; set; }
        public SrtToTtsToFCPXMLViewModel SrtToTtsToFCPXMLViewModel { get; set; }

        public MainWindow()
        {
            EdlToYoutubeChapterViewModel = new EdlToYoutubeChapterViewModel();
            SrtToTtsToFCPXMLViewModel = new SrtToTtsToFCPXMLViewModel();
            DataContext = this;
            InitializeComponent();
        }
    }
}