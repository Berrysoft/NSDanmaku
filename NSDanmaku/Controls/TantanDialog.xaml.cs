using System;
using System.Collections.Generic;
using NSDanmaku.Helper;
using NSDanmaku.Model;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NSDanmaku.Controls
{
    public sealed partial class TantanDialog : ContentDialog
    {
        public TantanDialog()
        {
            this.InitializeComponent();
        }

        public event EventHandler<List<DanmakuModel>> ReturnDanmakus;
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (ResultView.SelectedItem == null)
            {
                args.Cancel = true;
                return;
            }
            var data = ResultView.SelectedItem as Episodes;
            ReturnDanmakus?.Invoke(null, await TanTanPlay.GetDanmakus(data.EpisodeId));
        }

        private async void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ErrorBlock.Visibility = Visibility.Collapsed;
            if (SearchBox.Text.Length == 0)
            {
                ShowError("请输入关键字");
                return;
            }
            try
            {
                ResultView.ItemsSource = await TanTanPlay.Search(SearchBox.Text);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            ErrorBlock.Visibility = Visibility.Visible;
            ErrorBlock.Text = msg;
        }
    }
}
