using Google.Cloud.TextToSpeech.V1;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace TWKVideoTools.ViewModels
{
    public class SrtToTtsToFCPXMLViewModel : BaseViewModel
    {
        public string SrtFilePath { get; set; }
        public string OutputPath { get; set; }
        public string KeyPath { get; set; }

        public string Prefix { get; set; }

        public ObservableCollection<SubtitlesParser.Classes.SubtitleItem> SubtitleItems { get; set; }

        public ICommand LoadSrtCommand { get; set; }
        public ICommand BrowseForOutputDirectoryCommand { get; set; }
        public ICommand BrowseForGCloudConfigCommand { get; set; }

        public ICommand ConvertTextToSpeechCommand { get; set; }

        public SrtToTtsToFCPXMLViewModel()
        {
            SubtitleItems = new ObservableCollection<SubtitlesParser.Classes.SubtitleItem>();
            OutputPath = Settings.OutputPath.Current.Value;
            KeyPath = Settings.KeyPath.Current.Value;
            Prefix = Settings.Prefix.Current.Value;

            LoadSrtCommand = new RelayCommand(() => { SelectSrt(); });
            BrowseForOutputDirectoryCommand = new RelayCommand(BrowseForOutputDirectory);
            BrowseForGCloudConfigCommand = new RelayCommand(BrowseForGCloudConfig);
            ConvertTextToSpeechCommand = new AsyncRelayCommand(() => ConvertSrtToTts(), CanExecuteConversion);
        }

        private bool CanExecuteConversion()
        {
            if (string.IsNullOrEmpty(SrtFilePath) || string.IsNullOrEmpty(OutputPath) || string.IsNullOrEmpty(KeyPath))
                return false;

            return true;
        }

        private void BrowseForGCloudConfig()
        {
            
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "Config|*.json" };
            if (openFileDialog.ShowDialog() == true)
            {
                KeyPath = openFileDialog.FileName;
                Settings.KeyPath.Current.Value = KeyPath;
                Common.Settings.SettingsUtility.Save();
            }

            ((AsyncRelayCommand)ConvertTextToSpeechCommand).NotifyCanExecuteChanged();
        }

        private void BrowseForOutputDirectory()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                OutputPath = dialog.FileName;
                Settings.OutputPath.Current.Value = OutputPath;
                Common.Settings.SettingsUtility.Save();
            }

            ((AsyncRelayCommand)ConvertTextToSpeechCommand).NotifyCanExecuteChanged();
        }

        public async Task ConvertSrtToTts(bool skip = false)
        {
            Settings.Prefix.Current.Value = Prefix;
            Common.Settings.SettingsUtility.Save();

            if (!skip)
            {
                //Iterate through all the loaded subtitles, and send them off to become indivdual wav files
                for (int i = 0; i < SubtitleItems.Count; i++)
                {
                    //this can be done concurrently, but needs to all be completed before XML generation can happen
                    await Voiceify(string.Join(",", SubtitleItems[i].PlaintextLines), Prefix, i);
                }
            }

            WriteFCPXML(Prefix);
        }

        public async Task Voiceify(string text, string filename = "output.mp3", int i = 0, string name = "en-AU-Wavenet-B", string language = "en-AU", SsmlVoiceGender voiceGender = SsmlVoiceGender.Male)
        {
            var client = new TextToSpeechClientBuilder() { CredentialsPath = KeyPath }.Build();
            var input = new SynthesisInput { Text = text };

            // Build the voice request.
            var voiceSelection = new VoiceSelectionParams
            {
                LanguageCode = language,
                Name = name,
                SsmlGender = voiceGender
            };

            // Specify the type of audio file.
            var audioConfig = new AudioConfig { AudioEncoding = AudioEncoding.Linear16 };

            // Perform the text-to-speech request.
            var response = client.SynthesizeSpeech(input, voiceSelection, audioConfig);

            // Write the response to the output file.
            using (var output = File.Create(string.Format("{0}\\{1}_{2}.wav", OutputPath, filename, i)))
            {
                response.AudioContent.WriteTo(output);
            }
        }

        public void SelectSrt()
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

            ((AsyncRelayCommand)ConvertTextToSpeechCommand).NotifyCanExecuteChanged();
        }

        private void WriteFCPXML(string prefix)
        {
            string name = "name_goes_here";

            XmlWriter xWrite;
            XmlWriterSettings xwSettings = new XmlWriterSettings();
            xwSettings.CheckCharacters = true;
            xwSettings.CloseOutput = true;
            xwSettings.Indent = true;
            xwSettings.IndentChars = "   ";

            string duration = "1158/5s"; //TODO: This needs to be calculated

            var file = Path.Combine(OutputPath, prefix + ".fcpxml");

            using (xWrite = XmlWriter.Create(file, xwSettings))
            {
                XElement spine = new XElement("spine");
                XElement assets = new XElement("resources",
                    new XElement("format", //I don't know if this is needed, but best not to question
                        new XAttribute("name", "FFVideoFormatRateUndefined"),
                        new XAttribute("id", "r0"),
                        new XAttribute("frameDuration", "1/25s"),
                        new XAttribute("width", "1080"),
                        new XAttribute("height", "1920")
                    ));

                for (int i = 0; i < SubtitleItems.Count; i++)
                {
                    string clipname = string.Format("{0}_{1}.wav", prefix, i);
                    string clipfilename = Path.Combine(OutputPath, clipname);

                    WaveFileReader reader = new WaveFileReader(clipfilename);

                    string trackDuration = Math.Floor((reader.TotalTime.TotalSeconds * 25)).ToString() + "/25s";// I *think* this is in <frames>/<framerate>, ie 6 second clip @ 25fps = "157/25s"

                    var startTime = Math.Floor((((double)SubtitleItems[i].StartTime) / 1000) * 25).ToString() + "/25s";
                    assets.Add(
                        new XElement("asset",
                            new XAttribute("name", clipname),
                            new XAttribute("id", "r" + i),
                            new XAttribute("duration", trackDuration),
                            new XAttribute("audioSources", "1"),
                            new XAttribute("start", "0/1s"),
                            new XAttribute("audioChannels", "1"),
                            new XAttribute("hasAudio", "1"),
                            new XElement("media-rep",
                                new XAttribute("kind", "original-media"),
                                new XAttribute("src", string.Format("file://localhost/{0}", clipfilename.Replace('\\', '/')) // TODO: Better way to convert the path? Needs full file path, *nix style slashes, ie Z:/my/path/to/file.wav
                        ))));

                    spine.Add(
                        new XElement("asset-clip",
                            new XAttribute("lane", "4"),    //which layer?
                            new XAttribute("enabled", "1"),
                            new XAttribute("name", clipname),
                            new XAttribute("duration", trackDuration),
                            new XAttribute("ref", "r" + i),
                            new XAttribute("offset", startTime),   //Offset in frames (seconds * fps) it starts, '12345/25s'
                            new XAttribute("start", "0/1s")    //'12345/25s'
                        ));
                }

                XDocument doc = new XDocument(
                    new XDocumentType("fcpxml", null, null, null),
                    new XElement("fcpxml", new XAttribute("version", "1.9"),
                           assets,
                           new XElement("library",
                            new XElement("event",
                                new XAttribute("name", name),
                                new XElement("project",
                                    new XAttribute("name", name),
                                    new XElement("sequence",
                                    new XAttribute("tcStart", "0/1s"),
                                    new XAttribute("duration", duration),    //whole sequence length? why is this /5s?
                                    new XAttribute("tcFormat", "NDF"),
                                    new XAttribute("format", "r0"), // links to "format" in resources
                                    spine
                                    ))))));

                doc.WriteTo(xWrite);
            }
        }
    }
}