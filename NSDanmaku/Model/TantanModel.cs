using System.Collections.Generic;
using Newtonsoft.Json;

namespace NSDanmaku.Model
{
    public class TantanSearchModel
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
        [JsonProperty("errorCode")]
        public int ErrorCode { get; set; }
        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }
        [JsonProperty("animes")]
        public List<Animes> Animes { get; set; }
    }
    public class Animes
    {
        [JsonProperty("animeId")]
        public int AnimeId { get; set; }
        [JsonProperty("animeTitle")]
        public string AnimeTitle { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("episodes")]
        public List<Episodes> Episodes { get; set; }
    }
    public class Episodes
    {
        [JsonProperty("episodeId")]
        public int EpisodeId { get; set; }
        [JsonProperty("episodeTitle")]
        public string EpisodeTitle { get; set; }
        [JsonProperty("animeTitle")]
        public string AnimeTitle { get; set; }
    }

    public class CommentModel
    {
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("comments")]
        public List<Comments> Comments { get; set; }
    }
    public class Comments
    {
        [JsonProperty("cid")]
        public long Cid { get; set; }
        [JsonProperty("m")]
        public string Text { get; set; }
        [JsonProperty("p")]
        public string Time { get; set; }
    }
}
