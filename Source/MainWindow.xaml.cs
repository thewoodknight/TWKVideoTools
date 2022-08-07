using System.Windows;
using TWKVideoTools.ViewModels;

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
    }
}