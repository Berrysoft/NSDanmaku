using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp.UI.Controls;
using NSDanmaku.Model;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace NSDanmaku.Controls
{
    public sealed partial class Danmaku : UserControl
    {
        public Danmaku()
        {
            this.InitializeComponent();
            NotHideSubtitle = false;
            Bold = false;
            Font = string.Empty;
        }
        /// <summary>
        /// 字体大小缩放，电脑推荐默认1.0，手机推荐0.5
        /// </summary>
        public double SizeZoom
        {
            get => (double)GetValue(SizeZoomProperty);
            set => SetValue(SizeZoomProperty, value);
        }
        // Using a DependencyProperty as the backing store for sizeZoom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeZoomProperty =
            DependencyProperty.Register(nameof(SizeZoom), typeof(double), typeof(Danmaku), new PropertyMetadata(1.0, OnSizeZoomChanged));

        private static void OnSizeZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = Convert.ToDouble(e.NewValue);
            if (value > 3)
            {
                value = 3;
            }
            if (value < 0.1)
            {
                value = 0.1;
            }

            ((Danmaku)d).SetFontSizeZoom(value);
        }
        private void SetFontSizeZoom(double value)
        {
            TestBlock.FontSize = 25 * value;
            SetRows();
            foreach (var item in RollGrid.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * SizeZoom;
                    }
                }
            }
            foreach (var item in TopGrid.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * SizeZoom;
                    }
                }
            }
            foreach (var item in BottomGrid.Children)
            {
                var grid = item as Grid;
                var m = grid.Tag as DanmakuModel;
                foreach (var tb in grid.Children)
                {
                    if (tb is TextBlock)
                    {
                        (tb as TextBlock).FontSize = Convert.ToInt32(m.size) * SizeZoom;
                    }
                }
            }
        }

        /// <summary>
        /// 滚动弹幕速度
        /// </summary>
        public int Speed
        {
            get => (int)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        // Using a DependencyProperty as the backing store for speed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register(nameof(Speed), typeof(int), typeof(Danmaku), new PropertyMetadata(10, OnSpeedChanged));
        private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var value = Convert.ToInt32(e.NewValue);
            if (value <= 0)
            {
                value = 1;
            }
           ((Danmaku)d).SetSpeed(value);
        }
        public void SetSpeed(int value)
        {
            Speed = value;
        }

        public bool Bold
        {
            get => (bool)GetValue(BoldProperty);
            set => SetValue(BoldProperty, value);
        }

        // Using a DependencyProperty as the backing store for bold.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoldProperty =
            DependencyProperty.Register(nameof(Bold), typeof(bool), typeof(Danmaku), new PropertyMetadata(0));



        public string Font
        {
            get => (string)GetValue(FontProperty);
            set => SetValue(FontProperty, value);
        }

        // Using a DependencyProperty as the backing store for font.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FontProperty =
            DependencyProperty.Register(nameof(Font), typeof(string), typeof(Danmaku), new PropertyMetadata(0));


        /// <summary>
        /// 弹幕样式
        /// </summary>
        public DanmakuBorderStyle BorderStyle
        {
            get => (DanmakuBorderStyle)GetValue(BorderStyleProperty);
            set => SetValue(BorderStyleProperty, value);
        }

        // Using a DependencyProperty as the backing store for borderStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BorderStyleProperty =
            DependencyProperty.Register(nameof(BorderStyle), typeof(DanmakuBorderStyle), typeof(Danmaku), new PropertyMetadata(DanmakuBorderStyle.Default));



        /// <summary>
        /// 弹幕动画管理
        /// </summary>
        private readonly List<Storyboard> topBottomStoryList = new List<Storyboard>();
        private readonly List<Storyboard> rollStoryList = new List<Storyboard>();
        private readonly List<Storyboard> positionStoryList = new List<Storyboard>();

        /// <summary>
        /// 弹幕模式
        /// </summary>
        public DanmakuMode danmakuMode = DanmakuMode.Video;

        /// <summary>
        /// 防挡字幕
        /// </summary>
        public bool NotHideSubtitle
        {
            get => (bool)GetValue(NotHideSubtitleProperty);
            set => SetValue(NotHideSubtitleProperty, value);
        }

        // Using a DependencyProperty as the backing store for notHideSubtitle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NotHideSubtitleProperty =
            DependencyProperty.Register(nameof(NotHideSubtitle), typeof(bool), typeof(Danmaku), new PropertyMetadata(false));


        public void InitializeDanmaku(DanmakuMode mode, double fontZoom, int _speed, DanmakuBorderStyle style)
        {
            SizeZoom = fontZoom;
            Speed = _speed;
            BorderStyle = style;
            SetRows();
        }


        protected override Size MeasureOverride(Size availableSize)
        {
            SetRows();
            return base.MeasureOverride(availableSize);
        }

        private void SetRows()
        {
            if (TestBlock.ActualHeight == 0)
            {
                return;
            }
            var rowHieght = TestBlock.ActualHeight;

            var topHieght = TopGrid.ActualHeight;
            var pageHieght = RollGrid.ActualHeight;
            //将全部行去除
            TopGrid.RowDefinitions.Clear();
            BottomGrid.RowDefinitions.Clear();
            RollGrid.RowDefinitions.Clear();

            int num = Convert.ToInt32(topHieght / rowHieght);
            int pnum = Convert.ToInt32(pageHieght / rowHieght);

            for (int i = 0; i < num; i++)
            {
                BottomGrid.RowDefinitions.Add(new RowDefinition());
                TopGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < pnum; i++)
            {
                RollGrid.RowDefinitions.Add(new RowDefinition());
            }
        }

        /// <summary>
        /// 暂停弹幕
        /// </summary>
        public void PauseDanmaku()
        {
            foreach (var item in topBottomStoryList)
            {
                item.Pause();
            }
            foreach (var item in rollStoryList)
            {
                item.Pause();
            }
            foreach (var item in positionStoryList)
            {
                item.Pause();
            }
        }
        /// <summary>
        /// 继续弹幕
        /// </summary>
        public void ResumeDanmaku()
        {
            foreach (var item in topBottomStoryList)
            {
                item.Resume();
            }
            foreach (var item in rollStoryList)
            {
                item.Resume();
            }
            foreach (var item in positionStoryList)
            {
                item.Resume();
            }
        }

        public void Remove(DanmakuModel danmaku)
        {
            switch (danmaku.location)
            {
                case DanmakuLocation.Top:

                    foreach (Grid item in TopGrid.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
                        {
                            TopGrid.Children.Remove(item);
                        }
                    }
                    break;
                case DanmakuLocation.Bottom:
                    foreach (Grid item in BottomGrid.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
                        {
                            BottomGrid.Children.Remove(item);
                        }
                    }
                    break;
                case DanmakuLocation.Other:
                    foreach (Grid item in RollGrid.Children)
                    {
                        if (item.Tag as DanmakuModel == danmaku)
                        {
                            RollGrid.Children.Remove(item);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearAll()
        {
            topBottomStoryList.Clear();
            rollStoryList.Clear();
            BottomGrid.Children.Clear();
            TopGrid.Children.Clear();
            RollGrid.Children.Clear();
        }

        private TextBlock CreateControlTextBlock(DanmakuModel model)
        {
            TextBlock tx = new TextBlock();

            tx.Text = model.text;
            if (Bold)
            {
                tx.FontWeight = FontWeights.Bold;
            }
            if (!string.IsNullOrEmpty(Font))
            {
                tx.FontFamily = new FontFamily(Font);
            }
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            tx.FontSize = model.size * SizeZoom;

            return tx;
        }

        private Grid CreateControlShadow(DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = CreateControlTextBlock(model);
            DropShadowPanel dropShadowPanel = new DropShadowPanel()
            {
                BlurRadius = 6,
                ShadowOpacity = 1,
                OffsetX = 0,
                OffsetY = 0,
                Color = SetBorder(model.color)
            };

            Grid grid = new Grid();

            dropShadowPanel.Content = tx;

            grid.Children.Add(dropShadowPanel);
            grid.Tag = model;
            return grid;
        }
        private Grid CreateControlBorder(DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = CreateControlTextBlock(model);
            TextBlock tx2 = CreateControlTextBlock(model);
            Grid grid = new Grid();

            tx2.Foreground = new SolidColorBrush(SetBorder(model.color));
            //grid包含弹幕文本信息

            grid.Children.Add(tx2);
            grid.Children.Add(tx);
            grid.Tag = model;
            return grid;
        }
        private Grid CreateControlNoBorder(DanmakuModel model)
        {
            //创建基础控件
            TextBlock tx = CreateControlTextBlock(model);

            Grid grid = new Grid();

            grid.Children.Add(tx);
            grid.Tag = model;
            return grid;
        }
        private Grid CreateImageControl(BitmapImage image)
        {
            //创建基础控件
            Image img = new Image();
            img.Source = image;
            Grid grid = new Grid();
            DanmakuModel model = new DanmakuModel() { text = "666666" };
            grid.Tag = model;
            grid.Children.Add(img);
            return grid;
        }
        private async Task<Grid> CreateControlBorder2(DanmakuModel model)
        {
            float size = (float)model.size * (float)SizeZoom;

            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasTextFormat fmt = new CanvasTextFormat() { FontSize = size };
            if (Bold)
            {
                fmt.FontWeight = FontWeights.Bold;
            }
            if (!string.IsNullOrEmpty(Font))
            {
                fmt.FontFamily = Font;
            }
            using (var myBitmap = new CanvasRenderTarget(device, model.text.Length * size, 34, 96))
            {
                CanvasTextLayout canvasTextLayout = new CanvasTextLayout(device, model.text, fmt, model.text.Length * size, 0);

                CanvasGeometry combinedGeometry = CanvasGeometry.CreateText(canvasTextLayout);

                using (var ds = myBitmap.CreateDrawingSession())
                {

                    ds.FillGeometry(combinedGeometry, model.color);
                    ds.DrawGeometry(combinedGeometry, SetBorder(model.color), 1.0f);
                }
                Image image = new Image();
                BitmapImage im = new BitmapImage();
                using (InMemoryRandomAccessStream oStream = new InMemoryRandomAccessStream())
                {
                    // Save the Win2D canvas renderer a stream.
                    await myBitmap.SaveAsync(oStream, CanvasBitmapFileFormat.Png, 1.0f);
                    // Stream our Win2D pixels into the Bitmap
                    await im.SetSourceAsync(oStream);
                }
                image.Source = im;
                image.Stretch = Stretch.None;
                Grid grid = new Grid();

                grid.Tag = model;
                grid.Children.Add(image);

                return grid;
            }
        }

        private Color SetBorder(Color textColor)
        {
            if (textColor.R < 100 && textColor.G < 100 && textColor.B < 100)
            {
                return Colors.White;
            }
            else if (textColor.R > 200 && textColor.G < 100 && textColor.B < 100)
            {
                return Colors.White;
            }
            else if (textColor.R < 100 && textColor.G > 200 && textColor.B < 100)
            {
                return Colors.White;
            }
            else if (textColor.R < 100 && textColor.G < 100 && textColor.B > 200)
            {
                return Colors.White;
            }
            else
            {
                return Colors.Black;
            }
        }

        private int currentTopRow = 0;
        private int ComputeTopRow()
        {
            var max = TopGrid.RowDefinitions.Count;
            if (max == 0)
            {
                SetRows();
                max = TopGrid.RowDefinitions.Count;
            }
            if (NotHideSubtitle)
            {
                max -= 3;
            }
            for (int i = 0; i <= currentTopRow; i++)
            {
                var last = TopGrid.Children.LastOrDefault(x => Grid.GetRow(x as Grid) == i) as Grid;
                if (last == null)
                {
                    return i;
                }
            }
            currentTopRow++;
            currentTopRow %= max;
            return currentTopRow;
        }

        private int currentBottomRow = 0;
        private int ComputeBottomRow()
        {
            var max = BottomGrid.RowDefinitions.Count;
            if (max == 0)
            {
                SetRows();
                max = BottomGrid.RowDefinitions.Count;
            }
            if (NotHideSubtitle)
            {
                max -= 3;
            }
            for (int i = max - 1; i >= currentBottomRow; i--)
            {
                var last = BottomGrid.Children.LastOrDefault(x => Grid.GetRow(x as Grid) == i) as Grid;
                if (last == null)
                {
                    return i;
                }
            }
            currentBottomRow += max - 1;
            currentBottomRow %= max;
            return currentBottomRow;
        }

        private int currentRow = 0;
        private int ComputeRollRow(Grid item)
        {
            double num = 0.8;
            if ((item.Tag as DanmakuModel).text.Length > 15)
            {
                num = 0.6;
            }
            var max = RollGrid.RowDefinitions.Count;
            if (max == 0)
            {
                SetRows();
                max = RollGrid.RowDefinitions.Count;
            }
            if (NotHideSubtitle)
            {
                max -= 3;
            }
            for (int i = 0; i <= currentRow; i++)
            {
                var last = RollGrid.Children.LastOrDefault(x => Grid.GetRow(x as Grid) == i) as Grid;
                if (last == null || (last.RenderTransform as TranslateTransform).X <= RollGrid.ActualWidth * num)
                {
                    return i;
                }
            }
            currentRow++;
            currentRow %= max;
            return currentRow;
        }

        /// <summary>
        /// 添加滚动弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public async void AddRollDanmu(DanmakuModel m, bool own)
        {
            Grid grid = null;
            switch (BorderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                case DanmakuBorderStyle.BorderV2:
                    grid = await CreateControlBorder2(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(m.color);
                grid.BorderThickness = new Thickness(1);
            }
            var r = ComputeRollRow(grid);
            if (r == -1)
            {
                return;
            }
            Grid.SetRow(grid, r);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;
            RollGrid.Children.Add(grid);
            RollGrid.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = MainGrid.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(Speed));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            myDoubleAnimationX.To = -(grid.ActualWidth);//到达
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            rollStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += (senders, obj) =>
            {
                RollGrid.Children.Remove(grid);
                grid = null;
                rollStoryList.Remove(moveStoryboard);
            };
            moveStoryboard.Begin();
        }

        /// <summary>
        /// 添加图片滚动弹幕
        /// </summary>
        /// <param name="m">参数</param>
        public void AddRollImageDanmu(BitmapImage m)
        {
            Grid grid = null;
            grid = CreateImageControl(m);
            var r = ComputeRollRow(grid);
            if (r == -1)
            {
                return;
            }
            Grid.SetRow(grid, r);
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;
            RollGrid.Children.Add(grid);
            RollGrid.UpdateLayout();

            TranslateTransform moveTransform = new TranslateTransform();
            moveTransform.X = MainGrid.ActualWidth;
            grid.RenderTransform = moveTransform;

            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(Speed));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            myDoubleAnimationX.To = -(grid.ActualWidth);//到达
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            rollStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += (senders, obj) =>
            {
                RollGrid.Children.Remove(grid);
                grid = null;
                rollStoryList.Remove(moveStoryboard);
            };
            moveStoryboard.Begin();
        }


        /// <summary>
        /// 添加直播滚动弹幕
        /// </summary>
        /// <param name="text">参数</param>
        /// <param name="own">是否自己发送的</param>
        /// <param name="color">颜色</param>
        public void AddLiveDanmu(string text, bool own, Color? color)
        {
            if (color == null)
            {
                color = Colors.White;
            }
            var m = new DanmakuModel()
            {
                text = text,
                color = color.Value,
                location = DanmakuLocation.Roll,
                size = 25
            };
            AddRollDanmu(m, own);
        }
        /// <summary>
        ///  添加顶部弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public async void AddTopDanmu(DanmakuModel m, bool own)
        {
            Grid grid = null;
            switch (BorderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                case DanmakuBorderStyle.BorderV2:
                    grid = await CreateControlBorder2(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(m.color);
                grid.BorderThickness = new Thickness(1);
            }

            var r = ComputeTopRow();
            if (r == -1)
            {
                return;
            }

            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(grid, r);
            TopGrid.Children.Add(grid);


            //创建空转换动画
            TranslateTransform moveTransform = new TranslateTransform();
            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(5));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            topBottomStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                TopGrid.Children.Remove(grid);
                grid = null;
                topBottomStoryList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }
        /// <summary>
        ///  添加底部弹幕
        /// </summary>
        /// <param name="m">参数</param>
        /// <param name="own">是否自己发送的</param>
        public async void AddBottomDanmu(DanmakuModel m, bool own)
        {
            Grid grid = null;
            switch (BorderStyle)
            {
                case DanmakuBorderStyle.Default:
                    grid = CreateControlBorder(m);
                    break;
                case DanmakuBorderStyle.NoBorder:
                    grid = CreateControlNoBorder(m);
                    break;
                case DanmakuBorderStyle.Shadow:
                    grid = CreateControlShadow(m);
                    break;
                case DanmakuBorderStyle.BorderV2:
                    grid = await CreateControlBorder2(m);
                    break;
                default:
                    break;
            }
            if (own)
            {
                grid.BorderBrush = new SolidColorBrush(m.color);
                grid.BorderThickness = new Thickness(1);
            }
            grid.HorizontalAlignment = HorizontalAlignment.Center;
            grid.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetRow(grid, ComputeBottomRow());
            BottomGrid.Children.Add(grid);


            //创建空转换动画
            TranslateTransform moveTransform = new TranslateTransform();
            //创建动画
            Duration duration = new Duration(TimeSpan.FromSeconds(5));
            DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
            myDoubleAnimationX.Duration = duration;
            //创建故事版
            Storyboard moveStoryboard = new Storyboard();
            moveStoryboard.Duration = duration;
            moveStoryboard.Children.Add(myDoubleAnimationX);
            Storyboard.SetTarget(myDoubleAnimationX, moveTransform);
            //故事版加入动画   
            Storyboard.SetTargetProperty(myDoubleAnimationX, "X");
            topBottomStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                BottomGrid.Children.Remove(grid);
                grid = null;
                topBottomStoryList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }

        public void AddPositionDanmu(DanmakuModel model)
        {
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(model.text);
            //创建基础控件

            TextBlock tx2 = new TextBlock();
            tx2.Foreground = new SolidColorBrush(SetBorder(model.color));
            tx2.Margin = new Thickness(1);

            TextBlock tx = new TextBlock();
            Grid grid = new Grid();

            tx.FontWeight = FontWeights.Bold;
            tx2.FontWeight = FontWeights.Bold;
            tx.Foreground = new SolidColorBrush(model.color);
            //弹幕大小
            double size = model.size * SizeZoom;
            tx.FontSize = size;
            tx2.FontSize = size;

            tx.Text = data[4].ToString().Replace("/n", "\r\n");
            tx2.Text = data[4].ToString().Replace("/n", "\r\n");

            tx.FontFamily = new FontFamily(data[data.Length - 2].ToString());
            tx2.FontFamily = new FontFamily(data[data.Length - 2].ToString());

            grid.Children.Add(tx2);
            grid.Children.Add(tx);

            grid.Tag = model;
            grid.HorizontalAlignment = HorizontalAlignment.Left;
            grid.VerticalAlignment = VerticalAlignment.Center;

            double toX = 0;
            double toY = 0;

            double X = 0, Y = 0;
            double dur = 0;

            if (data.Length > 7)
            {
                X = Convert.ToDouble(data[0]);
                Y = Convert.ToDouble(data[1]);

                toX = Convert.ToDouble(data[7]);
                toY = Convert.ToDouble(data[8]);

                dur = Convert.ToDouble(data[10]);

            }
            else
            {
                toX = Convert.ToDouble(data[0]);
                toY = Convert.ToDouble(data[1]);
            }
            if (toX < 1 && toY < 1)
            {
                toX = this.ActualWidth * toX;
                toY = this.ActualHeight * toY;
            }
            if (X < 1 && Y < 1)
            {
                X = this.ActualWidth * X;
                Y = this.ActualHeight * Y;
            }

            if (data.Length >= 7)
            {
                var rotateZ = Convert.ToDouble(data[5]);
                var rotateY = Convert.ToDouble(data[6]);
                PlaneProjection projection = new PlaneProjection();
                projection.RotationZ = -rotateZ;
                projection.RotationY = rotateY;
                grid.Projection = projection;
            }

            PositionCanvas.Children.Add(grid);


            double dmDuration = Convert.ToDouble(data[3]);
            var opacitys = data[2].ToString().Split('-');
            double opacityFrom = Convert.ToDouble(opacitys[0]);
            double opacityTo = Convert.ToDouble(opacitys[1]);

            //创建故事版
            Storyboard moveStoryboard = new Storyboard();

            Duration duration = new Duration(TimeSpan.FromMilliseconds(dur));
            {
                DoubleAnimation myDoubleAnimationY = new DoubleAnimation();
                myDoubleAnimationY.Duration = duration;
                myDoubleAnimationY.From = Y;
                myDoubleAnimationY.To = toY;


                Storyboard.SetTarget(myDoubleAnimationY, grid);
                Storyboard.SetTargetProperty(myDoubleAnimationY, "(Canvas.Top)");
                moveStoryboard.Children.Add(myDoubleAnimationY);
            }
            {
                DoubleAnimation myDoubleAnimationX = new DoubleAnimation();
                myDoubleAnimationX.Duration = duration;
                myDoubleAnimationX.From = X;
                myDoubleAnimationX.To = toX;
                Storyboard.SetTarget(myDoubleAnimationX, grid);
                Storyboard.SetTargetProperty(myDoubleAnimationX, "(Canvas.Left)");
                moveStoryboard.Children.Add(myDoubleAnimationX);
            }

            //透明度动画 
            DoubleAnimation opacityAnimation = new DoubleAnimation()
            {
                Duration = new Duration(TimeSpan.FromSeconds(dmDuration)),
                From = opacityFrom,
                To = opacityTo
            };
            Storyboard.SetTarget(opacityAnimation, grid);
            Storyboard.SetTargetProperty(opacityAnimation, "Opacity");
            moveStoryboard.Children.Add(opacityAnimation);



            positionStoryList.Add(moveStoryboard);

            moveStoryboard.Completed += new EventHandler<object>((senders, obj) =>
            {
                PositionCanvas.Children.Remove(grid);
                positionStoryList.Remove(moveStoryboard);
            });
            moveStoryboard.Begin();
        }



        /// <summary>
        /// 读取屏幕上的全部弹幕
        /// </summary>
        /// <param name="danmakuLocation">类型</param>
        /// <returns></returns>
        public List<DanmakuModel> GetDanmakus(DanmakuLocation? danmakuLocation = null)
        {
            List<DanmakuModel> danmakus = new List<DanmakuModel>();
            if (danmakuLocation == null || danmakuLocation == DanmakuLocation.Top)
            {
                foreach (Grid item in TopGrid.Children)
                {
                    danmakus.Add(item.Tag as DanmakuModel);
                }
            }
            if (danmakuLocation == null || danmakuLocation == DanmakuLocation.Bottom)
            {
                foreach (Grid item in BottomGrid.Children)
                {
                    danmakus.Add(item.Tag as DanmakuModel);
                }
            }
            if (danmakuLocation == null || danmakuLocation == DanmakuLocation.Roll)
            {
                foreach (Grid item in RollGrid.Children)
                {
                    danmakus.Add(item.Tag as DanmakuModel);
                }
            }
            return danmakus;
        }

        /// <summary>
        /// 隐藏弹幕
        /// </summary>
        /// <param name="location">需要隐藏的位置</param>
        public void HideDanmaku(DanmakuLocation location)
        {
            switch (location)
            {
                case DanmakuLocation.Roll:
                    RollGrid.Visibility = Visibility.Collapsed;
                    break;
                case DanmakuLocation.Top:
                    TopGrid.Visibility = Visibility.Collapsed;
                    break;
                case DanmakuLocation.Bottom:
                    BottomGrid.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 显示弹幕
        /// </summary>
        /// <param name="location">需要显示的位置</param>
        public void ShowDanmaku(DanmakuLocation location)
        {
            switch (location)
            {
                case DanmakuLocation.Roll:
                    RollGrid.Visibility = Visibility.Visible;
                    break;
                case DanmakuLocation.Top:
                    TopGrid.Visibility = Visibility.Visible;
                    break;
                case DanmakuLocation.Bottom:
                    BottomGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }
    }
}
