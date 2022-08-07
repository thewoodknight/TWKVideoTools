using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TWKVideoTools.Models;

namespace TWKVideoTools.ViewModels
{
    public class EdlToYoutubeChapterViewModel : BaseViewModel
    {
        //|C:ResolveColorBlue |M:Whats Blockboard? |D:1
        private Regex MarkerPattern = new Regex(@"\|C:([a-zA-Z]*) \|M:([a-zA-Z0-9\\.\\,()\\#\\:\-\\! \\/?&]*) \|D:1", RegexOptions.Compiled);

        //001  001      V     C        00:01:00:14 00:01:00:15 00:01:00:14 00:01:00:15
        private Regex TimePattern = new Regex(@"(?:[0-9][0-9]):[0-5][0-9]:[0-5][0-9].[0-9][0-9]", RegexOptions.Compiled);

        public List<MarkerColour> Colours { get; set; }
        public List<Marker> Markers { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public bool Dummy { get; set; }
        public bool PadHours { get; set; }

        public EdlToYoutubeChapterViewModel()
        {
            Markers = new List<Marker>();
            Colours = new List<MarkerColour>();
        }

        public void OutputText()
        {
            StringBuilder output = new StringBuilder();

            //Figure out what markers have been checked
            var x = Colours
                .Where(c => c.Checked)
                .Select(c => c.Label)
                .ToList();

            //render only the checked markers
            RenderMarkers(output, Markers
                .Where(m => x.Contains(m.Colour))
                .OrderBy(m => m.Time));

            Output = output.ToString();
        }

        private StringBuilder RenderMarkers(StringBuilder sb, IEnumerable<Marker> _markers)
        {
            foreach (var m in _markers)
            {
                //Format is suitable for <1hr videos
                if (m.Time.Hours > 0 || PadHours)
                    sb.AppendLine(string.Format("{0} - {1}", m.Time.ToString(@"hh\:mm\:ss"), m.MarkerText));
                else
                    sb.AppendLine(string.Format("{0} - {1}", m.Time.ToString(@"mm\:ss"), m.MarkerText));
            }
            return sb;
        }

        //Auto-wired by IL Weaving
        public void OnDummyChanged()
        {
            ConvertClick();
        }

        //Auto-wired by IL Weaving
        public void OnPadHoursChanged()
        {
            ConvertClick();
        }

        public void ConvertClick()
        {
            Markers.Clear();

            //Do we need to output "video start"?
            if (Dummy)
                Markers.Add(new Marker() { Time = new TimeSpan(0), MarkerText = "Video Start" });

            using (var reader = new StringReader(Input))
            {
                string line;
                /* EDL is (as far as I can tell) in two line pairs eg

                      001  001      V     C        00:00:58:19 00:00:58:20 00:00:58:19 00:00:58:20
                      | C:ResolveColorBlue | M:Whats Blockboard? | D:1

                      So read one line, parse, then another, parse, then loop.

                */
                while ((line = reader.ReadLine()) != null)
                {
                    Marker m = new Marker();
                    var timeMatch = TimePattern.Match(line);
                    if (!timeMatch.Success)
                        continue;

                    //Why not TimeSpan.Parse? It kept refusing to interpret it as hh:mm:ss:ms, moving everything up one.
                    var t = timeMatch.Captures[0].ToString().Split(':');
                    var ts = new TimeSpan(0, int.Parse(t[0]), int.Parse(t[1]), int.Parse(t[2]), int.Parse(t[3]));

                    m.Time = ts;

                    line = reader.ReadLine();
                    var markerMatch = MarkerPattern.Match(line);

                    if (!markerMatch.Success)
                        continue;

                    m.Colour = markerMatch.Groups[1].ToString();
                    m.MarkerText = markerMatch.Groups[2].ToString();

                    Markers.Add(m);
                }
            }

            Colours = Markers.Select(m => m.Colour).Distinct().Select(o =>
                          new MarkerColour
                          {
                              Checked = true,
                              Label = o
                          }).ToList();

            OutputText();
        }

        public async Task LoadEdl()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Edit Decision List|*.edl" };
            if (openFileDialog.ShowDialog() == true)
            {
                Input = File.ReadAllText(openFileDialog.FileName);
                if (!string.IsNullOrEmpty(Input))
                    ConvertClick();
            }
        }
    }
}