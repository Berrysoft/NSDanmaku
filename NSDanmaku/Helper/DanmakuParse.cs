using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using NSDanmaku.Model;
using Windows.Storage;
using Windows.UI;

namespace NSDanmaku.Helper
{
    public static class DanmakuParse
    {
        public static Task<string> GetBiliBili(long cid) => WebHelper.GetResults(new Uri($"https://api.bilibili.com/x/v1/dm/list.so?oid={cid}"));

        public static async Task<List<DanmakuModel>> ParseBiliBili(long cid) => ParseBiliBiliXml(await GetBiliBili(cid));

        public static List<DanmakuModel> ParseBiliBili(string xml) => ParseBiliBiliXml(xml);

        public static async Task<List<DanmakuModel>> ParseBiliBili(Uri url) => ParseBiliBiliXml(await WebHelper.GetResults(url));

        public static async Task<List<DanmakuModel>> ParseBiliBili(StorageFile file) => ParseBiliBiliXml(await FileIO.ReadTextAsync(file));

        private static List<DanmakuModel> ParseBiliBiliXml(string xmlStr)
        {
            List<DanmakuModel> ls = new List<DanmakuModel>();

            XmlDocument xdoc = new XmlDocument();
            //处理下特殊字符
            xmlStr = Regex.Replace(xmlStr, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", string.Empty);
            xdoc.LoadXml(xmlStr);
            XmlElement el = xdoc.DocumentElement;
            XmlNodeList xml = el.ChildNodes;
            foreach (XmlNode item in xml)
            {
                if (item.Attributes["p"] != null)
                {
                    try
                    {
                        string heheda = item.Attributes["p"].Value;
                        string[] haha = heheda.Split(',');
                        var location = DanmakuLocation.Roll;
                        switch (haha[1])
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
                        var colorCode = int.Parse(haha[3]);
                        if (colorCode < 0x1000000)
                            colorCode |= unchecked((int)0xFF000000);
                        ls.Add(new DanmakuModel
                        {
                            Time = double.Parse(haha[0]),
                            Location = location,
                            Size = double.Parse(haha[2]),
                            Color = Color.FromArgb((byte)((colorCode >> 24) & 0xFF), (byte)((colorCode >> 16) & 0xFF), (byte)((colorCode >> 8) & 0xFF), (byte)(colorCode & 0xFF)),
                            SendTime = haha[4],
                            Pool = haha[5],
                            SendID = haha[6],
                            RowID = haha[7],
                            Text = item.InnerText,
                            Source = item.OuterXml,
                            FromSite = DanmakuSite.Bilibili
                        });
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }
            }
            return ls;
        }
    }
}
