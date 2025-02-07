﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NSDanmaku.Helper;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Demo
{
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
        }

        private void btn_AddRoll_Click(object sender, RoutedEventArgs e)
        {
            danmaku.AddRollDanmu(new NSDanmaku.Model.DanmakuModel()
            {
                Color = Colors.White,
                Location = NSDanmaku.Model.DanmakuLocation.Roll,
                Size = 25,
                Text = text.Text
            }, ck_own.IsChecked.Value);
        }

        private void btn_AddTop_Click(object sender, RoutedEventArgs e)
        {
            danmaku.AddTopDanmu(new NSDanmaku.Model.DanmakuModel()
            {
                Color = Colors.Blue,
                Location = NSDanmaku.Model.DanmakuLocation.Roll,
                Size = 25,
                Text = text.Text
            }, ck_own.IsChecked.Value);
        }

        private void btn_AddBottom_Click(object sender, RoutedEventArgs e)
        {
            danmaku.AddBottomDanmu(new NSDanmaku.Model.DanmakuModel()
            {
                Color = Colors.Red,
                Location = NSDanmaku.Model.DanmakuLocation.Roll,
                Size = 25,
                Text = text.Text
            }, ck_own.IsChecked.Value);
        }

        private void btn_Clear_Click(object sender, RoutedEventArgs e)
        {
            danmaku.ClearAll();
        }

        private void Timer_Tick(object sender, object e)
        {
            var danmu = danmakus.Where(x => Convert.ToInt32(x.Time) == slider.Value);
            foreach (var item in danmu)
            {
                try
                {
                    switch (item.Location)
                    {
                        case NSDanmaku.Model.DanmakuLocation.Top:
                            danmaku.AddTopDanmu(item, false);
                            break;
                        case NSDanmaku.Model.DanmakuLocation.Bottom:
                            danmaku.AddBottomDanmu(item, false);
                            break;
                        default:
                            danmaku.AddRollDanmu(item, false);
                            break;
                    }
                }
                catch (Exception)
                {
                    Debug.WriteLine("Can't add danmaku:" + item.Source);
                }

            }
            slider.Value++;
        }
        List<NSDanmaku.Model.DanmakuModel> danmakus;
        private async void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if (danmakus == null)
            {
                try
                {
                    danmakus = await DanmakuParse.ParseBiliBili(29892777);
                }
                catch (Exception)
                {
                    Debug.WriteLine("Can't load danmaku");
                    return;
                }

            }
            danmaku.ResumeDanmaku();
            timer.Start();
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            danmaku.PauseDanmaku();

        }

        private void btn_GetAll_Click(object sender, RoutedEventArgs e)
        {
            var ls = danmaku.GetDanmakus();
            Debug.WriteLine("Count:" + ls.Count);
        }

        private void ck_HideRoll_Checked(object sender, RoutedEventArgs e)
        {
            danmaku.HideDanmaku(NSDanmaku.Model.DanmakuLocation.Roll);
        }

        private void ck_HideRoll_Unchecked(object sender, RoutedEventArgs e)
        {
            danmaku.ShowDanmaku(NSDanmaku.Model.DanmakuLocation.Roll);
        }
    }
}
