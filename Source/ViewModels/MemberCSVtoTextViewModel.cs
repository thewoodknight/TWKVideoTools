using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.YouTube.v3;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace TWKVideoTools.ViewModels
{

    public class MemberCSVtoTextViewModel : BaseViewModel
    {
        public ICommand LoadCSVCommand { get; set; }

        public string Content { get; set; }
        public MemberCSVtoTextViewModel() {

            LoadCSVCommand = new RelayCommand(LoadCSV);
        }

        public void LoadCSV()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Comma Seperated Values|*.csv";
            if (openFileDialog.ShowDialog()== true)
            {
                Parse(openFileDialog.FileName);
            }
        }

        private void Parse(string filename)
        {

            StringBuilder output = new StringBuilder();
            output.AppendLine("With Special Thanks To\n");

            using (TextFieldParser parser = new TextFieldParser(filename))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                
                int i = 0;
                while (!parser.EndOfData)
                {
                    //Processing row
                    string[] fields = parser.ReadFields();
                    if (fields[0] == "Member")
                        continue;
                    if ((i % 2) == 0)
                        output.Append("\t" + fields[0] + "\t");
                    else
                        output.AppendLine(fields[0]);

                    i++;
                }
            }

            Content = output.ToString();

        }
        
        // As far as I can tell, this is an invite only endpoint currently. Code probably works, but I don't have access
        /*public async Task YoutubeFetch(string credentialfile)
        {
            UserCredential credential;

            using (var stream = new FileStream(credentialfile, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { YouTubeService.Scope.YoutubeChannelMembershipsCreator },
                    "use5",
                    CancellationToken.None,
                    new FileDataStore(this.GetType().ToString())
                );
            }

            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "twkvideotools"
            });

            var channelMemberRequest = youtubeService.Members.List("snippet");
            var channelMemberResponse = await channelMemberRequest.ExecuteAsync();
            List<string> names = new List<string>();
            foreach (var member in channelMemberResponse.Items)
            {
                names.Add(member.Snippet.MemberDetails.DisplayName);
            }

        }*/
    }
}