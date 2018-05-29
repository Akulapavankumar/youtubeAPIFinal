using System;
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
            
            if (video.VideoId != null)
            {
                Device.OpenUri(new Uri("https://www.youtube.com/watch?v=" + video.VideoId));
            }
            else
            {
                DisplayAlert("Pavan","ID Is Null !", "ok");
            }
        }
    }
}
