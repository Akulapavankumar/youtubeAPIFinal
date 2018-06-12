using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using YouTubeEmbed.Models;

namespace YouTubeEmbed.Pages
{
    public partial class YouTubeChannelPage : ContentPage
    {

        
        public YouTubeChannelPage()
        {
            InitializeComponent();
           
            
        }

        public void PlayVideo(object sender, SelectedItemChangedEventArgs e)
        {
            var video = e.SelectedItem as YouTubeItem;
            Navigation.PushAsync(new VideoPage(video.VideoId));
            // Navigation.PushAsync(new WebViewPage(video.VideoId));
            //if (video.VideoId != null)
            //{
            //    Device.OpenUri(new Uri("https://www.youtube.com/watch?v=" + video.VideoId));
            //}
            //else
            //{
            //    DisplayAlert("Pavan","ID Is Null !", "ok");
            //}
        }
               

        private void SearchDataContent(object sender, EventArgs e)
        {
            var filter = searchBar.Text;
            GetChannelData();
            
            MyList.ItemsSource = Items;
            

        }


        //const string ChannelId = "UCU_If3OQp9FPHoP1BteFNlA";

        //  string channelUrl = $"https://www.googleapis.com/youtube/v3/search?part=id&maxResults=20&channelId={ChannelId}&key={ApiKey}";

        private List<YouTubeItem> Items { get; set; } = new List<YouTubeItem>();
        private List<YouTubeItem> VideoId { get; set; } = new List<YouTubeItem>();

        //public override void Init(object initData)
        //{
        //    base.Init(initData);
        
        //    // Retrieve our data.
        //    Task.Factory.StartNew(GetChannelData);
        //}

        private async Task GetChannelData()
        {
            string filter = searchBar.Text;
            const string ApiKey = "AIzaSyB7cpaKiojWIKVz6rTYNRn_CiRI0GWt8q0";
            string channelUrl = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={filter}&maxResults=20&type=video&key={ApiKey}";
            try
            {

                using (var httpClient = new HttpClient())
                {
                    var videoIds = new List<string>();
                    var json = await httpClient.GetStringAsync(channelUrl);

                    //Deserialize our data, this is in a simple List format.
                    var response = JsonConvert.DeserializeObject<YouTubeApiListRoot>(json);

                    //Add all the video id's we've found to our list.
                    videoIds.AddRange(response.items.Select(item => item.id.videoId));

                    //Get the details for all our items.
                    Items = await GetVideoDetailsAsync(videoIds);
                }
            }
            catch (Exception ex)
            {
                var ms = ex;
            }
        }
       
        private async Task<List<YouTubeItem>> GetVideoDetailsAsync(List<string> videoIds)
        {
            const string ApiKey = "AIzaSyB7cpaKiojWIKVz6rTYNRn_CiRI0GWt8q0";
            string detailsUrl = "https://www.googleapis.com/youtube/v3/videos?part=snippet,statistics&key=" + ApiKey + "&id={0}";
            try
            {
                var videoIdString = string.Join(",", videoIds);
                var youtubeItems = new List<YouTubeItem>();

                using (var httpClient = new HttpClient())
                {
                    var json = await httpClient.GetStringAsync(string.Format(detailsUrl, videoIdString));
                    var response = JsonConvert.DeserializeObject<YouTubeApiDetailsRoot>(json);

                    foreach (var item in response.items)
                    {
                        var youTubeItem = new YouTubeItem()
                        {
                            Title = item.snippet.title,
                            Description = item.snippet.description,
                            ChannelTitle = item.snippet.channelTitle,
                            PublishedAt = item.snippet.publishedAt,
                            VideoId = item.id,
                            DefaultThumbnailUrl = item.snippet?.thumbnails?.@default?.url,
                            MediumThumbnailUrl = item.snippet?.thumbnails?.medium?.url,
                            HighThumbnailUrl = item.snippet?.thumbnails?.high?.url,
                            StandardThumbnailUrl = item.snippet?.thumbnails?.standard?.url,
                            MaxResThumbnailUrl = item.snippet?.thumbnails?.maxres?.url,
                            ViewCount = item.statistics?.viewCount,
                            LikeCount = item.statistics?.likeCount,
                            DislikeCount = item.statistics?.dislikeCount,
                            FavoriteCount = item.statistics?.favoriteCount,
                            CommentCount = item.statistics?.commentCount,
                            Tags = item.snippet?.tags
                        };

                        youtubeItems.Add(youTubeItem);
                    }
                }

                return youtubeItems;
            }
            catch (Exception ex)
            {
                var ms = ex;
                return new List<YouTubeItem>();
            }
        }
    }
}
