using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NSDanmaku.Model;
using Windows.UI;

namespace NSDanmaku.Helper
{
    /// <summary>
    /// 弹弹Play相关API操作
    /// </summary>
    public static class TanTanPlay
    {
        /// <summary>
        /// 通过关键字搜索弹弹Play上的剧集
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static async Task<List<Episodes>> Search(string keyword)
        {
            var results = await WebHelper.GetResults(new Uri("https://api.acplay.net/api/v2/search/episodes?anime=" + Uri.EscapeDataString(keyword)));
            var m = JsonConvert.DeserializeObject<TantanSearchModel>(results);
            if (m.Success)
            {
                List<Episodes> episodes = new List<Episodes>();
                foreach (var item in m.Animes)
                {
                    foreach (var item1 in item.Episodes)
                    {
                        item1.AnimeTitle = item.AnimeTitle;
                        episodes.Add(item1);
                    }
                }
                return episodes;
            }
            else
            {
                throw new Exception(m.ErrorMessage);
            }
        }

        public static async Task<List<DanmakuModel>> GetDanmakus(int episodeId)
        {
            try
            {
                var results = await WebHelper.GetResults(new Uri("https://api.acplay.net/api/v2/comment/" + episodeId));
                var m = JsonConvert.DeserializeObject<CommentModel>(results);
                List<DanmakuModel> list = new List<DanmakuModel>();
                if (m.Comments != null)
                {
                    foreach (var item in m.Comments)
                    {
                        var datas = item.Time.Split(',');
                        var location = DanmakuLocation.Roll;
                        switch (datas[1])
                        {
                            case "7":
                                location = DanmakuLocation.Position;
                                break;
                            case "4":
                                location = DanmakuLocation.Bottom;
                                break;
                            case "5":
                                location = DanmakuLocation.Top;
                                break;
                            default:
                                location = DanmakuLocation.Roll;
                                break;
                        }
                        var colorCode = int.Parse(datas[2]);
                        if (colorCode < 0x1000000)
                            colorCode |= unchecked((int)0xFF000000);
                        list.Add(new DanmakuModel()
                        {
                            Text = item.Text,
                            Color = Color.FromArgb((byte)((colorCode >> 24) & 0xFF), (byte)((colorCode >> 16) & 0xFF), (byte)((colorCode >> 8) & 0xFF), (byte)(colorCode & 0xFF)),
                            Location = location,
                            FromSite = DanmakuSite.Tantan,
                            RowID = item.Cid.ToString(),
                            Time = Convert.ToDouble(datas[0]),
                            SendID = datas[3],
                            Size = 25
                        });
                    }
                    return list;
                }
                else
                {
                    return list;
                }
            }
            catch (Exception)
            {
                return new List<DanmakuModel>();
            }
        }
    }
}
