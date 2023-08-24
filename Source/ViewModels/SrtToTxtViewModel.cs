using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

namespace TWKVideoTools.ViewModels
{
    public class SrtToTxtViewModel : BaseViewModel
    {
        public ObservableCollection<SubtitlesParser.Classes.SubtitleItem> SubtitleItems { get; set; }
        public string SrtFilePath { get; set; }
        public string Text { get; set; }
        public ICommand GoCommand { get; set; }

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            {"dado", "daydo"},
            {" mm ", " millimeter " },
            {"nopeino", "nope-in-o" },
            {"formply", "form ply" },
            {"mortise", "mortice" }
        };

        public SrtToTxtViewModel()
        {
            SubtitleItems = new ObservableCollection<SubtitlesParser.Classes.SubtitleItem>();
            GoCommand = new RelayCommand(() => { Go(); });

        }

        bool ReplaceWords = false;
        public async Task Go()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "SubRip Subtitle|*.srt" };
            if (openFileDialog.ShowDialog() == true)
            {
                SrtFilePath = openFileDialog.FileName;
                SubtitleItems.Clear();

                var parser = new SubtitlesParser.Classes.Parsers.SubParser();
                using var fileStream = File.OpenRead(SrtFilePath);
                var items = parser.ParseStream(fileStream);

                foreach (var i in items)
                {
                    SubtitleItems.Add(i);
                }
            }

            
            StringBuilder sb = new StringBuilder();
            foreach (var i in SubtitleItems)
            {
                sb.AppendLine(string.Join(" ", i.PlaintextLines));
                sb.AppendLine();
            }

            if (ReplaceWords)
                Text = Replace(sb.ToString());
            else 
                Text = sb.ToString();
        }

        public string Replace(string input)
        {
            foreach (var r in replacements)
                input = input.Replace(r.Key, r.Value, System.StringComparison.InvariantCultureIgnoreCase);

            return input;
        }
    }
}