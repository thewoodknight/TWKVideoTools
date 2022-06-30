using TWKVideoTools.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace TWKVideoTools.Views
{
    public partial class EdlToYoutubeChapterView : UserControl
    {
        private EdlToYoutubeChapterViewModel ViewModel;

        public EdlToYoutubeChapterView()
        {
            InitializeComponent();
            Loaded += EdlToYoutubeChapterView_Loaded;
        }

        private void EdlToYoutubeChapterView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = (EdlToYoutubeChapterViewModel)DataContext;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ConvertClick();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.OutputText();
        }

        private void LoadEdlClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadEdl();
        }
    }
}
