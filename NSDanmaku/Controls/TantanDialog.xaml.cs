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
        TanTanPlay tantan;
        public TantanDialog()
        {
            this.InitializeComponent();
            tantan = new TanTanPlay();
        }
        public event EventHandler<List<DanmakuModel>> ReturnDanmakus;
        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (list_Items.SelectedItem == null)
            {
                args.Cancel = true;
                return;
            }
            var data = list_Items.SelectedItem as Episodes;
            ReturnDanmakus(null, await tantan.GetDanmakus(data.EpisodeId));
        }

        private async void txt_search_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            txt_error.Visibility = Visibility.Collapsed;
            if (txt_search.Text.Length == 0)
            {
                ShowError("请输入关键字");
                return;
            }
            try
            {
                list_Items.ItemsSource = await tantan.Search(txt_search.Text);
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            txt_error.Visibility = Visibility.Visible;
            txt_error.Text = msg;
        }
    }
}
