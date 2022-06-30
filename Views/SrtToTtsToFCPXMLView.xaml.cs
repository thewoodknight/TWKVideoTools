using TWKVideoTools.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace TWKVideoTools.Views
{
    public partial class SrtToTtsToFCPXMLView : UserControl
    {
        private SrtToTtsToFCPXMLViewModel ViewModel;

        public SrtToTtsToFCPXMLView()
        {
            InitializeComponent();
            Loaded += SrtToTtsToFCPXMLView_Loaded;
        }

        private void SrtToTtsToFCPXMLView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = (SrtToTtsToFCPXMLViewModel)DataContext;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Voicify();
        }

        private void btnSelectOutputFolder_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.btnSelectOutputFolderClicked();
        }

        private void btnGCloudConfig_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.btnSelectConfigFileClicked();
        }
    }
}