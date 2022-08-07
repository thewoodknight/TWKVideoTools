using System.Diagnostics;
using System.Windows.Controls;

namespace TWKVideoTools.Views
{

    public partial class MemberCSVtoTextView : UserControl
    {
        public MemberCSVtoTextView()
        {
            InitializeComponent();
        }

        private void btnYoutube_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://studio.youtube.com/channel/UCyg_SKOwdhJjp71VayO3wTQ/monetization/memberships",
                UseShellExecute = true
            });
        }
    }
}
