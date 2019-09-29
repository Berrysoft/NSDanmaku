using Windows.UI;
using Windows.UI.Xaml.Media;

namespace NSDanmaku.Model
{
    public enum DanmakuLocation
    {
        /// <summary>
        /// 滚动弹幕Model1-3
        /// </summary>
        Roll,
        /// <summary>
        /// 顶部弹幕Model5
        /// </summary>
        Top,
        /// <summary>
        /// 底部弹幕Model4
        /// </summary>
        Bottom,
        /// <summary>
        /// 定位弹幕Model7
        /// </summary>
        Position,
        /// <summary>
        /// 其它暂未支持的类型
        /// </summary>
        Other
    }
    public enum DanmakuSite
    {
        Bilibili,
        Acfun,
        Tantan
    }
    public enum DanmakuBorderStyle
    {
        Default,
        NoBorder,
        Shadow,
        BorderV2
    }
    public enum DanmakuMode
    {
        Video,
        Live
    }
    public class DanmakuModel
    {
        public string Text { get; set; }
        /// <summary>
        /// 弹幕大小
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// 弹幕颜色
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// 弹幕出现时间
        /// </summary>
        public double Time { get; set; }
        /// <summary>
        /// 弹幕发送时间
        /// </summary>
        public string SendTime { get; set; }
        /// <summary>
        /// 弹幕池
        /// </summary>
        public string Pool { get; set; }
        /// <summary>
        /// 弹幕发送人ID
        /// </summary>
        public string SendID { get; set; }
        /// <summary>
        /// 弹幕ID
        /// </summary>
        public string RowID { get; set; }
        /// <summary>
        /// 弹幕出现位置
        /// </summary>
        public DanmakuLocation Location { get; set; }

        public DanmakuSite FromSite { get; set; }

        public string Source { get; set; }

        public SolidColorBrush ColorBrush => new SolidColorBrush(Color);
    }
}
